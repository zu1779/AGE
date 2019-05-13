namespace Zu1779.AGE.Agent.CardGame
{
    using System;
    using System.Collections.Generic;

    using Zu1779.AGE.Contract;
    using Zu1779.AGE.Environment.CardGame.Contract;

    [Serializable]
    public class CardGameAgent : MarshalByRefObject, IAgent, IAgentCardGame
    {
        public CardGameAgent(string code, string token)
        {
            Code = code;
            Token = token;
            rng = new Random();
        }
        private readonly Random rng;
        private IEnvironmentCommunication environment;
        private IEnvironmentCardGame EnvCardGame { get { return (IEnvironmentCardGame)environment; } }

        #region IAgent
        public string Code { get; }
        public string Token { get; }

        public CheckStatusResponse CheckStatus()
        {
            return new CheckStatusResponse
            {
                HealthState = true,
            };
        }

        public void SetUp(SetUpRequest request)
        {
            environment = request.Environment;
        }

        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }

        public void TearDown()
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region IAgentCardGame
        public void InitialHand(List<Card> cards)
        {
            Hand.Clear();
            Hand.AddRange(cards);
        }

        public void CardPlayed(List<Card> previousCardsInTable, List<Card> currentCardsInTable, Card cardPlayed)
        {
            Table.Clear();
            Table.AddRange(currentCardsInTable);
        }

        public void YourTurn(List<Card> cardsInTable)
        {
            Table.Clear();
            Table.AddRange(cardsInTable);

            var cardToPlay = Hand[rng.Next(Hand.Count)];
            EnvCardGame.PlayCard(Code, Token, cardToPlay);
            Hand.Remove(cardToPlay);
        }
        #endregion

        private List<Card> Hand { get; } = new List<Card>();
        private List<Card> Table { get; } = new List<Card>();
    }
}
