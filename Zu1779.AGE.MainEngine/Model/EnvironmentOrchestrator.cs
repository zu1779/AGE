namespace Zu1779.AGE.MainEngine.Model
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Policy;
    using System.Threading;
    using System.Threading.Tasks;

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
        private Thread environmentThread;

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

            //var adt = appDomain.CreateInstanceAndUnwrap(typeof(AppDomainTunnel).Assembly.FullName, typeof(AppDomainTunnel).FullName);

            var ad = AppDomain.CreateDomain("ProbingDomain");
            var tunnel = (AppDomainTunnel)ad.CreateInstanceAndUnwrap(typeof(AppDomainTunnel).Assembly.FullName, typeof(AppDomainTunnel).FullName);
            var (assemblyName, typeName) = tunnel.ProbeAssemblies(envTargetPath);
            AppDomain.Unload(ad);

            appDomain = createAppDomain(envTargetPath);
            environment = appDomain.CreateInstanceAndUnwrap(assemblyName, typeName, true, BindingFlags.Default, null, new[] { Code }, null, null) as IEnvironment;
        }

        private class AppDomainTunnel : MarshalByRefObject
        {
            public (string assemblyName, string typeName) ProbeAssemblies(string path)
            {
                var probedTypes = new List<Type>();
                var files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    try
                    {
                        var asm = Assembly.LoadFile(file);
                        var envs = asm.GetTypes().Where(c => typeof(IEnvironment).IsAssignableFrom(c));
                        if (envs.Any()) probedTypes.AddRange(envs);
                    }
                    catch (Exception) { }
                }
                if (!probedTypes.Any()) throw new ApplicationException("No environment found");
                if (probedTypes.Count > 1) throw new ApplicationException("More than 1 environment found");
                return (probedTypes[0].Assembly.FullName, probedTypes[0].FullName);
            }
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

        public void CheckStatus()
        {
            environment.CheckStatus();
            foreach (var agent in agents)
            {
                var agentResponse = agent.Value.CheckStatus();
                if (agentResponse.HealthState) log.Info(agentResponse);
                else log.Error(agentResponse);
            }
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
            var environmentThread = new Thread(environment.Start);
            environmentThread.Start();
        }

        public void Pause()
        {
            System.Diagnostics.Debugger.Break();
            var status = environmentThread.ThreadState;
            environmentThread.Suspend();
            status = environmentThread.ThreadState;
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
