namespace Zu1779.AGE.Contract
{
    using System.Collections.Generic;

    public interface IEnvironment
    {
        (bool isValid, string unvalidCause) CheckAgentValidity(AgentTypeEnum agentType, IAgent agent);
        void AttachAgent(AgentTypeEnum agentType, string agentCode, IAgent agent);

        void CheckStatus();
        void SetUp();
        void TearDown();
        void Start();
        void Pause();
        void Continue();
        void Command(int command);
        void Stop();
    }
}