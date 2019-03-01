using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitBreaker
{
    public class CircuitBreakerStateRepository : GenericRepository
    {
        const string StateKeySuffix = "-state";

        public CircuitBreakerStateRepository(IRepository repository): base(repository)
        {

        }

        public void SetState(string key,CircuitState state)
        {
            SetInt32(key + StateKeySuffix, (int)state);
        }

        public CircuitState GetState(string key)
        {
            return (CircuitState)GetInt32(key + StateKeySuffix);
        }
    }
}
