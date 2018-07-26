﻿namespace Zu1779.AGE.MainEngine
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
            var environment = new m.Environment(environmentCode, log);
            var added = environments.TryAdd(environmentCode, environment);
            if (!added) throw new ApplicationException($"Environment {environmentCode} already exists");
            environment.PrepareEnvironment(environmentDllPath);
        }

        public List<m.Environment> GetEnvironments()
        {
            return environments.Values.ToList();
        }
        #endregion

        #region Agents
        public void AddAgent(string environmentCode, string agentCode, string agentDllPath)
        {

            var agent = new m.Agent(agentCode, log);
            
        }
        #endregion
    }
}
