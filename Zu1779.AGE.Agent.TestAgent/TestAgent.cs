namespace Zu1779.AGE.Agent.TestAgent
{
    using System;

    using Zu1779.AGE.Contract;
    using Zu1779.AGE.Environment.TestEnvironment.Contract;

    [Serializable]
    public class TestAgent : MarshalByRefObject, IAgent, IAgentCommunication
    {
        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void SendMessage(string message)
        {
            throw new ApplicationException($"Unknown message: {message}");
        }
    }
}
