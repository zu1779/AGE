namespace Zu1779.AGE.Contract
{
    using System.Collections.Generic;

    public interface IEnvironment
    {
        void Start();
        void Stop();
        (bool isValid, string unvalidCause) CheckAgentValidity(AgentTypeEnum agentType, IAgent agent);
        void AttachAgent(AgentTypeEnum agentType, string agentCode, IAgent agent);
    }
}