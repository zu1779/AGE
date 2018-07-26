namespace Zu1779.AGE.MainEngine.Model
{
    using System;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Policy;

    using Common.Logging;

    [Serializable]
    public class Agent : IDisposable
    {
        public Agent(string agentCode, ILog log)
        {
            Code = agentCode ?? throw new ArgumentNullException(nameof(agentCode));
            appDomain = createAppDomain();
        }
        private AppDomain appDomain;

        private AppDomain createAppDomain()
        {
            var securityInfo = new Evidence();
            var appDomainSetup = new AppDomainSetup();
            var permissionSet = new PermissionSet(PermissionState.None);
            permissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            var appDomain = AppDomain.CreateDomain(Code, securityInfo, appDomainSetup, permissionSet);
            return appDomain;
        }

        public void Dispose()
        {
            AppDomain.Unload(appDomain);
            appDomain = null;
        }

        public string Code { get; }
    }
}
