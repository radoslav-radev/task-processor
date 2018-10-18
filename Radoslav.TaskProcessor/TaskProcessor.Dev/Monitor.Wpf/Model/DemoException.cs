using System;
using System.Runtime.Serialization;

namespace Radoslav.TaskProcessor.Model
{
    [Serializable]
    public sealed class DemoException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DemoException"/> class.
        /// </summary>
        public DemoException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DemoException"/> class with a specified error message.
        /// </summary>
        /// <param name="message"> A message that describes the error.</param>
        public DemoException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DemoException"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message"> A message that describes the error.</param>
        /// <param name="ex">The exception that is the cause of the current exception, or a null if no inner exception is specified.</param>
        public DemoException(string message, Exception ex)
            : base(message, ex)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.Exception class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref=" SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        private DemoException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}