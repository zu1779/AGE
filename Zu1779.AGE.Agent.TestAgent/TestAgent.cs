namespace Zu1779.AGE.Agent.TestAgent
{
    using System;
    using System.Reflection;

    using cnt = Zu1779.AGE.Contract;
    using envcnt = Zu1779.AGE.Environment.TestEnvironment.Contract;

    [Serializable]
    public class TestAgent : MarshalByRefObject, cnt.IAgent, cnt.IAgentCommunication, envcnt.IAgentTestComm
    {
        public TestAgent(string agentCode)
        {
            Code = agentCode;
        }
        private cnt.IEnvironmentCommunication environment;

        public string Code { get; }
        public string Token { get; set; }

        public cnt.CheckStatusResponse CheckStatus()
        {
            return new cnt.CheckStatusResponse { HealthState = true };
        }

        public void SetUp(cnt.SetUpRequest request)
        {
            environment = request.Environment;
        }

        public void TearDown()
        {
            
        }

        public void Start()
        {
            
        }

        public void Stop()
        {
            
        }

        public void SendMessage(string message)
        {
            throw new ApplicationException($"Unknown message: {message}");
        }
    }
}
