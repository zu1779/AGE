namespace Zu1779.AGE.Env.CardGameEnv
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using log4net;

    using Zu1779.AGE.Env.CardGameEnv.Contract;

    internal class Engine
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Engine));
        private Deck mace;

        public void SetUp()
        {
            mace = new Deck();
            mace.SetUp();
            mace.Shuffle(200);

            //TODO: setup agents
        }

        public void StartGame()
        {
            log.Info($"{nameof(StartGame)}");

        }

        public void StopGame()
        {
            log.Info($"{nameof(StopGame)}");
        }
    }

    internal class Deck
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Engine));
        private List<Card> cards { get; set; }

        public void SetUp()
        {
            cards = new List<Card>();
            var seeds = Enum.GetValues(typeof(Seed)).Cast<Seed>();
            foreach (var seed in seeds)
            {
                for (byte ciclo = 1; ciclo <= 10; ciclo++)
                {
                    var card = new Card { Seed = seed, Number = ciclo };
                    cards.Add(card);
                }
            }
            //log.Info($"Setted up Deck:\r\n{string.Join("\r\n", cards.Select((c, i) => $"[{i}] - {c.Number} {c.Seed}"))}");
        }

        public void Shuffle(int numberOfShuffle)
        {
            Random rng = new Random();
            for (int ciclo = 0; ciclo < numberOfShuffle; ciclo++)
            {
                cards = cards.OrderBy(c => rng.Next()).ToList();
            }
            //log.Info($"Shuffled Deck:\r\n{string.Join("\r\n", cards.Select((c, i) => $"[{i}] - {c.Number} {c.Seed}"))}");
        }
    }
}
