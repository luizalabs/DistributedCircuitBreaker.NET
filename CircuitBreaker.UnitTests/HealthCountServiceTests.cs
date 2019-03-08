using NSubstitute;
using System;
using System.Collections.Generic;

namespace CircuitBreaker.UnitTests
{
    public class HealthCountServiceTests
    {
        //[Fact]
        //public void ExecuteAction_ShouldGenerateClearHealthCount()
        //{
        //    //arrange
        //    string key = "testKey";
        //    IRepository repository = Substitute.For<IRepository>();
        //    Dictionary<string, byte[]> dic = new Dictionary<string, byte[]>();
        //    SetRepositoryBehavior(key, repository, dic);

        //    //act
        //    long now = DateTime.UtcNow.Ticks;
        //    HealthCount generated = new HealthCountService(repository, 0,0).GenerateNewHealthCounter(key);

        //    HealthCount n = new HealthCount()
        //    {
        //        Failures = 0,
        //        Successes = 0,
        //       // StartedAt = now
        //    };

        //    //Assert
        //    Assert.Equal(n.Successes, generated.Successes);
        //    Assert.Equal(n.Failures, generated.Failures);
        //    //Assert.Equal(new DateTime( n.StartedAt).ToShortDateString(), new DateTime( generated.StartedAt).ToShortDateString());
        //}

        //[Fact]
        //public void ExecuteAction_ShouldGenerateClearHealthCount()
        //{
        //    //arrange
        //    string key = "testKey";
        //    IRepository repository = Substitute.For<IRepository>();
        //    Dictionary<string, byte[]> dic = new Dictionary<string, byte[]>();
        //    SetRepositoryBehavior(key, repository, dic);

        //    //act
        //    long now = DateTime.UtcNow.Ticks;
        //    HealthCount generated = new HealthCountService(repository, 0, 0).GenerateNewHealthCounter(key);

        //    HealthCount n = new HealthCount()
        //    {
        //        Failures = 0,
        //        Successes = 0,
        //        // StartedAt = now
        //    };

        //    //Assert
        //    Assert.Equal(n.Successes, generated.Successes);
        //    Assert.Equal(n.Failures, generated.Failures);
        //    //Assert.Equal(new DateTime( n.StartedAt).ToShortDateString(), new DateTime( generated.StartedAt).ToShortDateString());
        //}
    }
}
