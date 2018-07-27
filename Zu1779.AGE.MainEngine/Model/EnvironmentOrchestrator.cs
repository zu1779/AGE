namespace Zu1779.AGE.MainEngine.Model
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Policy;

    using Common.Logging;

    using Zu1779.AGE.Contract;
    using m = Zu1779.AGE.MainEngine.Model;

    [Serializable]
    public class EnvironmentOrchestrator : IDisposable
    {
        public EnvironmentOrchestrator(string environmentCode, ILog log)
        {
            Code = environmentCode ?? throw new ArgumentNullException(nameof(environmentCode));
            this.log = log ?? throw new ArgumentNullException(nameof(log));
            InstanceTime = DateTimeOffset.Now;
        }
        private readonly ILog log;
        private AppDomain appDomain;
        private readonly ConcurrentDictionary<string, AgentOrchestrator> agents = new ConcurrentDictionary<string, AgentOrchestrator>();

        public void Dispose()
        {
            if (appDomain != null)
            {
                AppDomain.Unload(appDomain);
                appDomain = null;
            }
        }

        private AppDomain createAppDomain(string applicationBase)
        {
            var securityInfo = new Evidence();
            var appDomainSetup = new AppDomainSetup { ApplicationBase = applicationBase };
            var permissionSet = new PermissionSet(PermissionState.Unrestricted);
            var appDomain = AppDomain.CreateDomain(Code, securityInfo, appDomainSetup, permissionSet);
            return appDomain;
        }

        public string Code { get; }
        public DateTimeOffset InstanceTime { get; }
        private IEnvironment environment { get; set; }

        /// <summary>
        /// Prepare an AppDomain for the executin of the Environment.
        /// </summary>
        /// <param name="environmentPath">Path of the folder that contains the Environments dlls.</param>
        public void Prepare(string environmentPath)
        {
            if (!Directory.Exists(environmentPath)) throw new ApplicationException($"Path {environmentPath} not found");
            string envTargetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EnvironmentRepository", Code);
            if (!Directory.Exists(envTargetPath))
            {
                Directory.CreateDirectory(envTargetPath);
            }
            else
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(envTargetPath);
                foreach (var file in directoryInfo.EnumerateFiles()) file.Delete();
                foreach (var dir in directoryInfo.EnumerateDirectories()) dir.Delete(true);
            }
            // Copy environment assembly and dependencies
            foreach (var dir in Directory.GetDirectories(environmentPath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dir.Replace(environmentPath, envTargetPath));
            foreach (var file in Directory.GetFiles(environmentPath, "*.*", SearchOption.AllDirectories))
                File.Copy(file, file.Replace(environmentPath, envTargetPath));

            appDomain = createAppDomain(envTargetPath);
            environment = appDomain.CreateInstanceAndUnwrap("Zu1779.AGE.Environment.TestEnvironment", "Zu1779.AGE.Environment.TestEnvironment.TestEnvironment") as IEnvironment;
        }

        public void AddAgent(string agentCode, string agentPath)
        {
            var agent = new AgentOrchestrator(agentCode, this, log);
            agent.Prepare(agentPath);
            var added = agents.TryAdd(agentCode, agent);
            if (!added) throw new ApplicationException($"Agent {agentCode} already exists");
            var (valid, unvalidCause) = environment.CheckAgentValidity(AgentTypeEnum.User, agent.Agent);
            if (!valid) throw new ApplicationException($"Agent {agentCode} failed validation with environment {Code}. Cause: {unvalidCause}");
            environment.AttachAgent(AgentTypeEnum.User, agentCode, agent.Agent);
        }

        public List<AgentOrchestrator> GetAgents()
        {
            return agents.Values.ToList();
        }

        public void SetUp()
        {
            environment.SetUp();
        }

        public void TearDown()
        {
            environment.TearDown();
        }

        public void Start()
        {
            environment.Start();
        }

        public void Pause()
        {
            environment.Pause();
        }

        public void Continue()
        {
            environment.Continue();
        }

        public void Command(int command)
        {
            environment.Command(command);
        }

        public void Stop()
        {
            environment.Stop();
        }
    }
}
