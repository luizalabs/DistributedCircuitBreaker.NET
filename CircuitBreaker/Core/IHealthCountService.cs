namespace CircuitBreaker.Core
{
    public interface IHealthCountService
    {
        HealthCount GetCurrentHealthCount(CircuitBreakerKeys keys);
        CircuitState GetState(CircuitBreakerKeys keys);
        void IncrementFailure(CircuitBreakerKeys keys);
        void IncrementSuccess(CircuitBreakerKeys keys);
        void OpenCircuit(CircuitBreakerKeys keys);
    }
}