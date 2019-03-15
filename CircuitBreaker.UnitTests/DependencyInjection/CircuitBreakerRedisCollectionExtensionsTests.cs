using DistributedCircuitBreaker.DependencyInjection;
using DistributedCircuitBreaker.Redis.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DistributedCircuitBreaker.Redis.IntegratedTests.DependencyException
{
    public class CircuitBreakerRedisCollectionExtensionsTests
    {
        [Fact]
        public void AddDistributedRedisCircuitBreaker_ShouldThrowExceptionWhenRedisConfigurationIsNull()
        {
            //arrange
            Action<CircuitBreakerRedisFactoryOptions> setupAction =
                new Action<CircuitBreakerRedisFactoryOptions>(
                (options) => options.RedisConnectionConfiguration = null
                );

            //act
            Action act = () => CircuitBreakerRedisCollectionExtensions.AddDistributedRedisCircuitBreaker(new ServiceCollection(), setupAction);

            //assert
            Assert.Throws<ArgumentNullException>(act);


        }
    }
}
