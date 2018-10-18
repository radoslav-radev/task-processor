using System;
using System.Runtime.Serialization;

namespace Radoslav.Diagnostics
{
    /// <summary>
    /// Class for native exceptions.
    /// </summary>
    [Serializable]
    public sealed class NativeException : Exception
    {
        /// <inheritdoc />
        public NativeException()
        {
        }

        /// <inheritdoc />
        public NativeException(string message)
            : base(message)
        {
        }

        /// <inheritdoc />
        public NativeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        private NativeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}