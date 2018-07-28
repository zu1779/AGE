namespace Zu1779.AGE.Contract
{
    public interface IAgent
    {
        string Code { get; }
        CheckStatusResponse CheckStatus();
        void SetUp(SetUpRequest request);
        void TearDown();
        void Start();
        void Stop();
    }
}