namespace Zu1779.AGE.WindowsService
{
    using System.ServiceProcess;

    partial class MainService : ServiceBase
    {
        public MainService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            
        }

        protected override void OnPause()
        {
            
        }

        protected override void OnContinue()
        {
            
        }

        protected override void OnCustomCommand(int command)
        {
            
        }

        protected override void OnStop()
        {

        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {

            return true;
        }

        protected override void OnShutdown()
        {
            
        }
    }
}
