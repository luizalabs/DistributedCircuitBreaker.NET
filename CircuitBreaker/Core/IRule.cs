namespace CircuitBreaker.Core
{
    public interface IRule
    {
        bool ValidateRule(HealthCount healthCount);
    }
}
