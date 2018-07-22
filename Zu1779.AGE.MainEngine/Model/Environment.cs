namespace Zu1779.AGE.MainEngine.Model
{
    using System;
    using System.Collections.Concurrent;

    [Serializable]
    public class Environment
    {
        private readonly ConcurrentDictionary<string, Agent> agents = new ConcurrentDictionary<string, Agent>();
    }
}
