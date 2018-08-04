namespace Zu1779.AGE.Env.TestEnv.EnvCore
{
    using System;

    internal class Engine
    {
        public Engine()
        {

        }
    }

    internal class Location
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public GeoLocation GeoLocation { get; set; }
    }

    internal class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public GeoLocation GeoLocation { get; set; }
    }

    internal class GeoLocation
    {
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
    }
}
