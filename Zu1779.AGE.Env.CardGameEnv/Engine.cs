namespace Zu1779.AGE.Env.CardGameEnv
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Engine
    {
        private Deck mace;

        public void SetUp()
        {
            mace = new Deck();
            mace.SetUp();
            mace.Shuffle(20);

            //TODO: setup agents
        }

        public void StartGame()
        {

        }

        public void StopGame()
        {

        }
    }

    internal class Deck
    {
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
        }

        public void Shuffle(int numberOfShuffle)
        {
            Random rng = new Random();
            for (int ciclo = 0; ciclo < numberOfShuffle; ciclo++)
            {
                cards = cards.OrderBy(c => rng.Next()).ToList();
            }
        }
    }

    internal class Card
    {
        public Seed Seed { get; set; }
        public byte Number { get; set; }
    }
}
