using DistributedCircuitBreaker;
using DistributedCircuitBreaker.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;

namespace DistributedCircuitbreaker.Redis
{
    public class DistributedRedisRepository : IDistributedCircuitBreakerRepository
    {
        private readonly IDistributedCache _cache;
        private readonly IOptions<CircuitBreakerRedisFactoryOptions> _options;

        public DistributedRedisRepository(IDistributedCache cache, IOptions<CircuitBreakerRedisFactoryOptions> options)
        {
            _cache = cache;
            _options = options;
        }

        public void Increment(string key)
        {
            using (var connection = StackExchange.Redis.ConnectionMultiplexer.Connect(_options.Value.RedisConnectionConfiguration))
            {
                connection.GetDatabase().HashIncrement(key,"data",1);
            }
        }

        public string GetString(string key)
        {
            return _cache.GetString(key);
        }

        public void Set(string key, int value)
        {
            _cache.SetString(key, value.ToString());
        }

        public void Set(string key, int value, TimeSpan absoluteExpiration)
        {
            var cacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(absoluteExpiration);
            _cache.SetString(key, value.ToString(), cacheEntryOptions);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
