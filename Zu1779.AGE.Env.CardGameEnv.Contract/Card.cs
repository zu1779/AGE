namespace Zu1779.AGE.Env.CardGameEnv.Contract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Serializable]
    public class Card
    {
        public Seed Seed { get; set; }
        public byte Number { get; set; }

        public override string ToString()
        {
            return $"{nameof(Number)}: {Number}, {nameof(Seed)}: {Seed}";
        }
    }

    public static class CardExtensions
    {
        public static string ToCardString(this IEnumerable<Card> cards)
        {
            return string.Join(", ", cards.Select(c => $"{c.Number} {c.Seed}"));
        }
    }
}
