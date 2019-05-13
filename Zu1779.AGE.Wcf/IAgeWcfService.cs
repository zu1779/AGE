namespace Zu1779.AGE.Wcf
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;

    [ServiceContract]
    public interface IAgeWcfService
    {
        [OperationContract]
        List<string> ExecuteCommand(string inputCommand);

        [OperationContract]
        string GetVersion();
    }
}