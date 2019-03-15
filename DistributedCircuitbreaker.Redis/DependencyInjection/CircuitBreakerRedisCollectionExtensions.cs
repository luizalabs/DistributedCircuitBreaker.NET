using DistributedCircuitbreaker.Redis;
using DistributedCircuitBreaker.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DistributedCircuitBreaker.Redis.DependencyInjection
{
    public static class CircuitBreakerRedisCollectionExtensions
    {
        public static IServiceCollection AddDistributedRedisCircuitBreaker(this IServiceCollection collection,
    Action<CircuitBreakerRedisFactoryOptions> setupAction)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

            collection.Configure(setupAction);
            collection.AddTransient<IDistributedCircuitBreakerRepository, DistributedRedisRepository>();

            var redisOptions = new CircuitBreakerRedisFactoryOptions();
            setupAction(redisOptions);

            if(redisOptions.RedisConnectionConfiguration == null)
                throw new ArgumentNullException(nameof(redisOptions.RedisConnectionConfiguration));

            collection.AddDistributedRedisCache(options => options.Configuration = redisOptions.RedisConnectionConfiguration);

            collection.AddDistributedCircuitBreaker(options =>
            {
                options.DurationOfBreak = redisOptions.DurationOfBreak;
                options.WindowDuration = redisOptions.WindowDuration;
            }); 
            return collection;
        }
    }
}
