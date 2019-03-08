using CircuitBreaker.Core;
using CircuitBreaker.Repository;
using NSubstitute;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CircuitBreaker.UnitTests
{
    public class HealthCountServiceTests
    {
        [Fact]
        public void ExecuteAction_ShouldGenerateClearHealthCount()
        {
            //arrange
            string key = "testKey";
            ICircuitBreakRepository repository = Substitute.For<ICircuitBreakRepository>();
            var service = ServiceProviderFactory.ServiceProvider.GetService<IHealthCountService>();
            var keys = new CircuitBreakerKeys(key);

            //act
            service.IncrementFailure(keys);

            var repo = ServiceProviderFactory.ServiceProvider.GetService<IRepository>();

            var fail = repo.GetString(keys.FailureCountKey);
            //Assert
        }
    }
}
