using CircuitBreaker.Domain;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace CircuitBreaker.UnitTests
{
    public class HealthCountServiceTests
    {
        [Fact]
        public void ExecuteAction_ShouldGenerateClearHealthCount()
        {
            //arrange
            string key = "testKey";
            IRepository repository = Substitute.For<IRepository>();
            Dictionary<string, byte[]> dic = new Dictionary<string, byte[]>();
            SetRepositoryBehavior(key, repository, dic);

            //act
            long now = DateTime.UtcNow.Ticks;
            HealthCount generated = new HealthCountService(repository).GenerateNewHealthCounter(key);

            HealthCount n = new HealthCount()
            {
                Failures = 0,
                Successes = 0,
                StartedAt = now
            };

            //Assert
            Assert.Equal(n.Successes, generated.Successes);
            Assert.Equal(n.Failures, generated.Failures);
            Assert.Equal(new DateTime( n.StartedAt).ToShortDateString(), new DateTime( generated.StartedAt).ToShortDateString());
        }

        private static void SetRepositoryBehavior(string key, IRepository repository, Dictionary<string,byte[]> dic)
        {
            repository.GetString(key).ReturnsForAnyArgs(x => {
                var keyDic = x.Arg<string>();

                if (!dic.ContainsKey(keyDic))
                    return null;

                if (dic[keyDic].GetLength(0) <= 4)
                    return BitConverter.ToInt32(dic[keyDic]).ToString();
                else
                    return BitConverter.ToInt64(dic[keyDic]).ToString();
            });

            repository.WhenForAnyArgs(r => r.Set(key, new byte[1])).Do(p =>
            {
                var arr = p.Arg<byte[]>();
                var s = BitConverter.ToString(arr);
                var keyDic = p.Arg<string>();
                dic[keyDic] = arr;
            });

            repository.WhenForAnyArgs(r => r.Increment(key)).Do(p =>
            {
                var keyDic = p.Arg<string>();
                int i = BitConverter.ToInt32(dic[keyDic]);
                i++;
                dic[keyDic] = BitConverter.GetBytes(i);
            });
        }


    }
}
