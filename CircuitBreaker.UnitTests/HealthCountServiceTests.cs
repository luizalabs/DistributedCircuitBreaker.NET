using DistributedCircuitBreaker.Core;
using DistributedCircuitBreaker.Repository;
using NSubstitute;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DistributedCircuitBreaker.UnitTests
{
    public class HealthCountServiceTests
    {
        [Fact]
        public void ExecuteAction_ShouldGenerateClearHealthCount()
        {
            //arrange
            string key = "testKey";
            var service = ServiceProviderFactory.ServiceProvider.GetService<IHealthCountService>();
            var keys = new CircuitBreakerKeys(key);

            //act
            service.IncrementFailure(keys);

            var repo = ServiceProviderFactory.ServiceProvider.GetService<IDistributedCircuitBreakerRepository>();

            var fail = repo.GetString(keys.FailureCountKey);
            //Assert
        }
    }
}
