using System;
using System.Runtime.Serialization;

namespace Radoslav.Owin
{
    /// <summary>
    /// An exception indicating that OWIN has failed to start.
    /// </summary>
    [Serializable]
    public sealed class OwinFailedToStartException : Exception
    {
        /// <inheritdoc />
        public OwinFailedToStartException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OwinFailedToStartException"/> class.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public OwinFailedToStartException(Exception innerException)
            : base("OWIN failed to start.", innerException)
        {
        }

        /// <inheritdoc />
        public OwinFailedToStartException(string message)
            : base(message)
        {
        }

        /// <inheritdoc />
        public OwinFailedToStartException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        private OwinFailedToStartException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}