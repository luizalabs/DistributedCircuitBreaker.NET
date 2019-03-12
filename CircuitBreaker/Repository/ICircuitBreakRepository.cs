namespace DistributedCircuitBreaker.Repository
{
    public interface ICircuitBreakRepository
    {
        int GetInt32(string key);
        void Increment(string key);
        bool KeyExists(string key);
        void SetInt32(string key, int value, System.TimeSpan timeSpan);
        void Remove(string key);
    }
}