namespace Zu1779.AGE.MainEngine
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Common.Logging;

    using m = Zu1779.AGE.MainEngine.Model;

    //TODO: //LASTWORK: decide about separate environment from agent (test if an agent can read private properties of environment via reflection)

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

        private readonly ConcurrentDictionary<string, m.EnvironmentOrchestrator> environments = new ConcurrentDictionary<string, m.EnvironmentOrchestrator>();

        #region Environments
        public void AddEnvironment(string environmentCode, string environmentPath)
        {
            if (environments.ContainsKey(environmentCode)) throw new ApplicationException($"Environment {environmentCode} already exists");
            var environment = new m.EnvironmentOrchestrator(environmentCode, log);
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
            var environment = environments[environmentCode];
            environment.CheckStatus();
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
