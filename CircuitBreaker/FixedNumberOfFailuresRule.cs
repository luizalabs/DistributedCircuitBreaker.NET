using CircuitBreaker.Core;

namespace CircuitBreaker
{
    public class FixedNumberOfFailuresRule : IRule
    {
        private readonly int _exceptionsAllowedBeforeBreaking;

        /// <summary>
        /// Initializes a rule that mesures if HealhCount failures is higher than the threshold
        /// </summary>
        /// <param name="_failuresThreshold">The failures number that should cause the CB to be Open</param>
        public FixedNumberOfFailuresRule(int exceptionsAllowedBeforeBreaking)
        {
            _exceptionsAllowedBeforeBreaking = exceptionsAllowedBeforeBreaking;
        }

        public bool ValidateRule(HealthCount healthCount)
        {
            if (healthCount.Failures > _exceptionsAllowedBeforeBreaking)
                return true;

            return false;
        }
    }
}
