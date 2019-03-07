using CircuitBreaker.Repository;
using System;

namespace CircuitBreaker.Core
{
    public class HealthCountService : IHealthCountService
    {
        ICircuitBreakRepository _circuitBreakRepository;
        TimeSpan _windowDuration;
        private TimeSpan _durationOfBreak;
        const string SuccessCountKeySuffix = "-success";
        const string FailureCountKeySuffix = "-failure";
        const string StateKeySuffix = "-state";
        private readonly string _successCountKey;
        private readonly string _failureCountKey;
        private readonly string _stateKey;

        public HealthCountService(string key, IRepository repository, TimeSpan windowDuration, TimeSpan durationOfBreak) 
        {
            _circuitBreakRepository = new CircuitBreakRepository(repository);
            _windowDuration = windowDuration;
            _durationOfBreak = durationOfBreak;
            _successCountKey = key + SuccessCountKeySuffix;
            _failureCountKey = key + FailureCountKeySuffix;
            _stateKey = key + StateKeySuffix;
        }

        public HealthCount GetCurrentHealthCount()
        {
            return new HealthCount()
            {
                Successes = _circuitBreakRepository.GetInt32(_successCountKey),  
                Failures = _circuitBreakRepository.GetInt32(_failureCountKey) 
            };        
        }

        public void IncrementSuccess()
        {
            var keyExists = _circuitBreakRepository.KeyExists(_successCountKey);
            if (!keyExists)
                _circuitBreakRepository.SetInt32(_successCountKey, 0, _windowDuration);

            _circuitBreakRepository.Increment(_successCountKey);
        }

        public void IncrementFailure()
        {
            var keyExists = _circuitBreakRepository.KeyExists(_failureCountKey);
            if (!keyExists)
                _circuitBreakRepository.SetInt32(_failureCountKey, 0, _windowDuration);

            _circuitBreakRepository.Increment(_failureCountKey);
        }

        /// <summary>
        /// 
        /// </summary>
        public void OpenCircuit()
        {
            _circuitBreakRepository.SetInt32(_stateKey, (int)CircuitState.Open, _durationOfBreak);
        }

        public CircuitState GetState()
        {
            var keyExists = _circuitBreakRepository.KeyExists(_stateKey);

            if (keyExists)
                return CircuitState.Open;

            return CircuitState.Closed;
        }
    }
}
