namespace Zu1779.AGE.Environment.TestEnvironment
{
    using System;

    using log4net;

    using Zu1779.AGE.Contract;

    [Serializable]
    public class TestEnvironment : MarshalByRefObject, IEnvironment
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TestEnvironment));

        static TestEnvironment()
        {
            log4net.Config.XmlConfigurator.Configure();
            log.Info("static TestEnvironment()");
        }

        public TestEnvironment()
        {
            log.Info("public TestEnvironment");
        }

        public void Start()
        {
            log.Info("Start()");
        }

        public void Stop()
        {
            log.Info("Stop()");
        }
    }
}
