namespace Zu1779.AGE.Wcf
{
    using System.Reflection;

    public class AgeWcfService : IAgeWcfService
    {
        public string GetVersion()
        {
            return Assembly.GetEntryAssembly().GetName().ToString();
        }
    }
}
