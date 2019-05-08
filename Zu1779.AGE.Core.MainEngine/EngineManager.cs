namespace Zu1779.AGE.Core.MainEngine
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using m = Model;

    using log = NLog;

    public class EngineManager : IDisposable
    {
        public EngineManager()
        {
            this.logger = log.LogManager.GetCurrentClassLogger();
            AppDomain.MonitoringIsEnabled = true;
        }
        private readonly log.ILogger logger;

        public void Dispose()
        {

        }

        private readonly ConcurrentDictionary<string, m.EnvironmentOrchestrator> environments 
            = new ConcurrentDictionary<string, m.EnvironmentOrchestrator>();

        #region Environments
        public void AddEnvironment(string environmentCode, string environmentPath)
        {
            if (environments.ContainsKey(environmentCode)) throw new ApplicationException($"Environment {environmentCode} already exists");
            var environment = new m.EnvironmentOrchestrator(environmentCode, logger);
            var added = environments.TryAdd(environmentCode, environment);
            environment.Prepare(environmentPath);
            if (!added) throw new ApplicationException($"Environment {environmentCode} already exists");
        }

        public List<m.EnvironmentOrchestrator> GetEnvironments()
        {
            return environments.Values.ToList();
        }

        public m.EnvironmentOrchestrator GetEnvironment(string environmentCode)
        {
            return environments[environmentCode];
        }

        public void CheckStatusEnvironment(string environmentCode)
        {
            if (environments.ContainsKey(environmentCode))
            {
                var environment = environments[environmentCode];
                environment.CheckStatus();
            }
            else throw new ApplicationException($"Environment '{environmentCode}' does not exists");
        }

        public void SetUpEnvironment(string environmentCode)
        {
            var environment = environments[environmentCode];
            environment.SetUp();
        }

        public void StartEnvironment(string environmentCode)
        {
            var environment = environments[environmentCode];
            environment.Start();
        }

        public void PauseEnvironment(string environmentCode)
        {
            var environment = environments[environmentCode];
            environment.Pause();
        }

        public void ContinueEnvironment(string environmentCode)
        {
            var environment = environments[environmentCode];
            environment.Continue();
        }

        public void CommandEnvironment(string environmentCode, int command)
        {
            var environment = environments[environmentCode];
            environment.Command(command);
        }

        public void StopEnvironment(string environmentCode)
        {
            var environment = environments[environmentCode];
            environment.Stop();
        }

        public void TearDownEnvironment(string environmentCode)
        {
            var environment = environments[environmentCode];
            environment.TearDown();
        }
        #endregion

        #region Agents
        public void AddAgent(string environmentCode, string agentCode, string agentPath)
        {
            var environment = environments[environmentCode];
            environment.AddAgent(agentCode, agentPath);
        }

        public List<m.AgentOrchestrator> GetAgents(string environmentCode)
        {
            var environment = environments[environmentCode];
            return environment.GetAgents();
        }
        #endregion
    }
}
