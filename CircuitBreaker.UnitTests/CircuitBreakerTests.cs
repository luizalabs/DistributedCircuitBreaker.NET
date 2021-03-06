using DistributedCircuitBreaker.Core;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace DistributedCircuitBreaker.UnitTests
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
            var repository = ServiceProviderFactory.ServiceProvider.GetService<IDistributedCircuitBreakerRepository>();
            Dictionary<string, string> dic = new Dictionary<string, string>();
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
                catch (TimeoutException)
                {

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
            var repository = ServiceProviderFactory.ServiceProvider.GetService<IDistributedCircuitBreakerRepository>();
            Dictionary<string, string> dic = new Dictionary<string, string>();
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
                catch(TimeoutException)
                {

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

            Thread.Sleep(TimeSpan.FromSeconds(15));
            dic.Clear();

            var cbClosed = circuitBreakerFactory.Create(key, numberOfFailuresThreshold);
            try
            {
                cbClosed.ExecuteAction(() => { throw new TimeoutException(); });
            }
            catch (TimeoutException)
            {

            }
            catch (BrokenCircuitException)
            {
               
            }
            //Assert
            Assert.False(cbClosed.IsOpen());
        }

        [Fact]
        public void ExecuteAction_StateShouldBeClosedWhenJustCreated()
        {
            //Arrange
            string key = "testKey";
            var circuitBreakerFactory = ServiceProviderFactory.ServiceProvider.GetService<ICircuitBreakerFactory>();
            var repository = ServiceProviderFactory.ServiceProvider.GetService<IDistributedCircuitBreakerRepository>();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            ServiceProviderFactory.SetRepositoryBehavior(key, repository, dic);
            var cbClosed = circuitBreakerFactory.Create(key, 2);

            //Act

            //Assert
            Assert.False(cbClosed.IsOpen());
        }

        [Fact]
        public void ExecuteAction_ShouldPropagateExceptionWhenActionThrowsException()
        {
            //Arrange
            string key = "testKey";
            int numberOfFailuresThreshold = 2;
            var circuitBreakerFactory = ServiceProviderFactory.ServiceProvider.GetService<ICircuitBreakerFactory>();
            var repository = ServiceProviderFactory.ServiceProvider.GetService<IDistributedCircuitBreakerRepository>();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            ServiceProviderFactory.SetRepositoryBehavior(key, repository, dic);

            bool exceptionExpectedWasThrown = false;
            try
            {
                var cb = circuitBreakerFactory.Create(key, numberOfFailuresThreshold);
                cb.ExecuteAction(() => { throw new TimeoutException(); });
            }
            catch (TimeoutException)
            {
                exceptionExpectedWasThrown = true;
            }
            catch (BrokenCircuitException ex)
            {

            }
            Assert.True(exceptionExpectedWasThrown);
        }


        [Fact]
        public async void ExecuteAction_ShouldPropagateExceptionWhenAsyncFunctionThrowsExceptionAsync()
        {
            //Arrange
            string key = "testKey";
            int numberOfFailuresThreshold = 2;
            var circuitBreakerFactory = ServiceProviderFactory.ServiceProvider.GetService<ICircuitBreakerFactory>();
            var repository = ServiceProviderFactory.ServiceProvider.GetService<IDistributedCircuitBreakerRepository>();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            ServiceProviderFactory.SetRepositoryBehavior(key, repository, dic);

            bool exceptionExpectedWasThrown = false;
            try
            {
                var cb = circuitBreakerFactory.Create(key, numberOfFailuresThreshold);

                var funcTimeout = new Func<Task<int>>(async () =>
                {
                    await Task.Delay(1000);
                    throw new TimeoutException();
                });

                var i = await cb.ExecuteActionAsync(() => funcTimeout());
            }
            catch (TimeoutException)
            {
                exceptionExpectedWasThrown = true;
            }
            catch (BrokenCircuitException ex)
            {

            }
            Assert.True(exceptionExpectedWasThrown);

            var keyChain = new CircuitBreakerKeys(key);

            string failuresCount = repository.GetString(keyChain.FailureCountKey);
            string successCount = repository.GetString(keyChain.SuccessCountKey);

            Assert.True(failuresCount == "1");
            Assert.True(successCount == "0" || string.IsNullOrEmpty(successCount));
        }

        [Fact]
        public async void ExecuteAction_ShouldPropagateExceptionWhenAsyncActionThrowsExceptionAsync()
        {
            //Arrange
            string key = "testKey";
            int numberOfFailuresThreshold = 2;
            var circuitBreakerFactory = ServiceProviderFactory.ServiceProvider.GetService<ICircuitBreakerFactory>();
            var repository = ServiceProviderFactory.ServiceProvider.GetService<IDistributedCircuitBreakerRepository>();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            ServiceProviderFactory.SetRepositoryBehavior(key, repository, dic);

            bool exceptionExpectedWasThrown = false;
            try
            {
                var cb = circuitBreakerFactory.Create(key, numberOfFailuresThreshold);

                var funcTimeout = new Func<Task>(async () =>
                {
                    await Task.Delay(1000);
                    throw new TimeoutException();
                });
                await cb.ExecuteActionAsync(() => funcTimeout());
            }
            catch (TimeoutException)
            {
                exceptionExpectedWasThrown = true;
            }
            catch (BrokenCircuitException ex)
            {

            }
            Assert.True(exceptionExpectedWasThrown);

            var keyChain = new CircuitBreakerKeys(key);

            string failuresCount = repository.GetString(keyChain.FailureCountKey);
            string successCount = repository.GetString(keyChain.SuccessCountKey);

            Assert.True(failuresCount == "1");
            Assert.True(successCount == "0" || string.IsNullOrEmpty(successCount));
        }

    }
}
