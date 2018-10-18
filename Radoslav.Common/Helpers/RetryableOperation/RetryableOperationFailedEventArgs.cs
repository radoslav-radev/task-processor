using System;

namespace Radoslav.Retryable
{
    /// <summary>Provides data for the <see cref="RetryableOperation.RetryFailed"/> event.</summary>
    public sealed class RetryableOperationFailedEventArgs : EventArgs
    {
        private readonly int failedRetriesCount;
        private readonly Exception error;

        internal RetryableOperationFailedEventArgs(int failedRetriesCount, Exception error)
        {
            this.failedRetriesCount = failedRetriesCount;
            this.error = error;
        }

        /// <summary>Gets how many times the operation has failed until now.</summary>
        /// <value>How many times the operation has failed until now.</value>
        public int FailedRetriesCount
        {
            get { return this.failedRetriesCount; }
        }

        /// <summary>Gets the exception from the last attempt to execute the operation.</summary>
        /// <value>The exception from the last attempt to execute the operation.</value>
        public Exception Error
        {
            get { return this.error; }
        }
    }
}