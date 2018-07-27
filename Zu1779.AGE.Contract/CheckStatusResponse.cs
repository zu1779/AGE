namespace Zu1779.AGE.Contract
{
    using System;

    using Newtonsoft.Json;

    [Serializable]
    public class CheckStatusResponse
    {
        public bool HealthState { get; set; }
        public Exception Exception { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
