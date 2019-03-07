using System;

namespace CircuitBreaker.Core
{
    [Serializable]
    public class HealthCount 
    {
        public int Successes { get; set; }
        public int Failures { get; set; }
        public int Total { get { return Successes + Failures; } }
    }
}