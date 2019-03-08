using CircuitBreaker.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace CircuitBreaker.Core
{
    public class CircuitBreakerFactory : ICircuitBreakerFactory
    {
        private TimeSpan _windowDuration;
        private TimeSpan _durationOfBreak;
        private IRepository _repository;

        public CircuitBreakerFactory(IOptions<CircuitBreakerFactoryOptions> options)
        {
            _windowDuration = options.Value.WindowDuration == null ? TimeSpan.FromSeconds(1) : options.Value.WindowDuration;
            _durationOfBreak = options.Value.DurationOfBreak == null ? TimeSpan.FromSeconds(5) : options.Value.DurationOfBreak;
            _repository = options.Value.Repository;

            if (_repository == null)
                throw new ArgumentException("Repository could not be null");
        }

        /// <summary>
        /// Initializes an instance of CircuitBreaker
        /// </summary>
        /// <param name="key">The Key that defines a unique unit to be controlled by circuit breaker. E.g. a URI.</param>
        /// <param name="rules">Rules to be evaluated to decide if the state should be set to OPEN</param>
        /// <returns>new instance of CircuitBreaker</returns>
        /// <exception cref="System.ArgumentException">key;key must be provided</exception>
        /// <exception cref="System.ArgumentException">rules;At least one rule must be provided</exception>
        /// <exception cref="System.ArgumentException">repository;Repository could not be null</exception>
        public CircuitBreaker Create(string key,  List<IRule> rules)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key must be provided");

            if (rules == null || rules.Count == 0)
                throw new ArgumentException("At least one rule must be provided");

            return new CircuitBreaker(key, _windowDuration, _durationOfBreak, rules, _repository);
        }

        /// <summary>
        /// Initializes an instance of CircuitBreaker 
        /// CircuitBreaker will be configured with Fixed number of exceptions allowed before break also considering a proportion of failures compared to total executions
        /// The two rules must be broken to cause the Circuit to open
        /// </summary>
        /// <param name="key">The Key that defines a unique unit to be controlled by circuit breaker. E.g. a URI.</param>
        /// <param name="exceptionsAllowedBeforeBreaking">Number of exceptions which Circuit should be opened after</param>
        /// <param name="failureRateAllowed">The rate in % of failure permitted before open the circuit. e.g 0.5M for 50% of failure tolerance</param>
        /// <returns>new instance of CircuitBreaker</returns>
        /// <exception cref="System.ArgumentException">key;key must be provided</exception>
        /// <exception cref="System.ArgumentException">rules;At least one rule must be provided</exception>
        /// <exception cref="System.ArgumentException">repository;Repository could not be null</exception>
        public CircuitBreaker Create(string key, int exceptionsAllowedBeforeBreaking, decimal failureRateAllowed)
        {
            var rules = new List<IRule>()
            {
                new FixedNumberOfFailuresRule(exceptionsAllowedBeforeBreaking),
                new ProportionFailuresRule(failureRateAllowed)
            };

            return Create(key, rules);
        }

        /// <summary>
        /// Initializes an instance of CircuitBreaker 
        /// CircuitBreaker will be configured with Fixed number of exceptions allowed before break
        /// </summary>
        /// <param name="key">The Key that defines a unique unit to be controlled by circuit breaker. E.g. a URI.</param>
        /// <param name="exceptionsAllowedBeforeBreaking">Number of exceptions which Circuit should be opened after</param>
        /// <returns>new instance of CircuitBreaker</returns>
        /// <exception cref="System.ArgumentException">key;key must be provided</exception>
        /// <exception cref="System.ArgumentException">rules;At least one rule must be provided</exception>
        /// <exception cref="System.ArgumentException">repository;Repository could not be null</exception>
        public CircuitBreaker Create(string key,int exceptionsAllowedBeforeBreaking)
        {
            var rules = new List<IRule>()
            {
                new FixedNumberOfFailuresRule(exceptionsAllowedBeforeBreaking)
            };

            return Create(key, rules);
        }
    }
}
