namespace Zu1779.AGE.MainEngine
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Common.Logging;

    using m = Zu1779.AGE.MainEngine.Model;

    public class EngineManager : IDisposable
    {
        public EngineManager(ILog log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }
        private readonly ILog log;

        public void Dispose()
        {

        }

        private readonly ConcurrentDictionary<string, m.Environment> environments = new ConcurrentDictionary<string, m.Environment>();

        #region Environments
        public void AddEnvironment(string environmentCode, string environmentDllPath)
        {
            if (environments.ContainsKey(environmentCode)) throw new ApplicationException($"Environment {environmentCode} already exists");
            var environment = new m.Environment(environmentCode, log);
            var added = environments.TryAdd(environmentCode, environment);
            environment.Prepare(environmentDllPath);
            if (!added) throw new ApplicationException($"Environment {environmentCode} already exists");
        }

        public List<m.Environment> GetEnvironments()
        {
            return environments.Values.ToList();
        }

        public m.Environment GetEnvironment(string environmentCode)
        {
            return environments[environmentCode];
        }
        #endregion

        #region Agents
        public void AddAgent(string environmentCode, string agentCode, string agentDllPath)
        {
            var environment = environments[environmentCode];
            environment.AddAgent(agentCode);
        }

        public List<m.Agent> GetAgents(string environmentCode)
        {
            var environment = environments[environmentCode];
            return environment.GetAgents();
        }
        #endregion
    }
}
