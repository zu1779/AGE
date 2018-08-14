namespace Zu1779.AGE.Agent.CardGameAgent
{
    using System;
    using System.Collections.Generic;

    using Zu1779.AGE.Contract;
    using Zu1779.AGE.Env.CardGameEnv.Contract;

    [Serializable]
    public class CardGameAgent : MarshalByRefObject, IAgent, IAgentCardGame
    {
        public CardGameAgent(string code)
        {
            Code = code;
        }

        #region IAgent
        public string Code { get; }

        public CheckStatusResponse CheckStatus()
        {
            return new CheckStatusResponse
            {
                HealthState = true,
            };
        }

        public void SetUp(SetUpRequest request)
        {
            throw new System.NotImplementedException();
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
            hand.Clear();
            hand.AddRange(cards);
        }

        public void CardPlayed(List<Card> previousCardsInTable, List<Card> currentCardsInTable, Card cardPlayed)
        {
            table.Clear();
            table.AddRange(currentCardsInTable);
        }

        public void YourTurn(List<Card> cardsInTable)
        {
            table.Clear();
            table.AddRange(cardsInTable);
        }
        #endregion

        private List<Card> hand { get; } = new List<Card>();
        private List<Card> table { get; } = new List<Card>();
    }
}
