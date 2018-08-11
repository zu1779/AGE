namespace Zu1779.AGE.Env.CardGameEnv
{
    using System;
    using System.Collections.Concurrent;

    using Zu1779.AGE.Contract;
    using Zu1779.AGE.Env.CardGameEnv.Contract;

    [Serializable]
    public class CardGameEnvironment : MarshalByRefObject, IEnvironment
    {
        public CardGameEnvironment(string code)
        {
            this.code = code;
        }
        private string code { get; }
        private Engine engine;
        private ConcurrentDictionary<string, IAgentCardGame> agents { get; } = new ConcurrentDictionary<string, IAgentCardGame>();

        #region IEnvironment
        public (bool isValid, string unvalidCause) CheckAgentValidity(AgentTypeEnum agentType, IAgent agent)
        {
            if (agent is IAgentCardGame) return (true, null);
            else return (false, $"Agent doesn't implement {nameof(IAgentCardGame)}");
        }

        public void AttachAgent(AgentTypeEnum agentType, string agentCode, IAgent agent)
        {
            throw new NotImplementedException();
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

        public void Continue()
        {
            throw new NotImplementedException();
        }

        public void Command(int command)
        {
            throw new NotImplementedException();
        }

        public void Pause()
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
