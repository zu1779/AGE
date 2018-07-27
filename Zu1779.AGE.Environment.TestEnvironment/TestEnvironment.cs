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

    using Zu1779.AGE.Contract;
    using Zu1779.AGE.Environment.TestEnvironment.Contract;

    [Serializable]
    public class TestEnvironment : MarshalByRefObject, IEnvironment
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

        public TestEnvironment()
        {
            log.Info("public TestEnvironment");
        }

        private ConcurrentDictionary<string, IAgent> agents { get; } = new ConcurrentDictionary<string, IAgent>();

        public (bool isValid, string unvalidCause) CheckAgentValidity(AgentTypeEnum agentType, IAgent agent)
        {
            if (agent is IAgentCommunication) return (true, null);
            else return (false, $"Agent doesn't implement {nameof(IAgentCommunication)}");
        }

        public void AttachAgent(AgentTypeEnum agentType, string agentCode, IAgent agent)
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
            log.Info(nameof(SetUp));
        }

        public void TearDown()
        {
            log.Info(nameof(TearDown));
        }

        public void Start()
        {
            log.Info(nameof(Start));
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
            log.Info(nameof(Stop));
        }
    }
}
