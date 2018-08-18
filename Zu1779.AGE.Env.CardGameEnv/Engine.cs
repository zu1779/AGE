namespace Zu1779.AGE.Env.CardGameEnv
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using log4net;

    using Zu1779.AGE.Contract;
    using Zu1779.AGE.Env.CardGameEnv.Contract;

    internal class Engine
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Engine));
        public Deck Deck;
        public ConcurrentBag<IAgent> Agents { get; } = new ConcurrentBag<IAgent>();
        public readonly Dictionary<int, List<Card>> PlayerCards = new Dictionary<int, List<Card>>()
        {
            [0] = new List<Card>(), [1] = new List<Card>(), [2] = new List<Card>(), [3] = new List<Card>(),
        };
        public List<Card> Table { get; } = new List<Card>();

        public void SetUp()
        {
            Deck = new Deck();
            Deck.SetUp(200);
            for (int cycle = 0; cycle < 4; cycle++) PlayerCards[cycle].Clear();
            //TODO: setup agents
        }

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
                (Agents.ElementAt(0) as IAgentCardGame).InitialHand(PlayerCards[cycle]);
            // Start first turn
            var firstPlayer = Agents.ElementAt(0) as IAgentCardGame;
            firstPlayer.YourTurn(Table);
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
