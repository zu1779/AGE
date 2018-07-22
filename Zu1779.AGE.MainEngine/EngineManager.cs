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

        private readonly ConcurrentBag<m.Environment> environments = new ConcurrentBag<m.Environment>();
    }
}
