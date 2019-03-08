using CircuitBreaker.Core;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;

namespace CircuitBreaker.UnitTests
{
    public class CircuitBreakerBuilderTests
    {

        [Fact]
        public void ShouldThrowExceptionWhenInstantiatePassingEmptyRuleList()
        {
            //Arrange
            string key = "testKey";
            TimeSpan windowDuration = TimeSpan.FromSeconds(1000);
            TimeSpan durationOfBreak = TimeSpan.FromSeconds(15);
            IRepository repository = Substitute.For<IRepository>();
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
            TimeSpan windowDuration = TimeSpan.FromSeconds(1000);
            TimeSpan durationOfBreak = TimeSpan.FromSeconds(15);
            IRepository repository = Substitute.For<IRepository>();
            List<IRule> rules = new List<IRule>();
            var circuitBreakerFactory = ServiceProviderFactory.ServiceProvider.GetService<ICircuitBreakerFactory>();

            //Act
            Action act = () => circuitBreakerFactory.Create(string.Empty, rules);

            //Assert
            Assert.Throws<ArgumentException>(act);
        }

        //[Fact]
        //public void ShouldThrowExceptionWhenInstantiateWithoutRepository()
        //{
        //    //Arrange
        //    TimeSpan windowDuration = TimeSpan.FromSeconds(1000);
        //    TimeSpan durationOfBreak = TimeSpan.FromSeconds(15);
        //    List<IRule> rules = new List<IRule>();
        //    var CircuitBreakerFactory = ServiceProviderFactory.ServiceProvider.GetService<ICircuitBreakerFactory>();

        //    //Act
        //    Action act = () => CircuitBreakerFactory.Create("http://www.contoso.com", rules);

        //    //Assert
        //    Assert.Throws<ArgumentException>(act);
        //}
    }
}
