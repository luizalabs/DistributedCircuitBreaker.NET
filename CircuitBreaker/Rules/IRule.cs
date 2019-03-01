namespace CircuitBreaker
{
    public interface IRule
    {
        bool ShouldOpenCircuitBreaker(HealthCount healthCount);
    }
}
