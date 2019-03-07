using CircuitBreaker.Core;
using System;
using Xunit;

namespace CircuitBreaker.UnitTests
{
    public class ProportionFailuresRuleTests
    {
        [Theory]
        [InlineData(0.49, 1, 1,true)]
        [InlineData(0.50, 1, 1, false)]
        [InlineData(0.50, 1, 2, false)]
        [InlineData(0.50, 2, 1, true)]
        [InlineData(0.10, 11, 89, true)]
        public void ShouldBeTrueWhenNumberOfFailuresIsGreaterThenThreshold(decimal threshold, int failures, int successes,bool expected)
        {
            //Arrange
            var healthCount = new HealthCount()
            {
                Failures = failures,
                Successes = successes
            };
            var sut = new ProportionFailuresRule(threshold);

            //Act
            var result = sut.ValidateRule(healthCount);

            //Assert
            Assert.Equal(expected, result);
        }
    }
}
