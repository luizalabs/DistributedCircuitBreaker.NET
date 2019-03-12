using System;

namespace DistributedCircuitBreaker.DependencyInjection
{
    public class CircuitBreakerRedisFactoryOptions
    {
        /// <summary>
        /// Redis database configuration string
        /// </summary>
        public string RedisConnectionConfiguration { get; set; }
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
