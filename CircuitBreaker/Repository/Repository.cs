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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Returns the value of the key. If the key does not exist returns zero</returns>
        public int GetInt32(string key)
        {
            if (KeyExists(key))
                return int.Parse(_repository.GetString(key));
            else
                return default(int);
        }

        public void SetInt32(string key, int value, TimeSpan timeSpan)
        {
            _repository.Set(key, BitConverter.GetBytes(value), timeSpan);
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
