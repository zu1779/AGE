namespace Zu1779.AGE.Contract
{
    using System.Collections.Generic;

    public interface IEnvironment
    {
        void Start();
        void Stop();
        List<string> GetAppConfig();
    }
}