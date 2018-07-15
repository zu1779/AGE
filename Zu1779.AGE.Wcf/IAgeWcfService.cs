namespace Zu1779.AGE.Wcf
{
    using System.ServiceModel;

    [ServiceContract]
    public interface IAgeWcfService
    {
        [OperationContract]
        string GetVersion();
    }
}
