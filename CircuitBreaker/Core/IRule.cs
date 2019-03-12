namespace DistributedCircuitBreaker.Core
{
    public interface IRule
    {
        bool ValidateRule(HealthCount healthCount);
    }
}
