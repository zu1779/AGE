namespace Zu1779.AGE.MainEngine
{
    using System;
    using System.Collections.Concurrent;

    using m = Zu1779.AGE.MainEngine.Model;

    public class EngineManager : IDisposable
    {
        public EngineManager()
        {

        }

        public void Dispose()
        {

        }

        private readonly ConcurrentDictionary<string, m.Environment> environments = new ConcurrentDictionary<string, m.Environment>();


    }
}
