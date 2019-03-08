using System;
using System.Collections.Generic;

namespace CircuitBreaker.Core
{
    public interface ICircuitBreakerFactory
    {
        CircuitBreaker Create(string key, List<IRule> rules);
        CircuitBreaker Create(string key, int exceptionsAllowedBeforeBreaking, decimal failureRateAllowed);
        CircuitBreaker Create(string key, int exceptionsAllowedBeforeBreaking);
    }
}