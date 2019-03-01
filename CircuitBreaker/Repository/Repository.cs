using System;

namespace CircuitBreaker.Repository
{
    public class CircuitBreakRepository : ICircuitBreakRepository
    {
        protected IRepository _repository;
        public CircuitBreakRepository(IRepository repository)
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
}
