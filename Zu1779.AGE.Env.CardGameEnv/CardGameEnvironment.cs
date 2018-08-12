namespace Zu1779.AGE.Env.CardGameEnv
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Reflection;
    using System.Threading;

    using log4net;

    using Zu1779.AGE.Contract;
    using Zu1779.AGE.Env.CardGameEnv.Contract;

    [Serializable]
    public class CardGameEnvironment : MarshalByRefObject, IEnvironment
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CardGameEnvironment));

        public CardGameEnvironment(string code)
        {
            string logDirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string logFilePath = Path.Combine(logDirPath, "log4net_config.xml");
            FileInfo fileInfo = new FileInfo(logFilePath);
            log4net.Config.XmlConfigurator.Configure(fileInfo);
            this.code = code;
        }
        private string code { get; }
        private Engine engine;
        private ConcurrentDictionary<string, IAgent> agents { get; } = new ConcurrentDictionary<string, IAgent>();

        #region IEnvironment
        public (bool isValid, string unvalidCause) CheckAgentValidity(AgentTypeEnum agentType, IAgent agent)
        {
            if (agent is IAgentCardGame) return (true, null);
            else return (false, $"Agent doesn't implement {nameof(IAgentCardGame)}");
        }

        public void AttachAgent(AgentTypeEnum agentType, string agentCode, IAgent agent)
        {
            var added = agents.TryAdd(agentCode, agent);
            if (!added) throw new ApplicationException($"Environment cannot add agent");
        }

        public void CheckStatus()
        {
            //TODO: da implementare
        }

        public void SetUp()
        {
            engine = new Engine();
            engine.SetUp();
        }

        public void TearDown()
        {
            engine = null;
        }

        public void Start()
        {
            engine.StartGame();
        }

        public void Command(int command)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            engine.StopGame();
        }
        #endregion
    }
}
