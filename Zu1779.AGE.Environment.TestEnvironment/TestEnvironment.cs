namespace Zu1779.AGE.Environment.TestEnvironment
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;

    using log4net;

    using Zu1779.AGE.Contract;

    [Serializable]
    public class TestEnvironment : MarshalByRefObject, IEnvironment
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TestEnvironment));

        static TestEnvironment()
        {
            FileInfo fileInfo = new FileInfo("log4net_config.xml");
            log4net.Config.XmlConfigurator.Configure(fileInfo);
            log.Info("static TestEnvironment()");
        }

        public TestEnvironment()
        {
            log.Info("public TestEnvironment");
        }

        private ConcurrentDictionary<string, IAgent> agents { get; } = new ConcurrentDictionary<string, IAgent>();

        public (bool isValid, string unvalidCause) CheckAgentValidity(AgentTypeEnum agentType, IAgent agent)
        {

        }

        public void AttachAgent(AgentTypeEnum agentType, string agentCode, IAgent agent)
        {
            var added = agents.TryAdd(agentCode, agent);
            if (!added) throw new ApplicationException($"Environment cannot add agent");
        }

        public void Start()
        {
            log.Info("Start()");
        }

        public void Stop()
        {
            log.Info("Stop()");
        }
    }
}
