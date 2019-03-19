using DistributedCircuitBreaker.Redis.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DistributedCircuitBreaker.Redis.IntegratedTests
{
    public static class ServiceProviderFactory
    {
        public static IServiceProvider ServiceProvider { get; internal set; }

        static ServiceProviderFactory()
        {
            ServiceProvider = BuildServiceProvider();
        }

        private static IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddDistributedRedisCircuitBreaker(options =>
            {
                options.DurationOfBreak = TimeSpan.FromSeconds(10);
                options.WindowDuration = TimeSpan.FromSeconds(60);
                options.RedisConnectionConfiguration = "localhost:6379";
            });

            return services.BuildServiceProvider();
        }
    }
}
