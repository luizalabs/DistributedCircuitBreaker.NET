using System;

namespace CircuitBreaker
{
    public class GenericRepository
    {
        protected IRepository _repository;
        public GenericRepository(IRepository repository)
        {
            _repository = repository;
        }
        public int GetInt32(string key)
        {
            var str = _repository.GetString(key);
            return int.Parse(str);
        }

        public void SetInt32(string key, int value)
        {
            _repository.Set(key, BitConverter.GetBytes(value));
        }

        public long GetInt64(string key)
        {
            var str = _repository.GetString(key);
            return long.Parse(str);
        }

        public void SetInt64(string key, long value)
        {
            _repository.Set(key, BitConverter.GetBytes(value));
        }

        public bool KeyExists(string key)
        {
            return _repository.GetString(key) != null;
        }

        public void Increment(string key)
        {
            _repository.Increment(key);
        }
    }

    public class HealthCountRepository : GenericRepository
    {
        const string SuccessCountKeySuffix = "-success";
        const string FailureCountKeySuffix = "-failure";
        const string StartedAtCountKeySuffix = "-startedAt";

        public HealthCountRepository(IRepository repository) : base(repository)
        {
           
        }

        public HealthCount Get(string key)
        {
            if (KeyExists(key + StartedAtCountKeySuffix) == false)
                return GenerateNewCounters(key);

            return new HealthCount()
            {
                Successes = GetInt32(key + SuccessCountKeySuffix),  
                Failures = GetInt32(key + FailureCountKeySuffix), 
                StartedAt = GetInt64(key + StartedAtCountKeySuffix)
            };        
        }

        private HealthCount GenerateNewCounters(string key)
        {
            _repository.Set(key + SuccessCountKeySuffix, BitConverter.GetBytes(0));
            _repository.Set(key + FailureCountKeySuffix, BitConverter.GetBytes(0));
            var now = DateTime.UtcNow.Ticks;
            _repository.Set(key + StartedAtCountKeySuffix, BitConverter.GetBytes(now));
            return new HealthCount() { Failures = 0, Successes = 0, StartedAt = now };
        }

        public void ClearCounters(string key)
        {
            SetInt32(key + SuccessCountKeySuffix, 0);
            SetInt32(key + FailureCountKeySuffix, 0);
        }

        public void IncrementSuccess(string key)
        {
            _repository.Increment(key + SuccessCountKeySuffix);
        }

        public void IncrementFailure(string key)
        {
            _repository.Increment(key + FailureCountKeySuffix);
        }

        public void SetStartedAt(string key, long startedAt)
        {
            SetInt64(key + StartedAtCountKeySuffix, startedAt);
        }
    }
}
