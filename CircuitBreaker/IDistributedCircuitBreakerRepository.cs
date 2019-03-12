using System;

namespace DistributedCircuitBreaker
{
    public interface IDistributedCircuitBreakerRepository
    {
        void Increment(string key);
        /// <summary>
        ///  Gets a value with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested value.</param>
        /// <returns>The located value or null.</returns>
        string GetString(string key);

        void Set(string key, int value);

        void Set(string key, int value, TimeSpan absoluteExpiration);

        void Remove(string key);
    }
}
