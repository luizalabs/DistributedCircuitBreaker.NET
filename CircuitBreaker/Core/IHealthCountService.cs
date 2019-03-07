using System;

namespace CircuitBreaker.Core
{
    public interface IHealthCountService
    {
        HealthCount GetCurrentHealthCount();
        CircuitState GetState();
        void IncrementFailure();
        void IncrementSuccess();
        void OpenCircuit();
    }
}