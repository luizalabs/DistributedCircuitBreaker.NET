using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitBreaker
{
    public interface IRepository
    {
        void Increment(string key);
        //
        // Summary:
        //     Gets a value with the given key.
        //
        // Parameters:
        //   key:
        //     A string identifying the requested value.
        //
        // Returns:
        //     The located value or null.
        byte[] Get(string key);

        string GetString(string key);

        void Set(string key, byte[] value);
    }
}
