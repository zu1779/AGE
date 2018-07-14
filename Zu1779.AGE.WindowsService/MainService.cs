namespace Zu1779.AGE.WindowsService
{
    using System.ServiceProcess;

    using Common.Logging;
    using Newtonsoft.Json;

    partial class MainService : ServiceBase
    {
        private static readonly ILog log = LogManager.GetLogger(nameof(MainService));

        public MainService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            log.Info($"{nameof(MainService)}.{nameof(OnStart)} - {nameof(args)}={JsonConvert.SerializeObject(args)}");
        }

        protected override void OnPause()
        {
            log.Info($"{nameof(MainService)}.{nameof(OnPause)}");
        }

        protected override void OnContinue()
        {
            log.Info($"{nameof(MainService)}.{nameof(OnContinue)}");
        }

        protected override void OnCustomCommand(int command)
        {
            log.Info($"{nameof(MainService)}.{nameof(OnCustomCommand)} - {nameof(command)}={command}");
        }

        protected override void OnStop()
        {
            log.Info($"{nameof(MainService)}.{nameof(OnStop)}");
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            log.Info($"{nameof(MainService)}.{nameof(OnSessionChange)} - {nameof(changeDescription)}={JsonConvert.SerializeObject(changeDescription)}");
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            log.Info($"{nameof(MainService)}.{nameof(OnPowerEvent)} - {nameof(powerStatus)}={powerStatus}");
            return true;
        }

        protected override void OnShutdown()
        {
            log.Info($"{nameof(MainService)}.{nameof(OnShutdown)}");
        }
    }
}
