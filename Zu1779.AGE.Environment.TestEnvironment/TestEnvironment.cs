namespace Zu1779.AGE.Environment.TestEnvironment
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using log4net;

    using cnt = Zu1779.AGE.Contract;
    using envcnt = Zu1779.AGE.Environment.TestEnvironment.Contract;

    [Serializable]
    public class TestEnvironment : MarshalByRefObject, cnt.IEnvironment, cnt.IEnvironmentCommunication
    {
        private static readonly ILog log;

        static TestEnvironment()
        {
            string logDirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string logFilePath = Path.Combine(logDirPath, "log4net_config.xml");
            FileInfo fileInfo = new FileInfo(logFilePath);
            log4net.Config.XmlConfigurator.Configure(fileInfo);
            log = LogManager.GetLogger(typeof(TestEnvironment));
            log.Info("static TestEnvironment");
        }

        public TestEnvironment(string environmentCode)
        {
            log.Info("public TestEnvironment");
            code = environmentCode;
        }

        private ConcurrentDictionary<string, cnt.IAgent> agents { get; } = new ConcurrentDictionary<string, cnt.IAgent>();
        private string code { get; }

        public (bool isValid, string unvalidCause) CheckAgentValidity(cnt.AgentTypeEnum agentType, cnt.IAgent agent)
        {
            if (agent is cnt.IAgentCommunication) return (true, null);
            else return (false, $"Agent doesn't implement {nameof(cnt.IAgentCommunication)}");
        }

        public void AttachAgent(cnt.AgentTypeEnum agentType, string agentCode, cnt.IAgent agent)
        {
            var added = agents.TryAdd(agentCode, agent);
            if (!added) throw new ApplicationException($"Environment cannot add agent");
        }

        public void CheckStatus()
        {
            log.Info(nameof(CheckStatus));
        }

        public void SetUp()
        {
            //log.Info(nameof(SetUp));
            //foreach (var agent in agents)
            //{
            //    var request = new cnt.SetUpRequest { Environment = this };
            //    agent.Value.SetUp(request);
            //}
        }

        public void TearDown()
        {
            //log.Info(nameof(TearDown));
            //foreach (var agent in agents)
            //{
            //    agent.Value.TearDown();
            //}
        }

        public void Start()
        {
            //log.Info(nameof(Start));
            //foreach (var agent in agents)
            //{
            //    agent.Value.Start();
            //}
        }

        public void Pause()
        {
            log.Info(nameof(Pause));
        }

        public void Continue()
        {
            log.Info(nameof(Continue));
        }

        public void Command(int command)
        {
            log.Info($"{nameof(Command)} {command}");
        }

        public void Stop()
        {
            //log.Info(nameof(Stop));
            //foreach (var agent in agents)
            //{
            //    agent.Value.Stop();
            //}
        }
    }
}
