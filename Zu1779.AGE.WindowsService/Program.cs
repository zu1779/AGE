namespace Zu1779.AGE.WindowsService
{
    using System.ServiceProcess;

    static class Program
    {
        static void Main()
        {
            ServiceBase.Run(new MainService());
        }
    }
}
