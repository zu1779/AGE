namespace Zu1779.AGE.Contract
{
    public interface IAgent
    {
        void CheckStatus();
        void SetUp();
        void TearDown();
        void Start();
        void Stop();
    }
}