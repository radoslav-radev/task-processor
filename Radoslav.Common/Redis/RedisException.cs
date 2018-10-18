using System;
using System.Runtime.Serialization;

namespace Radoslav.Redis
{
    /// <summary>
    /// Exception thrown by an operation concerning Redis.
    /// </summary>
    [Serializable]
    public sealed class RedisException : Exception
    {
        /// <inheritdoc />
        public RedisException()
        {
        }

        /// <inheritdoc />
        public RedisException(string message)
            : base(message)
        {
        }

        /// <inheritdoc />
        public RedisException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        private RedisException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}