using System;
using System.Collections.Generic;

namespace CircuitBreaker
{
    public class CircuitBreaker
    {
        private List<IRule> _rules;
        //private IRepository _repository;
        private HealthCount _healthCount ;
        private string _key;
        private TimeSpan _windowDuration;
        private TimeSpan _durationOfBreak;
        private CircuitState _state;
        private Exception _lastException;
        private HealthCountRepository _healthCountRepository;
        private CircuitBreakerStateRepository _circuitBreakerStateRepository;

        /// <summary>
        /// Initializes an instance of CircuitBreaker
        /// </summary>
        /// <param name="key">The Key that defines a unique unit to be controlled by circuit breaker</param>
        /// <param name="windowDuration">The amount of time considered to count failures</param>
        /// <param name="durationOfBreak">The amount of time the circuit should be open after changed to an open state</param>
        /// <param name="rules">Rules to be evaluated to decide if the state should be set to OPEN</param>
        /// <param name="repository">The repository that is used to store the CB information</param>
        public CircuitBreaker(string key, TimeSpan windowDuration, TimeSpan durationOfBreak, List<IRule> rules, IRepository repository)
        {
            //_repository = repository;
            _rules = rules;
            _key = key;
            _durationOfBreak = durationOfBreak;
            _windowDuration = windowDuration;
            _healthCountRepository = new HealthCountRepository(repository);
            _circuitBreakerStateRepository = new CircuitBreakerStateRepository(repository);
            _healthCount = _healthCountRepository.Get(key);                    
        }

        /// <summary>
        /// Returns True is the state is Open
        /// </summary>
        /// <returns>bool</returns>
        public bool IsOpen()
        {
            //return _state == CircuitState.Open;
            return _circuitBreakerStateRepository.GetState(_key).Equals(CircuitState.Open);

        }

        /// <summary>
        /// sets the state to Closed. 
        /// </summary>
        public void Reset()
        {
            _circuitBreakerStateRepository.SetState(_key, CircuitState.Closed);
            _state = CircuitState.Closed;
            _lastException = null;
        }

        /// <summary>
        /// Check All rules. If all rules returns true, open circuit
        /// </summary>
        /// <param name="ex"></param>
        private void ValidateRules(Exception ex)
        {
            bool shouldBreak = true;
            _rules.ForEach(r => 
            {
                if(r.ShouldOpenCircuitBreaker(_healthCount) == false)
                    shouldBreak = false;
            });

            if (!shouldBreak)
                return;

            _state = CircuitState.Open;
            _circuitBreakerStateRepository.SetState(_key, CircuitState.Open);
            throw new BrokenCircuitException("The circuit is now open and is not allowing calls.", _lastException);
        }

        private void IncrementFailure()
        {
            CreateNewWindowIfExpired();
            _healthCount.Failures++;
            _healthCountRepository.IncrementFailure(_key);
        }

        private void IncrementSuccess()
        {
            CreateNewWindowIfExpired();
            _healthCount.Successes++;
            _healthCountRepository.IncrementSuccess(_key);
        }

        private void CreateNewWindowIfExpired()
        {
            if(IsWindowsExpired())
            { 
                _healthCount = new HealthCount();
                ClearCounters();
                Reset();
            }
        }

        private void ClearCounters()
        {
            _healthCountRepository.ClearCounters(_key);
            _healthCountRepository.SetStartedAt(_key, DateTime.UtcNow.Ticks);
        }

        private bool IsWindowsExpired()
        {
            long now = DateTime.UtcNow.Ticks;    
            if (_healthCount == null || now - _healthCount.StartedAt >= _windowDuration.Ticks)
                return true;

            return false;
        }

        private void ActionPreExecute()
        {
            switch (_state)
            {
                case CircuitState.Closed:
                    break;
                case CircuitState.HalfOpen:
                    //TODO
                    break;
                case CircuitState.Open:
                    throw new BrokenCircuitException("The circuit is now open and is not allowing calls.", _lastException);
                case CircuitState.Isolated:
                    //throw new IsolatedCircuitException("The circuit is manually held open and is not allowing calls.");
                default:
                    throw new InvalidOperationException("Unhandled CircuitState.");
            }
        }

        public TResult ExecuteAction<TResult>(Func<TResult> action)
        {
            ActionPreExecute();
            bool success = true;
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                success = false;
                IncrementFailure();
                ValidateRules(ex);
                throw ex;
                //_lastException = ex;
            }
            finally
            {
                if(success)
                    IncrementSuccess();
            }
        }

        public void ExecuteAction(Action action)
        {
            ActionPreExecute();
            try
            {
                action();
                IncrementSuccess();
            }
            catch (Exception ex)
            {
                IncrementFailure();
                ValidateRules(ex);
                //_lastException = ex;
            }
        }
    }
}
