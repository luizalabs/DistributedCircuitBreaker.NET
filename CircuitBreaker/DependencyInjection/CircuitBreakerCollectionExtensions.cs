using DistributedCircuitBreaker.Core;
using DistributedCircuitBreaker.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DistributedCircuitBreaker.DependencyInjection
{
    public static class CircuitBreakerCollectionExtensions
    {
        public static IServiceCollection AddDistributedCircuitBreaker(this IServiceCollection collection,
    Action<CircuitBreakerFactoryOptions> setupAction)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

            collection.Configure(setupAction);

            collection.AddTransient<ICircuitBreakRepository, CircuitBreakRepository>();
            collection.AddTransient<ICircuitBreakerFactory, CircuitBreakerFactory>();
            return collection.AddTransient<IHealthCountService, HealthCountService>();

        }
    }
}
