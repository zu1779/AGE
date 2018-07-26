namespace Zu1779.AGE.MainEngine.Model
{
    using System;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Policy;

    [Serializable]
    public class Agent : IDisposable
    {
        public Agent(string friendlyName)
        {
            this.friendlyName = friendlyName ?? throw new ArgumentNullException(nameof(friendlyName));
            appDomain = createAppDomain();
        }
        private readonly string friendlyName;
        private AppDomain appDomain;

        private AppDomain createAppDomain()
        {
            var securityInfo = new Evidence();
            var appDomainSetup = new AppDomainSetup();
            var permissionSet = new PermissionSet(PermissionState.None);
            permissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
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
