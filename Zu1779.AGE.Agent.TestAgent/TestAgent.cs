namespace Zu1779.AGE.Agent.TestAgent
{
    using System;

    using Zu1779.AGE.Contract;

    [Serializable]
    public class TestAgent : MarshalByRefObject, IAgent
    {
        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
