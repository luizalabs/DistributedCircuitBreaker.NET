using CircuitBreaker.Core;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace CircuitBreaker.UnitTests
{
    public class CircuitBreakerTests
    {
        [Fact]
        public void ExecuteAction_StateShouldBeOpenAfterNumberOfFailuresExceedsThreshold()
        {
            //Arrange
            string key = "testKey";
            int numberOfFailuresThreshold = 2;
            var circuitBreakerFactory = ServiceProviderFactory.ServiceProvider.GetService<ICircuitBreakerFactory>();
            var repository = ServiceProviderFactory.ServiceProvider.GetService<IRepository>();
            Dictionary<string, byte[]> dic = new Dictionary<string, byte[]>();
            ServiceProviderFactory.SetRepositoryBehavior(key, repository, dic);

            int actualNumberOfFailures = 0;

            //Act
            for (int i = 1; i < numberOfFailuresThreshold + 3; i++)
            {
                try
                {
                    var cb = circuitBreakerFactory.Create(key, numberOfFailuresThreshold);
                    cb.ExecuteAction(() => { throw new TimeoutException(); });
                }
                catch (BrokenCircuitException ex)
                {
                    actualNumberOfFailures = i - 1;//i stores the actual trial 
                    break;
                }
            }

            var cbf = circuitBreakerFactory.Create(key, numberOfFailuresThreshold);

            //Assert
            Assert.True(cbf.IsOpen());
            Assert.Equal(numberOfFailuresThreshold, actualNumberOfFailures);
        }

        [Fact]
        public void ExecuteAction_StateShouldBeClosedAfterDurationOfBreak()
        {
            //Arrange
            string key = "testKey";
            int numberOfFailuresThreshold = 2;
            var circuitBreakerFactory = ServiceProviderFactory.ServiceProvider.GetService<ICircuitBreakerFactory>();
            var repository = ServiceProviderFactory.ServiceProvider.GetService<IRepository>();
            Dictionary<string, byte[]> dic = new Dictionary<string, byte[]>();
            ServiceProviderFactory.SetRepositoryBehavior(key, repository, dic);

            int actualNumberOfFailures = 0;

            //Act
            for (int i = 1; i <= numberOfFailuresThreshold +1; i++)
            {
                try
                {
                    var cb = circuitBreakerFactory.Create(key, numberOfFailuresThreshold);
                    cb.ExecuteAction(() => { throw new TimeoutException(); });
                }
                catch (BrokenCircuitException)
                {
                    actualNumberOfFailures = i - 1;//i stores the actual trial 
                    break;
                }
            }

            var cbOpened = circuitBreakerFactory.Create(key, numberOfFailuresThreshold);
            Assert.True(cbOpened.IsOpen());
            Assert.Equal(numberOfFailuresThreshold, actualNumberOfFailures);

            Thread.Sleep(TimeSpan.FromSeconds(1));
            dic.Clear();

            var cbClosed = circuitBreakerFactory.Create(key, numberOfFailuresThreshold);
            cbClosed.ExecuteAction(() => { throw new TimeoutException(); });
            //Assert
            Assert.False(cbClosed.IsOpen());
        }


    }
}
