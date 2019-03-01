namespace CircuitBreaker
{
    public class FixedNumberOfFailuresRule : IRule
    {
        private readonly int _failuresThreshold;

        /// <summary>
        /// Initializes a rule that mesures if HealhCount failures is higher than the threshold
        /// </summary>
        /// <param name="_failuresThreshold">The failures number that should cause the CB to be Open</param>
        public FixedNumberOfFailuresRule(int failuresThreshold)
        {
            _failuresThreshold = failuresThreshold;
        }

        public bool ShouldOpenCircuitBreaker(HealthCount healthCount)
        {
            if (healthCount.Failures > _failuresThreshold)
                return true;

            return false;
        }
    }
}
