using DistributedCircuitBreaker.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributedCircuitBreaker
{
    /// <summary>
    /// Responsible for controlling the circuit Break, validate if the rules are broken
    /// </summary>
    public class CircuitBreaker : ICircuitBreaker
    {
        private List<IRule> _rules;
        private HealthCount _healthCount ;
        private Exception _lastException;
        private IHealthCountService _healthCountService;
        private CircuitBreakerKeys _keys;

        /// <summary>
        /// Initializes an instance of CircuitBreaker
        /// </summary>
        /// <param name="key">The Key that defines a unique unit to be controlled by circuit breaker. E.g. a URI.</param>
        /// <param name="rules">Rules to be evaluated to decide if the state should be set to OPEN</param>
        /// <param name="service">service that deal with the persistance of the information</param>
        internal CircuitBreaker(string key, List<IRule> rules, IHealthCountService service)
        {
            _rules = rules;
            _keys = new CircuitBreakerKeys(key);
            _healthCountService = service;
        }

        public CircuitState State
        {
            get { return _healthCountService.GetState(_keys); }
        }

        private void RetrieveCurrentHealthCount()
        {
            _healthCount = _healthCountService.GetCurrentHealthCount(_keys);            
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
            _healthCountService.OpenCircuit(_keys);
        }

        private void IncrementFailure()
        {
            _healthCount.Failures++;
            _healthCountService.IncrementFailure(_keys);
        }

        private void IncrementSuccess()
        {
            _healthCount.Successes++;
            _healthCountService.IncrementSuccess(_keys);
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

        /// <summary>
        ///  Warp the call to a function with the circuitbreaker
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public TResult ExecuteAction<TResult>(Func<TResult> action)
        {
            RetrieveCurrentHealthCount();
            ValidateCircuitStatus();

            try
            {
                TResult result = action();
                IncrementSuccess();
                return result;
            }
            catch (Exception ex)
            {
                HandleFailureExecution(ex);
                throw ex;
            }
        }

        public async Task<TResult> ExecuteActionAsync<TResult>(Func<Task<TResult>> action)
        {
            RetrieveCurrentHealthCount();
            ValidateCircuitStatus();

            try
            {
                var f = new Func<Task<TResult>>(async () =>
                {
                    return await action();
                });

                TResult result = await f();
                IncrementSuccess();
                return result;
            }
            catch (Exception ex)
            {
                HandleFailureExecution(ex);
                throw ex;
            }
        }

        public void ExecuteAction(Action action)
        {
            RetrieveCurrentHealthCount();
            ValidateCircuitStatus();
            try
            {
                action();
                IncrementSuccess();
            }
            catch (Exception ex)
            {
                HandleFailureExecution(ex);
                throw ex;
            }
        }

        private void HandleFailureExecution(Exception ex)
        {
            IncrementFailure();
            if (ShouldOpenCircuit())
            {
                OpenCircuit();
                throw new BrokenCircuitException("The circuit is now open and is not allowing calls.", ex);
            }
        }

        public async Task ExecuteActionAsync(Func<Task> action)
        {
            RetrieveCurrentHealthCount();
            ValidateCircuitStatus();
            try
            {
                var c = new Func<Task>(async() => 
                {
                    await action();
                });

                await c();
                IncrementSuccess();
            }
            catch (Exception ex)
            {
                HandleFailureExecution(ex);
                throw ex;
            }
        }
    }
}
