using System;

namespace CircuitBreaker.DependencyInjection
{
    public class CircuitBreakerFactoryOptions
    {
        /// <summary>
        /// The repository that is used to store the CircuitBreaker information
        /// </summary>
        public IRepository Repository { get; set; }
        /// <summary>
        /// The amount of time considered to count failures
        /// </summary>
        public TimeSpan WindowDuration { get; set; }
        /// <summary>
        /// The amount of time the circuit should be open after changed to an open state
        /// </summary>
        public TimeSpan DurationOfBreak { get; set; }
    }
}
