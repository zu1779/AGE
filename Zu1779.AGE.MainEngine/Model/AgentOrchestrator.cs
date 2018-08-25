namespace Zu1779.AGE.MainEngine.Model
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Policy;

    using Common.Logging;

    using Zu1779.AGE.Contract;

    [Serializable]
    public class AgentOrchestrator : IDisposable
    {
        public AgentOrchestrator(string agentCode, EnvironmentOrchestrator environment, ILog log)
        {
            Code = agentCode ?? throw new ArgumentNullException(nameof(agentCode));
            this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
            InstanceTime = DateTimeOffset.Now;
        }
        public void Dispose()
        {
            AppDomain.Unload(appDomain);
            appDomain = null;
        }
        private AppDomain appDomain;
        private readonly EnvironmentOrchestrator environment;

        private AppDomain createAppDomain(string applicationBase, bool forProbing = false)
        {
            var securityInfo = new Evidence();
            var appDomainSetup = new AppDomainSetup { ApplicationBase = applicationBase };
            PermissionSet permissionSet;
            if (forProbing)
            {
                permissionSet = new PermissionSet(PermissionState.Unrestricted);
            }
            else
            {
                permissionSet = new PermissionSet(PermissionState.None);
                permissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            }
            var appDomain = AppDomain.CreateDomain(Code, securityInfo, appDomainSetup, permissionSet);
            return appDomain;
        }

        public string Code { get; }
        public DateTimeOffset InstanceTime { get; }
        public IAgent Agent { get; private set; }

        public void Prepare(string agentPath)
        {
            if (!Directory.Exists(agentPath)) throw new ApplicationException($"Path {agentPath} not found");
            string agentTargetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EnvironmentRepository", environment.Code, Code);
            if (!Directory.Exists(agentTargetPath))
            {
                Directory.CreateDirectory(agentTargetPath);
            }
            else
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(agentTargetPath);
                foreach (var file in directoryInfo.EnumerateFiles()) file.Delete();
                foreach (var dir in directoryInfo.EnumerateDirectories()) dir.Delete(true);
            }
            // Copy environment assembly and dependencies
            foreach (var dir in Directory.GetDirectories(agentPath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dir.Replace(agentPath, agentTargetPath));
            foreach (var file in Directory.GetFiles(agentPath, "*.*", SearchOption.AllDirectories))
                File.Copy(file, file.Replace(agentPath, agentTargetPath));

            var ad = createAppDomain(agentTargetPath, true);
            var tunnel = (AppDomainTunnel)ad.CreateInstanceAndUnwrap(typeof(AppDomainTunnel).Assembly.FullName, typeof(AppDomainTunnel).FullName);
            var (assemblyName, typeName) = tunnel.ProbeAssemblies(agentTargetPath, typeof(IAgent));
            AppDomain.Unload(ad);

            appDomain = createAppDomain(agentTargetPath);
            Agent = appDomain.CreateInstanceAndUnwrap(assemblyName, typeName, true, BindingFlags.Default, null, new[] { Code }, null, null) as IAgent;
        }

        public CheckStatusResponse CheckStatus()
        {
            try
            {
                var response = Agent.CheckStatus();
                return response;
            }
            catch (Exception ex)
            {
                return new CheckStatusResponse { HealthState = false, Exception = ex };
            }
        }

        public void SetUp()
        {
            var setUpRequest = new SetUpRequest { Environment = (IEnvironmentCommunication)environment.Environment };
            Agent.SetUp(setUpRequest);
        }
    }
}
