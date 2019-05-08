namespace Zu1779.AGE.Core.Contract
{
    using System.Collections.Generic;

    public interface IEnvironment
    {
        (bool isValid, string unvalidCause) CheckAgentValidity(AgentTypeEnum agentType, IAgent agent);
        void AttachAgent(AgentTypeEnum agentType, string agentCode, string agentToken, IAgent agent);

        void CheckStatus();
        void SetUp();
        void TearDown();
        void Start();
        void Command(int command);
        void Stop();
    }
}