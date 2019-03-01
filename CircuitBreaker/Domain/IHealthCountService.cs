namespace CircuitBreaker.Domain
{
    public interface IHealthCountService
    {
        void ClearCounters(string key);
        HealthCount GetCurrentHealthCount(string key);
        long GetBreakedAt(string key);
        CircuitState GetState(string key);
        void IncrementFailure(string key);
        void IncrementSuccess(string key);
        void SetBreakedAt(string key, long breakedAt);
        void RefreshStartedDate(string key);
        void SetState(string key, CircuitState state);
        HealthCount GenerateNewHealthCounter(string key);
    }
}