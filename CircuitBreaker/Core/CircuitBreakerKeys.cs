using System;

namespace DistributedCircuitBreaker.Core
{
    public class CircuitBreakerKeys
    {
        public CircuitBreakerKeys(string mainKey)
        {
            Key = mainKey;
        }

        const string SuccessCountKeySuffix = "-success";
        const string FailureCountKeySuffix = "-failure";
        const string StateKeySuffix = "-state";

        public string Key { get; set; }

        public string FailureCountKey
        {
            get
            {
                ValidateKey();
                return Key + FailureCountKeySuffix;
            }
        }

        public string SuccessCountKey
        {
            get
            {
                ValidateKey();
                return Key + SuccessCountKeySuffix;
            }
        }

        public string StateKey
        {
            get
            {
                ValidateKey();
                return Key + StateKeySuffix;
            }
        }

        private void ValidateKey()
        {
            if (string.IsNullOrEmpty(Key))
                throw new Exception("key not configured");
        }
    }
}
