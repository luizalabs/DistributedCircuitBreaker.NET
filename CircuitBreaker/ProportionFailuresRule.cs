using DistributedCircuitBreaker.Core;

namespace DistributedCircuitBreaker
{
    public class ProportionFailuresRule : IRule
    {
        readonly decimal _failureThreshold;
        public ProportionFailuresRule(decimal failureThreshold)
        {
            _failureThreshold = failureThreshold;
        }

        public bool ValidateRule(HealthCount healthCount)
        {
            if ((healthCount.Failures / (decimal)healthCount.Total) > _failureThreshold)
                return true;

            return false;
        }
    }
}
