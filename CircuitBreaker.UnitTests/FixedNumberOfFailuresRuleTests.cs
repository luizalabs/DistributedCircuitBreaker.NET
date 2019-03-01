using System;
using Xunit;

namespace CircuitBreaker.UnitTests
{
    public class FixedNumberOfFailuresRuleTests
    {
        [Theory]
        [InlineData(20, 1, false)]
        [InlineData(20, 21, true)]
        [InlineData(20, 2, false)]
        [InlineData(10, 10, false)]
        [InlineData(10, 11, true)]
        [InlineData(1, 1, false)]
        [InlineData(1, 2, true)]
        public void ShouldBeTrueWhenNumberOfFailuresIsGreaterThenThreshold(int threshold, int failures, bool expected)
        {
            //Arrange
            var healthCount = new HealthCount()
            {
                Failures = failures,
                Successes = new Random(1).Next(100)
            };
            var sut = new FixedNumberOfFailuresRule(threshold);

            //Act
            var result = sut.ShouldOpenCircuitBreaker(healthCount);

            //Assert
            Assert.Equal(expected, result);
        }
    }
}
