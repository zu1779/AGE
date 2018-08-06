namespace Zu1779.AGE.Env.CardGameEnv
{
    using System;

    using Zu1779.AGE.Contract;

    [Serializable]
    public class CardGameEnvironment : MarshalByRefObject, IEnvironment
    {
        public CardGameEnvironment(string code)
        {
            this.code = code;
        }
        private string code { get; }

        #region IEnvironment
        public void AttachAgent(AgentTypeEnum agentType, string agentCode, IAgent agent)
        {
            throw new NotImplementedException();
        }

        public (bool isValid, string unvalidCause) CheckAgentValidity(AgentTypeEnum agentType, IAgent agent)
        {
            throw new NotImplementedException();
        }

        public void CheckStatus()
        {
            throw new NotImplementedException();
        }

        public void SetUp()
        {
            throw new NotImplementedException();
        }

        public void TearDown()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
        #endregion
    }
}
