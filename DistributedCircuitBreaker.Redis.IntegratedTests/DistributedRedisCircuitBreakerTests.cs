using DistributedCircuitBreaker.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;
using Microsoft.Extensions.DependencyInjection;


namespace DistributedCircuitBreaker.Redis.IntegratedTests
{
    public class DistributedRedisCircuitBreakerTests : IDisposable
    {            
        private string key = "testKey";

        [Fact]
        public void ExecuteAction_StateShouldBeOpenAfterNumberOfFailuresExceedsThreshold()
        {
            //Arrange
            int numberOfFailuresThreshold = 2;
            var circuitBreakerFactory = ServiceProviderFactory.ServiceProvider.GetService<ICircuitBreakerFactory>();

            int actualNumberOfFailures = 0;

            //Act
            for (int i = 1; i < numberOfFailuresThreshold + 3; i++)
            {
                try
                {
                    var cb = circuitBreakerFactory.Create(key, numberOfFailuresThreshold);
                    cb.ExecuteAction(() => { throw new TimeoutException(); });
                }
                catch (TimeoutException) { }
                catch (BrokenCircuitException)
                {
                    actualNumberOfFailures = i - 1;//i stores the actual trial 
                    break;
                }
            }

            var cbf = circuitBreakerFactory.Create(key, numberOfFailuresThreshold);

            //Assert
            Assert.True(cbf.IsOpen());
            Assert.Equal(numberOfFailuresThreshold, actualNumberOfFailures);
            ClearRepositoryKeys(key);
        }

        private static void ClearRepositoryKeys(string key)
        {
            var repository = ServiceProviderFactory.ServiceProvider.GetService<IDistributedCircuitBreakerRepository>();
            CircuitBreakerKeys keys = new CircuitBreakerKeys(key);
            repository.Remove(keys.FailureCountKey);
            repository.Remove(keys.SuccessCountKey);
            repository.Remove(keys.StateKey);
        }

        [Fact]
        public void ExecuteAction_StateShouldBeClosedAfterDurationOfBreak()
        {
            //Arrange
            int numberOfFailuresThreshold = 2;
            var circuitBreakerFactory = ServiceProviderFactory.ServiceProvider.GetService<ICircuitBreakerFactory>();

            int actualNumberOfFailures = 0;

            //Act
            for (int i = 1; i <= numberOfFailuresThreshold + 1; i++)
            {
                try
                {
                    var cb = circuitBreakerFactory.Create(key, numberOfFailuresThreshold);
                    cb.ExecuteAction(() => { throw new TimeoutException(); });
                }
                catch (TimeoutException) { }
                catch (BrokenCircuitException)
                {
                    actualNumberOfFailures = i - 1;//i stores the actual trial 
                    break;
                }
            }

            var cbOpened = circuitBreakerFactory.Create(key, numberOfFailuresThreshold);
            Assert.True(cbOpened.IsOpen());
            Assert.Equal(numberOfFailuresThreshold, actualNumberOfFailures);

            Thread.Sleep(TimeSpan.FromSeconds(11));

            var cbClosed = circuitBreakerFactory.Create(key, numberOfFailuresThreshold);
            try
            {
                cbClosed.ExecuteAction(() => { throw new TimeoutException(); });
            }
            catch (TimeoutException) { }
            catch (BrokenCircuitException)
            {

            }
            //Assert
            Assert.False(cbClosed.IsOpen());
            ClearRepositoryKeys(key);
        }

        [Fact]
        public void ExecuteAction_StateShouldBeClosedWhenJustCreated()
        {
            //Arrange
            var circuitBreakerFactory = ServiceProviderFactory.ServiceProvider.GetService<ICircuitBreakerFactory>();
            var repository = ServiceProviderFactory.ServiceProvider.GetService<IDistributedCircuitBreakerRepository>();
            Dictionary<string, byte[]> dic = new Dictionary<string, byte[]>();

            //Act
            var cbClosed = circuitBreakerFactory.Create(key, 2);

            //Assert
            Assert.False(cbClosed.IsOpen());
        }

        [Fact]
        public void ExecuteAction_ShouldPropagateExceptionWhenFunctionThrowsException()
        {
            //Arrange
            int numberOfFailuresThreshold = 2;
            var circuitBreakerFactory = ServiceProviderFactory.ServiceProvider.GetService<ICircuitBreakerFactory>();

            bool exceptionExpectedWasThrown = false;
            try
            {
                var cb = circuitBreakerFactory.Create(key, numberOfFailuresThreshold);
                cb.ExecuteAction<int>(() => { throw new TimeoutException(); });
            }
            catch (TimeoutException)
            {
                exceptionExpectedWasThrown = true;
            }
            catch (BrokenCircuitException ex)
            {

            }
            Assert.True(exceptionExpectedWasThrown);
            ClearRepositoryKeys(key);
        }

        public void Dispose()
        {
            ClearRepositoryKeys(key);
        }
    }
}
