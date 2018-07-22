namespace Zu1779.AGE.MainEngine.Model
{
    using System;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Policy;

    [Serializable]
    public class Agent
    {
        public Agent(string friendlyName)
        {
            this.friendlyName = friendlyName ?? throw new ArgumentNullException(nameof(friendlyName));
        }
        private readonly string friendlyName;

        public void Test()
        {
            var securityInfo = new Evidence();
            var appDomainSetup = new AppDomainSetup();
            var permissionSet = new PermissionSet(PermissionState.None);
            permissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            var appDomain = AppDomain.CreateDomain(friendlyName, securityInfo, appDomainSetup, permissionSet);
            
        }
    }
}
