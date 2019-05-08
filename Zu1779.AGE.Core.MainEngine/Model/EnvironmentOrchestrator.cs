namespace Zu1779.AGE.Core.MainEngine.Model
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using log = NLog;
    using Newtonsoft.Json;

    using Contract;
    using m = MainEngine.Model;

    [Serializable]
    public class EnvironmentOrchestrator : IDisposable
    {
        public EnvironmentOrchestrator(string environmentCode, log.ILogger logger)
        {
            Code = environmentCode ?? throw new ArgumentNullException(nameof(environmentCode));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            InstanceTime = DateTimeOffset.Now;
        }
        private readonly log.ILogger logger;
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

        private AppDomain CreateAppDomain(string applicationBase)
        {
            //var securityInfo = new Evidence();
            //var appDomainSetup = new AppDomainSetup { ApplicationBase = applicationBase };
            //var permissionSet = new PermissionSet(PermissionState.Unrestricted);
            //var appDomain = AppDomain.CreateDomain(Code, securityInfo, appDomainSetup, permissionSet);
            var appDomain = AppDomain.CreateDomain(Code);
            return appDomain;
        }

        public string Code { get; }
        public DateTimeOffset InstanceTime { get; }
        public IEnvironment Environment { get; set; }

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

            var probeAppDomain = CreateAppDomain(envTargetPath);
            //var probeTunnel = (AppDomainTunnel)probeAppDomain.CreateInstanceAndUnwrap(typeof(AppDomainTunnel).Assembly.FullName, typeof(AppDomainTunnel).FullName);
            //var (assemblyName, typeName) = probeTunnel.ProbeAssemblies(envTargetPath, typeof(IEnvironment));
            AppDomain.Unload(probeAppDomain);

            appDomain = CreateAppDomain(envTargetPath);
            //var tunnel = (AppDomainTunnel)appDomain.CreateInstanceAndUnwrap(typeof(AppDomainTunnel).Assembly.FullName, typeof(AppDomainTunnel).FullName);
            //Environment = (IEnvironment)appDomain.CreateInstanceAndUnwrap(assemblyName, typeName, true, BindingFlags.Default, null, new[] { Code }, null, null);
        }

        public void AddAgent(string agentCode, string agentPath)
        {
            Utility utility = new Utility();
            var agentToken = utility.GenerateToken();
            var agent = new AgentOrchestrator(agentCode, agentToken, this, logger);
            agent.Prepare(agentPath);
            var added = agents.TryAdd(agentCode, agent);
            if (!added) throw new ApplicationException($"Agent {agentCode} already exists");
            var (valid, unvalidCause) = Environment.CheckAgentValidity(AgentTypeEnum.User, agent.Agent);
            if (!valid) throw new ApplicationException($"Agent {agentCode} failed validation with environment {Code}. Cause: {unvalidCause}");
            Environment.AttachAgent(AgentTypeEnum.User, agentCode, agentToken, agent.Agent);
        }

        public List<AgentOrchestrator> GetAgents()
        {
            return agents.Values.ToList();
        }

        public void CheckStatus()
        {
            Environment.CheckStatus();
            foreach (var agent in agents)
            {
                var agentResponse = agent.Value.CheckStatus();
                if (agentResponse.HealthState) logger.Info(agentResponse);
                else logger.Error(agentResponse);
            }
            logger.Info($"{nameof(appDomain.MonitoringTotalProcessorTime)}={appDomain.MonitoringTotalProcessorTime}");
            logger.Info($"{nameof(environmentThread.ThreadState)}={environmentThread?.ThreadState}");
        }

        public void SetUp()
        {
            Environment.SetUp();
            foreach (var agent in agents)
            {
                agent.Value.SetUp();
            }
        }

        public void TearDown()
        {
            Environment.TearDown();
        }

        public void Start()
        {
            environmentThread = new Thread(Environment.Start);
            environmentThread.Start();
            logger.Info($"{nameof(environmentThread.ThreadState)}={environmentThread.ThreadState}");
        }

        public void Pause()
        {
            var status = environmentThread.ThreadState;
#pragma warning disable CS0618 // Type or member is obsolete
            environmentThread.Suspend();
#pragma warning restore CS0618 // Type or member is obsolete
            status = environmentThread.ThreadState;
        }

        public void Continue()
        {
            var status = environmentThread.ThreadState;
#pragma warning disable CS0618 // Type or member is obsolete
            environmentThread.Resume();
#pragma warning restore CS0618 // Type or member is obsolete
            status = environmentThread.ThreadState;
        }

        public void Command(int command)
        {
            Environment.Command(command);
        }

        public void Stop()
        {
            Environment.Stop();
        }
    }
}
