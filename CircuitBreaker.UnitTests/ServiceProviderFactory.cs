using DistributedCircuitBreaker.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace DistributedCircuitBreaker.UnitTests
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
            TimeSpan windowDuration = TimeSpan.FromSeconds(60);
            TimeSpan durationOfBreak = TimeSpan.FromSeconds(10);
            IDistributedCircuitBreakerRepository repository = Substitute.For<IDistributedCircuitBreakerRepository>();
            services.AddDistributedCircuitBreaker(options =>
            {
                options.DurationOfBreak = durationOfBreak;
                options.WindowDuration = windowDuration;
            });

            services.AddTransient(typeof(IDistributedCircuitBreakerRepository), serviceProvider => repository);

            return services.BuildServiceProvider();
        }

        public static void SetRepositoryBehavior(string key, IDistributedCircuitBreakerRepository repository, Dictionary<string, string> dic)
        {
            repository.GetString(key).ReturnsForAnyArgs(x => {
                var keyDic = x.Arg<string>();

                if (!dic.ContainsKey(keyDic))
                    return null;

                return dic[keyDic];
            });

            repository.WhenForAnyArgs(r => r.Set(key, 0)).Do(p =>
            {
                var value = p.Arg<int>();
                var keyDic = p.Arg<string>();
                dic[keyDic] = value.ToString();
            });

            repository.WhenForAnyArgs(r => r.Set(key, 0, TimeSpan.FromSeconds(0))).Do(p =>
            {
                var value = p.Arg<int>();
                var keyDic = p.Arg<string>();
                dic[keyDic] = value.ToString();
            });

            repository.WhenForAnyArgs(r => r.Increment(key)).Do(p =>
            {
                var keyDic = p.Arg<string>();
                int i = int.Parse(dic[keyDic]);
                i++;
                dic[keyDic] = i.ToString();
            });
        }
    }
}
