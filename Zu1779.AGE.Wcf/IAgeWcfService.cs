namespace Zu1779.AGE.Wcf
{
    using System;
    using System.ServiceModel;

    [ServiceContract]
    public interface IAgeWcfService
    {
        [OperationContract]
        object ExecuteCommand(string inputCommand);

        [OperationContract]
        string GetVersion();
    }
}