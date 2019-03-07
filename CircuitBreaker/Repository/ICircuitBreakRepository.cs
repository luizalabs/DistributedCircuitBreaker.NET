namespace CircuitBreaker.Repository
{
    public interface ICircuitBreakRepository
    {
        int GetInt32(string key);
        long GetInt64(string key);
        void Increment(string key);
        bool KeyExists(string key);
        void SetInt32(string key, int value, System.TimeSpan timeSpan);
        void SetInt64(string key, long value);
    }
}