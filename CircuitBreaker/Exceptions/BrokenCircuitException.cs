using System;
using System.Runtime.Serialization;
#if NETSTANDARD2_0
using System.Runtime.Serialization;
#endif

namespace DistributedCircuitBreaker
{
//#if NETSTANDARD2_0

    [Serializable]
//#endif
    public class BrokenCircuitException : ExecutionRejectedException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrokenCircuitException"/> class.
        /// </summary>
        public BrokenCircuitException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrokenCircuitException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public BrokenCircuitException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrokenCircuitException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The inner exception.</param>
        public BrokenCircuitException(string message, Exception inner) : base(message, inner)
        {
        }

//#if NETSTANDARD2_0
        /// <summary>
        /// Initializes a new instance of the <see cref="BrokenCircuitException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected BrokenCircuitException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
//#endif
    }
}
