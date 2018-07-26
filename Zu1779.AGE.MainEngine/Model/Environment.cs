﻿namespace Zu1779.AGE.MainEngine.Model
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Policy;

    using Common.Logging;

    using Zu1779.AGE.Contract;

    [Serializable]
    public class Environment : IDisposable
    {
        public Environment(string environmentCode, ILog log)
        {
            this.environmentCode = environmentCode ?? throw new ArgumentNullException(nameof(environmentCode));
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }
        private readonly string environmentCode;
        private readonly ILog log;
        private AppDomain appDomain;
        private IEnvironment environment;
        private readonly ConcurrentDictionary<string, Agent> agents = new ConcurrentDictionary<string, Agent>();

        public void Dispose()
        {
            if (appDomain != null)
            {
                AppDomain.Unload(appDomain);
                appDomain = null;
            }
        }

        public string Code { get { return environmentCode; } }

        private AppDomain createAppDomain(string applicationBase)
        {
            var securityInfo = new Evidence();
            var appDomainSetup = new AppDomainSetup { ApplicationBase = applicationBase };
            var permissionSet = new PermissionSet(PermissionState.Unrestricted);
            var appDomain = AppDomain.CreateDomain(environmentCode, securityInfo, appDomainSetup, permissionSet);
            return appDomain;
        }

        public void PrepareEnvironment(string environmentPath)
        {
            if (!Directory.Exists(environmentPath)) throw new ApplicationException($"Path {environmentPath} not found");
            string envTargetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EnvironmentRepository", environmentCode);
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
    }
}
