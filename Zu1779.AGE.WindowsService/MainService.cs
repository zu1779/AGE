namespace Zu1779.AGE.WindowsService
{
    using System;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceProcess;

    using Common.Logging;
    using Newtonsoft.Json;

    using Zu1779.AGE.MainEngine;
    using Zu1779.AGE.Wcf;

    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    };

    partial class MainService : ServiceBase
    {
        private static readonly ILog log = LogManager.GetLogger(nameof(MainService));

        public MainService()
        {
            log4net.Config.XmlConfigurator.Configure();

            AppDomain.CurrentDomain.UnhandledException += (sender, evargs) =>
            {
                log.Fatal(c => c($"UnhandledException"), (Exception)evargs.ExceptionObject);
            };

            InitializeComponent();
        }
        private ServiceHost serviceHost;
        private EngineManager engineManager;

        #region ServiceStatus Extended
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
        private const int SERVICE_WAIT = 60000;

        private void setServiceStatus(ServiceState serviceState)
        {
            ServiceStatus serviceStatus = new ServiceStatus { dwCurrentState = serviceState, dwWaitHint = SERVICE_WAIT };
            SetServiceStatus(ServiceHandle, ref serviceStatus);
        }
        #endregion

        protected override void OnStart(string[] args)
        {
            setServiceStatus(ServiceState.SERVICE_START_PENDING);
            log.Info(c => c($"{nameof(MainService)}.{nameof(OnStart)} ({nameof(ServiceState.SERVICE_START_PENDING)}) - {nameof(args)}={JsonConvert.SerializeObject(args)}"));

            startEngine();
            startWcfInterface();

            setServiceStatus(ServiceState.SERVICE_RUNNING);
            log.Info(c => c($"{nameof(MainService)}.{nameof(OnStart)} ({nameof(ServiceState.SERVICE_RUNNING)}) - {nameof(args)}={JsonConvert.SerializeObject(args)}"));
        }

        protected override void OnPause()
        {
            setServiceStatus(ServiceState.SERVICE_PAUSE_PENDING);
            log.Info(c => c($"{nameof(MainService)}.{nameof(OnPause)} ({nameof(ServiceState.SERVICE_PAUSE_PENDING)})"));

            setServiceStatus(ServiceState.SERVICE_PAUSED);
            log.Info(c => c($"{nameof(MainService)}.{nameof(OnPause)} ({nameof(ServiceState.SERVICE_PAUSED)})"));
        }

        protected override void OnContinue()
        {
            setServiceStatus(ServiceState.SERVICE_CONTINUE_PENDING);
            log.Info(c => c($"{nameof(MainService)}.{nameof(OnContinue)} ({nameof(ServiceState.SERVICE_CONTINUE_PENDING)})"));

            setServiceStatus(ServiceState.SERVICE_RUNNING);
            log.Info(c => c($"{nameof(MainService)}.{nameof(OnContinue)} ({nameof(ServiceState.SERVICE_RUNNING)})"));
        }

        protected override void OnCustomCommand(int command)
        {
            log.Info(c => c($"{nameof(MainService)}.{nameof(OnCustomCommand)} - {nameof(command)}={command}"));
        }

        protected override void OnStop()
        {
            setServiceStatus(ServiceState.SERVICE_STOP_PENDING);
            log.Info(c => c($"{nameof(MainService)}.{nameof(OnStop)} ({nameof(ServiceState.SERVICE_STOP_PENDING)})"));

            stopWcfInterface();
            stopEngine();

            setServiceStatus(ServiceState.SERVICE_STOPPED);
            log.Info(c => c($"{nameof(MainService)}.{nameof(OnStop)} ({nameof(ServiceState.SERVICE_STOPPED)})"));
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            log.Info(c => c($"{nameof(MainService)}.{nameof(OnSessionChange)} - {nameof(changeDescription.Reason)}={changeDescription.Reason}, {nameof(changeDescription.SessionId)}={changeDescription.SessionId}"));
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            log.Info(c => c($"{nameof(MainService)}.{nameof(OnPowerEvent)} - {nameof(powerStatus)}={powerStatus}"));
            return true;
        }

        protected override void OnShutdown()
        {
            log.Info(c => c($"{nameof(MainService)}.{nameof(OnShutdown)}"));
        }

        protected void startEngine()
        {
            log.Info($"Begin {nameof(startEngine)}");

            engineManager = new EngineManager();

            log.Info($"End {nameof(startEngine)}");
        }

        private void stopEngine()
        {
            log.Info($"Begin {nameof(stopEngine)}");

            engineManager.Dispose();

            log.Info($"End {nameof(stopEngine)}");
        }

        private void startWcfInterface()
        {
            log.Info($"Starting WCF Interface");
            var ageWcfService = new AgeWcfService(engineManager);
            serviceHost = new ServiceHost(ageWcfService);
            serviceHost.Open();
            log.Info($"Started WCF Interface");
        }

        private void stopWcfInterface()
        {
            log.Info($"Stopping WCF Interface");
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
            log.Info($"Stopped WCF Interface");
        }
    }
}
