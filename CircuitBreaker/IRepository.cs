namespace CircuitBreaker
{
    public interface IRepository
    {
        void Increment(string key);
        /// <summary>
        ///  Gets a value with the given key.
        /// </summary>
        /// <param name="key">A string identifying the requested value.</param>
        /// <returns>The located value or null.</returns>
        byte[] Get(string key);

        string GetString(string key);

        void Set(string key, byte[] value);
    }
}
