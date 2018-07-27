namespace Zu1779.AGE.Contract
{
    public interface IAgent
    {
        string Code { get; }
        CheckStatusResponse CheckStatus();
        void SetUp();
        void TearDown();
        void Start();
        void Stop();
    }
}