using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitBreaker.DependencyInjection
{
    public class CircuitBreakerFactoryOptions
    {
        public IRepository Repository { get; set; }
        public TimeSpan WindowDuration { get; set; }
        public TimeSpan DurationOfBreak { get; set; }
    }
}
