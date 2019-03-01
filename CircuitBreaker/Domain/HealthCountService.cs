using CircuitBreaker.Repository;
using System;

namespace CircuitBreaker.Domain
{
    public class HealthCountService : IHealthCountService
    {
        ICircuitBreakRepository _circuitBreakRepository;
        const string SuccessCountKeySuffix = "-success";
        const string FailureCountKeySuffix = "-failure";
        const string StartedAtCountKeySuffix = "-startedAt";
        const string StateKeySuffix = "-state";
        const string BreakedAtCountKeySuffix = "-breakedAt";

        public HealthCountService(IRepository repository) 
        {
            _circuitBreakRepository = new CircuitBreakRepository(repository);
        }

        public HealthCount GetCurrentHealthCount(string key)
        {
            if (_circuitBreakRepository.KeyExists(key + StartedAtCountKeySuffix) == false)
                return GenerateNewHealthCounter(key);

            return new HealthCount()
            {
                Successes = _circuitBreakRepository.GetInt32(key + SuccessCountKeySuffix),  
                Failures = _circuitBreakRepository.GetInt32(key + FailureCountKeySuffix), 
                StartedAt = _circuitBreakRepository.GetInt64(key + StartedAtCountKeySuffix)
            };        
        }

        public HealthCount GenerateNewHealthCounter(string key)
        {
            ClearCounters(key);
            RefreshStartedDate(key);
            SetState(key, CircuitState.Closed);
            return new HealthCount() { Failures = 0, Successes = 0, StartedAt = DateTime.UtcNow.Ticks };
        }

        public void ClearCounters(string key)
        {
            _circuitBreakRepository.SetInt32(key + SuccessCountKeySuffix, 0);
            _circuitBreakRepository.SetInt32(key + FailureCountKeySuffix, 0);
        }

        public void IncrementSuccess(string key)
        {
            _circuitBreakRepository.Increment(key + SuccessCountKeySuffix);
        }

        public void IncrementFailure(string key)
        {
            _circuitBreakRepository.Increment(key + FailureCountKeySuffix);
        }

        public void RefreshStartedDate(string key)
        {
            _circuitBreakRepository.SetInt64(key + StartedAtCountKeySuffix, DateTime.UtcNow.Ticks);
        }

        public void SetState(string key, CircuitState state)
        {
            _circuitBreakRepository.SetInt32(key + StateKeySuffix, (int)state);
        }

        public CircuitState GetState(string key)
        {
            return (CircuitState)_circuitBreakRepository.GetInt32(key + StateKeySuffix);
        }

        public void SetBreakedAt(string key, long breakedAt)
        {
            _circuitBreakRepository.SetInt64(key + BreakedAtCountKeySuffix, (long)breakedAt);
        }

        public long GetBreakedAt(string key)
        {
            return _circuitBreakRepository.GetInt64(key + BreakedAtCountKeySuffix);
        }
    }
}
