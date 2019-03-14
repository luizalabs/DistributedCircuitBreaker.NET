using DistributedCircuitBreaker.DependencyInjection;
using DistributedCircuitBreaker.Repository;
using Microsoft.Extensions.Options;
using System;

namespace DistributedCircuitBreaker.Core
{
    internal class HealthCountService : IHealthCountService
    {
        ICircuitBreakRepository _circuitBreakRepository;
        TimeSpan _windowDuration;
        private TimeSpan _durationOfBreak;

        public HealthCountService(IOptions<CircuitBreakerFactoryOptions> options, ICircuitBreakRepository circuitBreakRepository)
        {
            _circuitBreakRepository = circuitBreakRepository;
            _windowDuration = options.Value.WindowDuration;
            _durationOfBreak = options.Value.DurationOfBreak;
        }

        public HealthCount GetCurrentHealthCount(CircuitBreakerKeys keys)
        {
            return new HealthCount()
            {
                Successes = _circuitBreakRepository.GetInt32(keys.SuccessCountKey),  
                Failures = _circuitBreakRepository.GetInt32(keys.FailureCountKey) 
            };        
        }

        public void IncrementSuccess(CircuitBreakerKeys keys)
        {
            var keyExists = _circuitBreakRepository.KeyExists(keys.SuccessCountKey);
            if (!keyExists)
                _circuitBreakRepository.SetInt32(keys.SuccessCountKey, 0, _windowDuration);

            _circuitBreakRepository.Increment(keys.SuccessCountKey);
        }

        public void IncrementFailure(CircuitBreakerKeys keys)
        {
            var keyExists = _circuitBreakRepository.KeyExists(keys.FailureCountKey);
            if (!keyExists)
                _circuitBreakRepository.SetInt32(keys.FailureCountKey, 0, _windowDuration);

            _circuitBreakRepository.Increment(keys.FailureCountKey);
        }

        public void OpenCircuit(CircuitBreakerKeys keys)
        {
            _circuitBreakRepository.SetInt32(keys.StateKey, (int)CircuitState.Open, _durationOfBreak);
            _circuitBreakRepository.Remove(keys.FailureCountKey);
            _circuitBreakRepository.Remove(keys.SuccessCountKey);
        }

        public CircuitState GetState(CircuitBreakerKeys keys)
        {
            var keyExists = _circuitBreakRepository.KeyExists(keys.StateKey);

            if (keyExists)
                return CircuitState.Open;

            return CircuitState.Closed;
        }
    }
}
