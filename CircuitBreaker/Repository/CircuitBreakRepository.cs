using System;

namespace DistributedCircuitBreaker.Repository
{
    public class CircuitBreakRepository : ICircuitBreakRepository
    {
        protected IDistributedCircuitBreakerRepository _repository;
        public CircuitBreakRepository(IDistributedCircuitBreakerRepository repository)
        {
            _repository = repository ?? throw new ArgumentException("Repository must be configured");
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
            _repository.Set(key, value, timeSpan);
        }

        public bool KeyExists(string key)
        {
            return _repository.GetString(key) != null;
        }

        public void Increment(string key)
        {
            _repository.Increment(key);
        }

        public void Remove(string key)
        {
            _repository.Remove(key);
        }
    }
}
