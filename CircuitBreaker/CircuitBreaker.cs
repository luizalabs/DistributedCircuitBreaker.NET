using CircuitBreaker.Domain;
using System;
using System.Collections.Generic;

namespace CircuitBreaker
{
    public class CircuitBreaker
    {
        private List<IRule> _rules;
        private HealthCount _healthCount ;
        private string _key;
        private TimeSpan _windowDuration;
        private TimeSpan _durationOfBreak;
        private Exception _lastException;
        private IHealthCountService _healthCountService;

        /// <summary>
        /// Initializes an instance of CircuitBreaker
        /// </summary>
        /// <param name="key">The Key that defines a unique unit to be controlled by circuit breaker. E.g. a URI.</param>
        /// <param name="windowDuration">The amount of time considered to count failures</param>
        /// <param name="durationOfBreak">The amount of time the circuit should be open after changed to an open state</param>
        /// <param name="rules">Rules to be evaluated to decide if the state should be set to OPEN</param>
        /// <param name="repository">The repository that is used to store the CB information</param>
        public CircuitBreaker(string key, TimeSpan windowDuration, TimeSpan durationOfBreak, List<IRule> rules, IRepository repository)
        {
            _rules = rules;
            _key = key;
            _durationOfBreak = durationOfBreak;
            _windowDuration = windowDuration;
            _healthCountService = new HealthCountService(repository);
            SetHealthCount(key);
            CreateNewWindowIfExpired();
        }

        public CircuitState State
        {
            get { return _healthCountService.GetState(_key); }
            set { _healthCountService.SetState(_key, value); }
        }

        private void SetHealthCount(string key)
        {
            _healthCount = _healthCountService.GetCurrentHealthCount(key);            
        }

        /// <summary>
        /// Returns True is the state is Open
        /// </summary>
        /// <returns>bool</returns>
        public bool IsOpen()
        {
            return _healthCountService.GetState(_key).Equals(CircuitState.Open);

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
                if (r.ShouldOpenCircuitBreaker(_healthCount) == false)
                    shouldBreak = false;
            });

            if (!shouldBreak)
                return;

            OpenCircuit();
        }

        private void OpenCircuit()
        {
            State = CircuitState.Open;

            _healthCountService.SetBreakedAt(_key, DateTime.UtcNow.Ticks);
            throw new BrokenCircuitException("The circuit is now open and is not allowing calls.", _lastException);
        }

        private void IncrementFailure()
        {
            CreateNewWindowIfExpired();
            _healthCount.Failures++;
            _healthCountService.IncrementFailure(_key);
        }

        private void IncrementSuccess()
        {
            CreateNewWindowIfExpired();
            _healthCount.Successes++;
            _healthCountService.IncrementSuccess(_key);
        }

        private void CreateNewWindowIfExpired()
        {
            if(State == CircuitState.Closed && IsWindowExpired())
            {
                CreateNewWindow();
                return;
            }

            if(IsOpen() && IsBreakExpired())
            {
                CreateNewWindow();
                return;
            }
        }

        private void CreateNewWindow()
        {
            _healthCount = _healthCountService.GenerateNewHealthCounter(_key);
        }

        private bool IsWindowExpired()
        {
            long now = DateTime.UtcNow.Ticks;
            if (_healthCount == null || (now - _healthCount.StartedAt >= _windowDuration.Ticks))
                return true;

            return false;
        }

        private bool IsBreakExpired()
        {
            if (State == CircuitState.Closed)
                return true;

            long now = DateTime.UtcNow.Ticks;
            if (State == CircuitState.Open && (now - _healthCountService.GetBreakedAt(_key) > _durationOfBreak.Ticks))
                return true;

            return false;
        }

        private void ValidateCircuitStatus()
        {
            switch (State)
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
            ValidateCircuitStatus();

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
            ValidateCircuitStatus();
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
