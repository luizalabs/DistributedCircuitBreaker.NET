using CircuitBreaker.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CircuitBreaker
{
    public class CircuitBreaker
    {
        private List<IRule> _rules;
        private HealthCount _healthCount ;
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
        internal CircuitBreaker(string key, TimeSpan windowDuration, TimeSpan durationOfBreak, List<IRule> rules, IRepository repository)
        {
            _rules = rules;
            _healthCountService = new HealthCountService(key, repository, windowDuration, durationOfBreak);
        }

        public CircuitState State
        {
            get { return _healthCountService.GetState(); }
        }

        private void SetCurrentHealthCount()
        {
            _healthCount = _healthCountService.GetCurrentHealthCount();            
        }

        /// <summary>
        /// Returns True is the state is Open
        /// </summary>
        /// <returns>bool</returns>
        public bool IsOpen()
        {
            return State.Equals(CircuitState.Open);
        }

        /// <summary>
        /// Check All rules. If all rules returns true, should open circuit
        /// </summary>
        /// <param name="ex"></param>
        private bool ShouldOpenCircuit()
        {
            return _rules.All(rule => rule.ValidateRule(_healthCount));
        }

        private void OpenCircuit()
        {
            _healthCountService.OpenCircuit();
        }

        private void IncrementFailure()
        {
            _healthCount.Failures++;
            _healthCountService.IncrementFailure();
        }

        private void IncrementSuccess()
        {
            _healthCount.Successes++;
            _healthCountService.IncrementSuccess();
        }

        private void ValidateCircuitStatus()
        {
            switch (State)
            {
                case CircuitState.Closed:
                    break;
                case CircuitState.Open:
                    throw new BrokenCircuitException("The circuit is now open and is not allowing calls.", _lastException);
                default:
                    throw new InvalidOperationException("Unhandled CircuitState.");
            }
        }

        public TResult ExecuteAction<TResult>(Func<TResult> action)
        {
            SetCurrentHealthCount();
            ValidateCircuitStatus();

            try
            {
                TResult result = action();
                IncrementSuccess();
                return result;
            }
            catch (Exception ex)
            {
                _lastException = ex;
                IncrementFailure();
                if (ShouldOpenCircuit())
                {
                    OpenCircuit();
                    throw new BrokenCircuitException("The circuit is now open and is not allowing calls.", _lastException);
                }
                throw ex;
            }
        }

        public void ExecuteAction(Action action)
        {
            SetCurrentHealthCount();
            ValidateCircuitStatus();
            try
            {
                action();
                IncrementSuccess();
            }
            catch (Exception ex)
            {
                _lastException = ex;
                IncrementFailure();
                if (ShouldOpenCircuit())
                {
                    OpenCircuit();
                    throw new BrokenCircuitException("The circuit is now open and is not allowing calls.", _lastException);
                }
            }
        }
    }
}
