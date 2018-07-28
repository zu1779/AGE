namespace Zu1779.AGE.Contract
{
    using System;

    [Serializable]
    public class SetUpRequest
    {
        public IEnvironmentCommunication Environment { get; set; }
    }
}
