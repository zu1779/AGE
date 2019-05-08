namespace Zu1779.AGE.Core.Contract
{
    public interface IAgent
    {
        string Code { get; }
        string Token { get; }
        CheckStatusResponse CheckStatus();
        void SetUp(SetUpRequest request);
        void TearDown();
        void Start();
        void Stop();
    }
}