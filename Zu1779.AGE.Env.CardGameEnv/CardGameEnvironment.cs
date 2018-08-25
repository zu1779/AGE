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
    public class CardGameEnvironment : MarshalByRefObject, IEnvironment, IEnvironmentCommunication, IEnvironmentCardGame
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
        private readonly Engine engine = new Engine();

        #region IEnvironment
        public (bool isValid, string unvalidCause) CheckAgentValidity(AgentTypeEnum agentType, IAgent agent)
        {
            if (!(agent is IAgentCardGame)) return (false, $"Agent doesn't implement {nameof(IAgentCardGame)}");
            else return (true, null);
        }

        public void AttachAgent(AgentTypeEnum agentType, string agentCode, IAgent agent)
        {
            engine.Agents.Add(agent);
        }

        public void CheckStatus()
        {
            log.Info($"Deck ({engine.Deck.Cards.Count}): {engine.Deck}");
            log.Info($"Table ({engine.Table.Count}): {engine.Table.ToCardString()}");
            for (byte ciclo = 0; ciclo < 4; ciclo++)
            {
                log.Info($"Player {ciclo + 1} ({engine.PlayerCards[ciclo].Count}): {engine.PlayerCards[ciclo].ToCardString()}");
            }
        }

        public void SetUp()
        {
            engine.SetUp();
        }

        public void TearDown()
        {
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

        #region IEnvironmentCardGame
        public void PlayCard(Card card)
        {
            Console.WriteLine("Checkpoint");
        }
        #endregion
    }
}
