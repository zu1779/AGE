namespace Zu1779.AGE.Environment.CardGame.Contract
{
    using System.Collections.Generic;

    public interface IAgentCardGame
    {
        void InitialHand(List<Card> cards);
        void CardPlayed(List<Card> previousCardsInTable, List<Card> currentCardsInTable, Card cardPlayed);
        void YourTurn(List<Card> cardsInTable);
    }
}
