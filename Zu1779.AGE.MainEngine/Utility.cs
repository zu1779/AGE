namespace Zu1779.AGE.MainEngine
{
    using System;

    public class Utility
    {
        private static readonly Random rng = new Random();

        public string GenerateToken(short numOfBytes = 128)
        {
            if (numOfBytes < 10) throw new ArgumentException($"{nameof(numOfBytes)} must be 10 or higher", nameof(numOfBytes));
            var bytes = new byte[numOfBytes];
            rng.NextBytes(bytes);
            var response = Convert.ToBase64String(bytes);
            return response;
        }
    }
}
