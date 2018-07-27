namespace Zu1779.AGE.MainEngine.Model
{
    using System;
    using System.IO;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Policy;

    using Common.Logging;

    using Zu1779.AGE.Contract;

    [Serializable]
    public class Agent : IDisposable
    {
        public Agent(string agentCode, Environment environment, ILog log)
        {
            Code = agentCode ?? throw new ArgumentNullException(nameof(agentCode));
            this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
            InstanceTime = DateTimeOffset.Now;
        }
        private AppDomain appDomain;
        private readonly Environment environment;
        private IAgent agent;

        public void Dispose()
        {
            AppDomain.Unload(appDomain);
            appDomain = null;
        }

        private AppDomain createAppDomain(string applicationBase)
        {
            var securityInfo = new Evidence();
            var appDomainSetup = new AppDomainSetup { ApplicationBase = applicationBase };
            var permissionSet = new PermissionSet(PermissionState.None);
            permissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            var appDomain = AppDomain.CreateDomain(Code, securityInfo, appDomainSetup, permissionSet);
            return appDomain;
        }

        public string Code { get; }
        public DateTimeOffset InstanceTime { get; }

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

            appDomain = createAppDomain(agentTargetPath);
            agent = appDomain.CreateInstanceAndUnwrap("Zu1779.AGE.Agent.TestAgent", "Zu1779.AGE.Agent.TestAgent.TestAgent") as IAgent;
        }
    }
}
