using DistributedCircuitBreaker.Core;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;
using Microsoft.Extensions.DependencyInjection;


namespace DistributedCircuitBreaker.UnitTests
{
    public class CircuitBreakerKeysTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void FailureCountKey_ShouldThrowExceptionWhenKeyIsNull(string value)
        {
            //arrange
            var cbkeys = new CircuitBreakerKeys(value);
            //act
            Func<string> act = new Func<string>(() => cbkeys.FailureCountKey);
            //assert
            Assert.Throws<Exception>(act);
        }

        [Theory]
        [InlineData("teste1","teste1-failure")]
        [InlineData("1", "1-failure")]
        [InlineData("##$", "##$-failure")]
        [InlineData("http://www.contoso.com", "http://www.contoso.com-failure")]
        public void FailureCountKey_ShouldReturnKeyWithSuffixWhenKeyIsNotNull(string value, string expected)
        {
            //arrange
            var cbkeys = new CircuitBreakerKeys(value);
            //act
            //assert
            Assert.Equal(expected, cbkeys.FailureCountKey);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SuccessCountKey_ShouldThrowExceptionWhenKeyIsNull(string value)
        {
            //arrange
            var cbkeys = new CircuitBreakerKeys(value);
            //act
            Func<string> act = new Func<string>(() => cbkeys.SuccessCountKey);
            //assert
            Assert.Throws<Exception>(act);
        }

        [Theory]
        [InlineData("teste1", "teste1-success")]
        [InlineData("1", "1-success")]
        [InlineData("##$", "##$-success")]
        [InlineData("http://www.contoso.com", "http://www.contoso.com-success")]
        public void SuccessCountKey_ShouldReturnKeyWithSuffixWhenKeyIsNotNull(string value, string expected)
        {
            //arrange
            var cbkeys = new CircuitBreakerKeys(value);
            //act
            //assert
            Assert.Equal(expected, cbkeys.SuccessCountKey);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void StateCountKey_ShouldThrowExceptionWhenKeyIsNull(string value)
        {
            //arrange
            var cbkeys = new CircuitBreakerKeys(value);
            //act
            Func<string> act = new Func<string>(() => cbkeys.StateKey);
            //assert
            Assert.Throws<Exception>(act);
        }

        [Theory]
        [InlineData("teste1", "teste1-state")]
        [InlineData("1", "1-state")]
        [InlineData("##$", "##$-state")]
        [InlineData("http://www.contoso.com", "http://www.contoso.com-state")]
        public void StateCountKey_ShouldReturnKeyWithSuffixWhenKeyIsNotNull(string value, string expected)
        {
            //arrange
            var cbkeys = new CircuitBreakerKeys(value);
            //act
            //assert
            Assert.Equal(expected, cbkeys.StateKey);
        }
    }
}
