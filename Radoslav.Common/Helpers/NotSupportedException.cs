using System;
using System.Runtime.Serialization;

namespace Radoslav
{
    /// <include file='Documentation\NotSupportedException.xml' path='Documentation/Class[@Name="NotSupportedException"]/*'/>
    [Serializable]
    public sealed class NotSupportedException<T> : NotSupportedException
    {
        /// <inheritdoc />
        public NotSupportedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotSupportedException{T}"/> class with an error message containing the value that is not supported.
        /// </summary>
        /// <param name="value">The value that is not supported.</param>
        public NotSupportedException(T value)
            : base(NotSupportedException<T>.GetMessage(value))
        {
        }

        /// <inheritdoc />
        public NotSupportedException(string message)
            : base(message)
        {
        }

        /// <inheritdoc />
        public NotSupportedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        private NotSupportedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        private static string GetMessage(T value)
        {
            return "{0} '{1}' is not supported.".FormatInvariant(typeof(T), value);
        }
    }
}