namespace Zu1779.AGE.MainEngine.Model
{
    using System;
    using System.Collections.Concurrent;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Policy;

    [Serializable]
    public class Environment : IDisposable
    {
        public Environment(string friendlyName)
        {
            this.friendlyName = friendlyName ?? throw new ArgumentNullException(nameof(friendlyName));
            appDomain = createAppDomain();
        }
        private readonly string friendlyName;
        private AppDomain appDomain;

        private readonly ConcurrentDictionary<string, Agent> agents = new ConcurrentDictionary<string, Agent>();

        private AppDomain createAppDomain()
        {
            var securityInfo = new Evidence();
            var appDomainSetup = new AppDomainSetup { ApplicationBase = AppDomain.CurrentDomain.BaseDirectory };
            var permissionSet = new PermissionSet(PermissionState.Unrestricted);
            var appDomain = AppDomain.CreateDomain(friendlyName, securityInfo, appDomainSetup, permissionSet);
            return appDomain;
        }

        public void Dispose()
        {
            AppDomain.Unload(appDomain);
            appDomain = null;
        }
    }
}
