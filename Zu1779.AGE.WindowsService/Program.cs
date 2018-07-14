namespace Zu1779.AGE.WindowsService
{
    using System.ServiceProcess;

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new MainService(),
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
