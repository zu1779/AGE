namespace Zu1779.AGE.Core.Contract
{
    using System;

    [Serializable]
    public class SetUpRequest
    {
        public IEnvironmentCommunication Environment { get; set; }
    }
}
