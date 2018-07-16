namespace Zu1779.AGE.Wcf
{
    using System;
    using System.Reflection;
    using System.ServiceModel;

    using Zu1779.AGE.MainEngine;

    [ServiceContract]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class AgeWcfService
    {
        public AgeWcfService(EngineManager engineManager)
        {
            this.engineManager = engineManager ?? throw new ArgumentNullException(nameof(engineManager));
        }
        private readonly EngineManager engineManager;

        [OperationContract]
        public string GetVersion()
        {
            return Assembly.GetEntryAssembly().GetName().ToString();
        }


    }
}
