using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitBreaker.Core
{
    public static class CircuitBreakBuilder
    {

        /// <summary>
        /// Initializes an instance of CircuitBreaker
        /// </summary>
        /// <param name="key">The Key that defines a unique unit to be controlled by circuit breaker. E.g. a URI.</param>
        /// <param name="windowDuration">The amount of time considered to count failures</param>
        /// <param name="durationOfBreak">The amount of time the circuit should be open after changed to an open state</param>
        /// <param name="rules">Rules to be evaluated to decide if the state should be set to OPEN</param>
        /// <param name="repository">The repository that is used to store the CB information</param>
        /// <returns>new instance of CircuitBreaker</returns>
        /// <exception cref="System.ArgumentException">key;key must be provided</exception>
        /// <exception cref="System.ArgumentException">rules;At least one rule must be provided</exception>
        /// <exception cref="System.ArgumentException">repository;Repository could not be null</exception>
        public static CircuitBreaker Build(string key, TimeSpan windowDuration, TimeSpan durationOfBreak, List<IRule> rules, IRepository repository)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key must be provided");

            if (rules == null || rules.Count == 0)
                throw new ArgumentException("At least one rule must be provided");

            if (repository == null)
                throw new ArgumentException("Repository could not be null");

            return new CircuitBreaker(key, windowDuration, durationOfBreak, rules, repository);
        }


        /// <summary>
        /// Initializes an instance of CircuitBreaker setting a default value of 1 minute to windowDuration and 5 minutes to durationOfBreak
        /// </summary>
        /// <param name="key">The Key that defines a unique unit to be controlled by circuit breaker. E.g. a URI.</param>
        /// <param name="rules">Rules to be evaluated to decide if the state should be set to OPEN</param>
        /// <param name="repository">The repository that is used to store the CB information</param>
        /// <returns>new instance of CircuitBreaker</returns>
        public static CircuitBreaker Build(string key, List<IRule> rules, IRepository repository)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key must be provided");

            if (rules == null || rules.Count == 0)
                throw new ArgumentException("At least one rule must be provided");

            if (repository == null)
                throw new ArgumentException("Repository could not be null");

            return Build(key, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5), rules, repository);
        }
    }
}
