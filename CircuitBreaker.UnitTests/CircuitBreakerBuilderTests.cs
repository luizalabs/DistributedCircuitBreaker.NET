using CircuitBreaker.Core;
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

            //Act
            Action act = () => CircuitBreakBuilder.Build(key, windowDuration, durationOfBreak, rules, repository);

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

            //Act
            Action act = () => CircuitBreakBuilder.Build(string.Empty, windowDuration, durationOfBreak, rules, repository);

            //Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void ShouldThrowExceptionWhenInstantiateWithoutRepository()
        {
            //Arrange
            TimeSpan windowDuration = TimeSpan.FromSeconds(1000);
            TimeSpan durationOfBreak = TimeSpan.FromSeconds(15);
            List<IRule> rules = new List<IRule>();

            //Act
            Action act = () => CircuitBreakBuilder.Build("http://www.contoso.com", windowDuration, durationOfBreak, rules, null);

            //Assert
            Assert.Throws<ArgumentException>(act);
        }
    }
}
