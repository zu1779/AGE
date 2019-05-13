namespace Zu1779.AGE.Wcf
{
    using System;
    using System.Reflection;
    using System.ServiceModel;

    using Newtonsoft.Json;

    using Zu1779.AGE.MainEngine;

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class AgeWcfService : IAgeWcfService
    {
        public AgeWcfService(EngineManager engineManager)
        {
            this.engineManager = engineManager ?? throw new ArgumentNullException(nameof(engineManager));
        }
        private readonly EngineManager engineManager;

        public string GetVersion()
        {
            return Assembly.GetEntryAssembly().GetName().ToString();
        }

        public object ExecuteCommand(string inputCommand)
        {
            throw new NotImplementedException();
        }
    }
}
