using System;

namespace Radoslav
{
    public static partial class Helpers
    {
        /// <summary>
        /// Checks if an exception is critical, for example <see cref="StackOverflowException"/>, <see cref="OutOfMemoryException"/>, etc.
        /// </summary>
        /// <param name="exception">The exception to check if it is critical.</param>
        /// <returns>True if exception is critical; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="exception"/> is null.</exception>
        public static bool IsCritical(this Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            return (exception is BadImageFormatException) ||
                (exception is InvalidProgramException) ||
                (exception is OutOfMemoryException);
        }
    }
}