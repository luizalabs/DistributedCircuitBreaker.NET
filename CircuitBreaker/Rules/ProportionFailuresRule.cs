using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitBreaker
{
    public class ProportionFailuresRule : IRule
    {
        readonly decimal _failureThreshold;
        public ProportionFailuresRule(decimal failureThreshold)
        {
            _failureThreshold = failureThreshold;
        }

        public bool ShouldOpenCircuitBreaker(HealthCount healthCount)
        {
            if ((healthCount.Failures / (decimal)healthCount.Total) > _failureThreshold)
                return true;

            return false;
        }
    }
}
