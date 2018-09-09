namespace Zu1779.AGE.Env.CardGameEnv
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using log4net;

    using Zu1779.AGE.Contract;
    using Zu1779.AGE.Env.CardGameEnv.Contract;

    internal class Engine
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Engine));
        public Deck Deck;
        public List<IAgent> Agents { get; } = new List<IAgent>();
        public readonly Dictionary<byte, List<Card>> PlayerCards = new Dictionary<byte, List<Card>>()
        {
            [0] = new List<Card>(),
            [1] = new List<Card>(),
            [2] = new List<Card>(),
            [3] = new List<Card>(),
        };
        public readonly Dictionary<byte, List<Card>> PlayerTaken = new Dictionary<byte, List<Card>>()
        {
            [0] = new List<Card>(),
            [1] = new List<Card>(),
            [2] = new List<Card>(),
            [3] = new List<Card>(),
        };
        public readonly Dictionary<byte, List<Card>> PlayerScopa = new Dictionary<byte, List<Card>>
        {
            [0] = new List<Card>(),
            [1] = new List<Card>(),
            [2] = new List<Card>(),
            [3] = new List<Card>(),
        };
        public List<Card> Table { get; } = new List<Card>();

        public void SetUp()
        {
            Deck = new Deck();
            Deck.SetUp(200);
            for (byte cycle = 0; cycle < 4; cycle++) PlayerCards[cycle].Clear();
            //TODO: setup agents
        }

        private byte currentPlayerIndex = 0;
        private byte lastPlayerTook;
        private Card currentCard;
        private readonly object lockNextTurn = new object();
        private bool nextTurn = false;
        public void StartGame()
        {
            log.Info($"{nameof(StartGame)}");
            // Distribute cards
            for (byte cycle = 0, player = 0; cycle < 40; cycle++, player = (++player == 4 ? (byte)0 : player))
            {
                var card = Deck.Cards.Dequeue();
                PlayerCards[player].Add(card);
            }
            for (byte cycle = 0; cycle < 4; cycle++)
                (Agents[cycle] as IAgentCardGame).InitialHand(PlayerCards[cycle]);

            // Start game cycle
            currentPlayerIndex = 0;
            for (byte cycle = 0; cycle < 40; cycle++, currentPlayerIndex = (++currentPlayerIndex == 4 ? (byte)0 : currentPlayerIndex))
            {
                var currentPlayer = Agents[currentPlayerIndex] as IAgentCardGame;
                log.Info($"Going to make turn {cycle} to player {currentPlayerIndex}");
                currentPlayer.YourTurn(Table);

                // Waiting for player to play card
                while (!nextTurn)
                {
                    //TODO: try suspend current thread (and awake from PlayCard function (remember to debug thread id to identify if suspending and resuming correct thread))
                    // nop
                }
                lock (lockNextTurn)
                {
                    nextTurn = false;
                }
                executeGameCard(currentPlayerIndex, currentCard);
            }
            // Give cards remained on table to the last player who took
            PlayerTaken[lastPlayerTook].AddRange(Table);
            Table.Clear();
            // Calculate score
            for (byte cycle = 0; cycle < 4; cycle++)
            {
                log.Info($"Player {cycle}");
                log.Info($"Taken cards ({PlayerTaken[cycle].Count}): {PlayerTaken[cycle].ToCardString()}");
                log.Info($"Scope made ({PlayerScopa[cycle].Count}): {PlayerScopa[cycle].ToCardString()}");
            }
            var totalCards = PlayerTaken.Sum(c => c.Value.Count) + Table.Count + PlayerCards.Sum(c => c.Value.Count);
            //log.Debug("Ended game: calculating total cards");
            //log.Debug($"Total Cards: {totalCards}");
            //log.Debug($"Total in player hands: {PlayerCards.Sum(c => c.Value.Count)}");
            //log.Debug($"Total in table: {Table.Count}");
            //log.Debug($"Total taken: {PlayerTaken.Sum(c => c.Value.Count)}");
        }

        public void PlayCard(string code, string token, Card card)
        {
            // Check current player is valid
            var playingAgent = Agents.Single(c => c.Code == code);
            byte playingAgentIndex = (byte)Agents.IndexOf(playingAgent);
            if (playingAgentIndex != currentPlayerIndex)
            {
                string errorMessage = $"Agent {playingAgentIndex} trying to play during {currentPlayerIndex}";
                log.Error(errorMessage);
                throw new ApplicationException(errorMessage);
            }
            if (token != playingAgent.Token)
            {
                string errorMessage = $"Invalid token for agent {playingAgentIndex}";
                log.Error(errorMessage);
                throw new ApplicationException(errorMessage);
            }

            currentCard = card;

            // Set ack for next round (next player turn)
            lock (lockNextTurn)
            {
                nextTurn = true;
            }
        }

        private void executeGameCard(byte playingAgentIndex, Card card)
        {
            // Check player card is valid
            if (!PlayerCards[playingAgentIndex].Any(c => c.Number == card.Number && c.Seed == card.Seed))
            {
                string errorMessage = $"Card {card} played from agent {playingAgentIndex} is not of his cards";
                log.Error(errorMessage);
                throw new ApplicationException(errorMessage);
            }

            // Play the card (game rules)
            var oldTable = new List<Card>(Table);
            log.Info($"Player {currentPlayerIndex} played card {card}");
            executeGameRules(card, currentPlayerIndex);
            log.Info($"Cards on table ({Table.Count}): {Table.ToCardString()}");

            // Acknowledge other players
            for (byte cycle = 0; cycle < 4; cycle++)
            {
                if (cycle != playingAgentIndex) (Agents[cycle] as IAgentCardGame).CardPlayed(oldTable, Table, card);
            }
        }

        private void executeGameRules(Card card, byte currentPlayerIndex)
        {
            // Remove card from player hand
            PlayerCards[currentPlayerIndex].RemoveAll(c => c.Number == card.Number && c.Seed == card.Seed);
            // Execute rules
            // Ace played
            if (card.Number == 1)
            {
                PlayerTaken[currentPlayerIndex].Add(card);
                PlayerTaken[currentPlayerIndex].AddRange(Table);
                Table.Clear();
                lastPlayerTook = currentPlayerIndex;
            }
            else
            {
                //TODO: implement rule for take sum of cards and choose which if more than one possible choose
                if (Table.Any(c => c.Number == card.Number))
                {
                    var cardTaken = Table.First(c => c.Number == card.Number);
                    PlayerTaken[currentPlayerIndex].Add(card);
                    PlayerTaken[currentPlayerIndex].Add(cardTaken);
                    Table.Remove(cardTaken);
                    // Check scopa condition
                    if (!Table.Any()) PlayerScopa[currentPlayerIndex].Add(card);
                    lastPlayerTook = currentPlayerIndex;
                }
                // No card in table
                else
                {
                    Table.Add(card);
                }
            }
            var totalCards = PlayerTaken.Sum(c => c.Value.Count) + Table.Count + PlayerCards.Sum(c => c.Value.Count);
            if (totalCards != 40) log.Warn($"Total Cards: {totalCards} (not 40)");
            //else log.Debug($"Total Cards: {totalCards}");
            //log.Debug($"Total in player hands: {PlayerCards.Sum(c => c.Value.Count)}");
            //log.Debug($"Total in table: {Table.Count}");
            //log.Debug($"Total taken: {PlayerTaken.Sum(c => c.Value.Count)}");
        }

        public void StopGame()
        {
            log.Info($"{nameof(StopGame)}");
        }
    }

    internal class Deck
    {
        public Queue<Card> Cards { get; set; }

        public void SetUp(int numberOfShuffle)
        {
            var cards = new List<Card>();
            var seeds = Enum.GetValues(typeof(Seed)).Cast<Seed>();
            foreach (var seed in seeds)
            {
                for (byte ciclo = 1; ciclo <= 10; ciclo++)
                {
                    var card = new Card { Seed = seed, Number = ciclo };
                    cards.Add(card);
                }
            }
            Random rng = new Random();
            for (int ciclo = 0; ciclo < numberOfShuffle; ciclo++)
            {
                cards = cards.OrderBy(c => rng.Next()).ToList();
            }
            Cards = new Queue<Card>(cards);
        }

        public override string ToString()
        {
            return Cards.ToCardString();
        }
    }
}
