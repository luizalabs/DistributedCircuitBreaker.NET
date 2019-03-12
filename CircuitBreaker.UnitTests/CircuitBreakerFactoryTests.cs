using DistributedCircuitBreaker.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Xunit;

namespace DistributedCircuitBreaker.UnitTests
{
    public class CircuitBreakerFactoryTests
    {

        [Fact]
        public void ShouldThrowExceptionWhenInstantiatePassingEmptyRuleList()
        {
            //Arrange
            string key = "testKey";
            List<IRule> rules = new List<IRule>();
            var circuitBreakerFactory = ServiceProviderFactory.ServiceProvider.GetService<ICircuitBreakerFactory>();

            //Act
            Action act = () => circuitBreakerFactory.Create(key, rules);

            //Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void ShouldThrowExceptionWhenInstantiateWithNoKey()
        {
            //Arrange
            List<IRule> rules = new List<IRule>();
            var circuitBreakerFactory = ServiceProviderFactory.ServiceProvider.GetService<ICircuitBreakerFactory>();

            //Act
            Action act = () => circuitBreakerFactory.Create(string.Empty, rules);

            //Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void ShouldCreateNotNullCircuitBreakerWhenArgumentsAreValid()
        {
            //Arrange
            List<IRule> rules = new List<IRule>();
            rules.Add(new FixedNumberOfFailuresRule(2));
            var circuitBreakerFactory = ServiceProviderFactory.ServiceProvider.GetService<ICircuitBreakerFactory>();

            //Act
            var cb = circuitBreakerFactory.Create("http://www.contoso.com", rules);
            //Assert
            Assert.NotNull(cb);
        }
    }
}
