namespace DistributedCircuitBreaker.Core
{
    /// <summary>
    /// Describes the possible states the circuit of a CircuitBreaker may be in.
    /// </summary>
    public enum CircuitState
    {
        /// <summary>
        /// Closed - When the circuit is closed.  Execution of actions is allowed.
        /// </summary>
        Closed,
        /// <summary>
        /// Open - When the automated controller has opened the circuit (typically due to some failure threshold being exceeded by recent actions). Execution of actions is blocked.
        /// </summary>
        Open
    }
}