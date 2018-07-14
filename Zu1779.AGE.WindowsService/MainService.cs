namespace Zu1779.AGE.WindowsService
{
    using System;
    using System.Runtime.InteropServices;
    using System.ServiceProcess;

    using Common.Logging;
    using Newtonsoft.Json;

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

            InitializeComponent();
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
        private const int SERVICE_WAIT = 60000;

        private void setServiceStatus(ServiceState serviceState)
        {
            ServiceStatus serviceStatus = new ServiceStatus { dwCurrentState = serviceState, dwWaitHint = SERVICE_WAIT };
            SetServiceStatus(ServiceHandle, ref serviceStatus);
        }

        protected override void OnStart(string[] args)
        {
            setServiceStatus(ServiceState.SERVICE_START_PENDING);

            log.Info(c => c($"{nameof(MainService)}.{nameof(OnStart)} - {nameof(args)}={JsonConvert.SerializeObject(args)}"));

            setServiceStatus(ServiceState.SERVICE_RUNNING);
        }

        protected override void OnPause()
        {
            setServiceStatus(ServiceState.SERVICE_PAUSE_PENDING);

            log.Info(c => c($"{nameof(MainService)}.{nameof(OnPause)}"));

            setServiceStatus(ServiceState.SERVICE_PAUSED);
        }

        protected override void OnContinue()
        {
            setServiceStatus(ServiceState.SERVICE_CONTINUE_PENDING);

            log.Info(c => c($"{nameof(MainService)}.{nameof(OnContinue)}"));

            setServiceStatus(ServiceState.SERVICE_RUNNING);
        }

        protected override void OnCustomCommand(int command)
        {
            log.Info(c => c($"{nameof(MainService)}.{nameof(OnCustomCommand)} - {nameof(command)}={command}"));
        }

        protected override void OnStop()
        {
            setServiceStatus(ServiceState.SERVICE_STOP_PENDING);

            log.Info(c => c($"{nameof(MainService)}.{nameof(OnStop)}"));

            setServiceStatus(ServiceState.SERVICE_STOPPED);
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            log.Info(c => c($"{nameof(MainService)}.{nameof(OnSessionChange)} - {nameof(changeDescription)}={JsonConvert.SerializeObject(changeDescription)}"));
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
    }
}
