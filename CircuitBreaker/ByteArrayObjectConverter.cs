using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace CircuitBreaker
{
    internal static class ByteArrayObjectConverter
    {
        public static byte[] ObjectToByteArray(object value)
        {
            if (value == null)
                return default(byte[]);

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, value);
                return memoryStream.ToArray();
            }
        }

        public static object ByteArrayToObject(byte[] value)
        {
            if (value.Length > 0)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream memoryStream = new MemoryStream(value))
                {
                    return binaryFormatter.Deserialize(memoryStream);
                }
            }

            return default(object);
        }
    }
}
