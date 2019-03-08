using CircuitBreaker.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace CircuitBreaker.UnitTests
{
    public static class ServiceProviderFactory
    {
        public static IServiceProvider ServiceProvider { get; internal set; }

        static ServiceProviderFactory()
        {
            ServiceProvider = BuildServiceProvider();
        }

        private static IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();
            TimeSpan windowDuration = TimeSpan.FromSeconds(1000);
            TimeSpan durationOfBreak = TimeSpan.FromSeconds(15);
            IRepository repository = Substitute.For<IRepository>();
            services.AddDistributedCircuitBreaker(options =>
            {
                options.DurationOfBreak = durationOfBreak;
                options.WindowDuration = windowDuration;
                options.Repository = repository;
            });

            services.AddTransient(typeof(IRepository),serviceProvider => repository);

            return services.BuildServiceProvider();
        }

        public static void SetRepositoryBehavior(string key, IRepository repository, Dictionary<string, byte[]> dic)
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

            repository.WhenForAnyArgs(r => r.Set(key, new byte[1], TimeSpan.FromSeconds(0))).Do(p =>
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
