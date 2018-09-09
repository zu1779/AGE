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
        }

        private byte currentPlayerIndex;
        private byte lastPlayerTook;
        private Card currentCard;
        private readonly object lockNextTurn = new object();
        private bool nextTurn = false;

        public void StartGame()
        {
            log.Info($"{nameof(StartGame)}");
            // Cleanup
            for (byte cycle = 0; cycle < 4; cycle++)
            {
                PlayerCards[cycle].Clear();
                PlayerScopa[cycle].Clear();
                PlayerTaken[cycle].Clear();
            }
            currentPlayerIndex = 0;
            lastPlayerTook = 0; // Will surely been set during game
            nextTurn = false;
            currentCard = null;

            Deck.SetUp(200);
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
                executeGameCard(currentPlayerIndex, currentCard, cycle == 39);
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
            calculateScore();
            //var totalCards = PlayerTaken.Sum(c => c.Value.Count) + Table.Count + PlayerCards.Sum(c => c.Value.Count);
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

        private void executeGameCard(byte playingAgentIndex, Card card, bool isLastCard)
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
            executeGameRules(card, currentPlayerIndex, isLastCard);
            log.Info($"Cards on table ({Table.Count}): {Table.ToCardString()}");

            // Acknowledge other players
            for (byte cycle = 0; cycle < 4; cycle++)
            {
                if (cycle != playingAgentIndex) (Agents[cycle] as IAgentCardGame).CardPlayed(oldTable, Table, card);
            }
        }

        private void executeGameRules(Card card, byte currentPlayerIndex, bool isLastCard)
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
                    // Check scopa condition (last card played is never a scopa)
                    if (!Table.Any() && !isLastCard) PlayerScopa[currentPlayerIndex].Add(card);
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

        private void calculateScore()
        {
            var takenTeam0 = PlayerTaken[0].Union(PlayerTaken[2]).ToList();
            var takenTeam1 = PlayerTaken[1].Union(PlayerTaken[3]).ToList();
            var scoreTeam0 = 0;
            var scoreTeam1 = 0;
            // Scope
            var ScopeTeam0 = PlayerScopa[0].Count + PlayerScopa[2].Count;
            var ScopeTeam1 = PlayerScopa[1].Count + PlayerScopa[3].Count;
            log.Info($"Team0 made {ScopeTeam0} scope");
            log.Info($"Team1 made {ScopeTeam1} scope");
            scoreTeam0 += ScopeTeam0;
            scoreTeam1 += ScopeTeam1;
            // Cards
            var cardsTeam0 = takenTeam0.Count;
            var cardsTeam1 = takenTeam1.Count;
            if (cardsTeam0 > cardsTeam1)
            {
                scoreTeam0++;
                log.Info("Team0 achieve cards (1 point)");
            }
            else if (cardsTeam1 > cardsTeam0)
            {
                scoreTeam1++;
                log.Info("Team1 achieve cards (1 point)");
            }
            else log.Info("Cards was not achieve");
            // Denari
            var denariTeam0 = takenTeam0.Count(c => c.Seed == Seed.Denari);
            var denariTeam1 = takenTeam1.Count(c => c.Seed == Seed.Denari);
            if (denariTeam0 > denariTeam1)
            {
                scoreTeam0++;
                log.Info("Team0 achieve denari (1 point)");
            }
            else if (denariTeam1 > denariTeam0)
            {
                scoreTeam1++;
                log.Info("Team1 achieve denari (1 point)");
            }
            else log.Info("Denari was not achieve");
            // Settebello
            if (takenTeam0.Any(c => c.Number == 7 && c.Seed == Seed.Denari))
            {
                scoreTeam0++;
                log.Info("Team0 achieve Settebello (1 point)");
            }
            else if (takenTeam1.Any(c => c.Number == 7 && c.Seed == Seed.Denari))
            {
                scoreTeam1++;
                log.Info("Team1 achieve Settebello (1 point)");
            }
            // Re bello
            if (takenTeam0.Any(c => c.Number == 10 && c.Seed == Seed.Denari))
            {
                scoreTeam0++;
                log.Info("Team0 achieve Re Bello (1 point)");
            }
            else if (takenTeam1.Any(c => c.Number == 10 && c.Seed == Seed.Denari))
            {
                scoreTeam1++;
                log.Info("Team1 achieve Re Bello (1 point)");
            }
            // Primiera: 7 = 21 punti, 6 = 18 punti, asso = 16 punti, 5 = 15 punti, 4 = 14 punti, 3 = 13 punti, 2 = 12 punti
            var primieraCardsTeam0 = takenTeam0.GroupBy(c => c.Seed, c => new
            {
                c.Number,
                Value = c.Number == 7 ? 21 : c.Number == 6 ? 18 : c.Number == 1 ? 16 : c.Number == 5 ? 15 : c.Number == 4 ? 14
                    : c.Number == 3 ? 13 : c.Number == 2 ? 12 : 0,
            }).Select(c => new
            {
                Seed = c.Key,
                c.OrderByDescending(d => d.Value).First().Number,
                c.OrderByDescending(d => d.Value).First().Value,
            });
            var primieraTeam0 = primieraCardsTeam0.Sum(c => c.Value);
            log.Info($"Team0 primiera: {primieraCardsTeam0.Select(c => new Card { Number = c.Number, Seed = c.Seed }).ToCardString()}");
            var primieraCardsTeam1 = takenTeam1.GroupBy(c => c.Seed, c => new
            {
                c.Number,
                Value = c.Number == 7 ? 21 : c.Number == 6 ? 18 : c.Number == 1 ? 16 : c.Number == 5 ? 15 : c.Number == 4 ? 14
                    : c.Number == 3 ? 13 : c.Number == 2 ? 12 : 0,
            }).Select(c => new
            {
                Seed = c.Key,
                c.OrderByDescending(d => d.Value).First().Number,
                c.OrderByDescending(d => d.Value).First().Value,
            });
            var primieraTeam1 = primieraCardsTeam1.Sum(c => c.Value);
            log.Info($"Team1 primiera: {primieraCardsTeam1.Select(c => new Card { Number = c.Number, Seed = c.Seed }).ToCardString()}");

            if (primieraTeam0 > primieraTeam1)
            {
                scoreTeam0++;
                log.Info("Team0 achieve Primiera (1 point)");
            }
            else if (primieraTeam1 > primieraTeam0)
            {
                scoreTeam1++;
                log.Info("Team1 achieve Primiera (1 point)");
            }
            else log.Info("Primiera was not achieve");
            // Napoli
            var napoliTeam0 = checkNapoli(takenTeam0);
            var napoliTeam1 = checkNapoli(takenTeam1);
            if (napoliTeam0 > 0)
            {
                scoreTeam0 += napoliTeam0;
                log.Info($"Team0 achieve Napoli for {napoliTeam0} points");
            }
            else if (napoliTeam1 > 0)
            {
                scoreTeam1 += napoliTeam1;
                log.Info($"Team1 achieve Napoli for {napoliTeam1} points");
            }
            else log.Info("Napoli was not achieve");
            log.Info($"Team0 made {scoreTeam0} points");
            log.Info($"Team1 made {scoreTeam1} points");
            if (scoreTeam0 > scoreTeam1) log.Info("Team0 won!!!");
            else if (scoreTeam1 > scoreTeam0) log.Info("Team1 won!!!");
            else log.Info("Draw!!!");
        }

        private int checkNapoli(List<Card> cards)
        {
            if (!cards.Any(c => c.Number == 1 && c.Seed == Seed.Denari)) return 0;
            if (!cards.Any(c => c.Number == 2 && c.Seed == Seed.Denari)) return 0;
            if (!cards.Any(c => c.Number == 3 && c.Seed == Seed.Denari)) return 0;
            for (byte cycle = 4; cycle <= 10; cycle++)
            {
                if (!cards.Any(c => c.Number == cycle && c.Seed == Seed.Denari)) return cycle - 1;
            }
            return 10;
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
