using System;

namespace Radoslav.Retryable
{
    /// <summary>Provides data for the <see cref="RetryableOperation.ExecuteOperation"/> event.</summary>
    public sealed class RetryableOperationEventArgs : EventArgs
    {
        private readonly int failedRetriesCount;

        internal RetryableOperationEventArgs(int failedRetriesCount)
        {
            this.failedRetriesCount = failedRetriesCount;
        }

        /// <summary>Gets how many times the operation has failed until now.</summary>
        /// <value>How many times the operation has failed until now.</value>
        public int FailedRetriesCount
        {
            get { return this.failedRetriesCount; }
        }

        /// <summary>Gets or sets a value indicating whether operation was executed successfully. </summary>
        /// <value>Whether the operation was executed successfully.</value>
        /// <remarks>Default value is <c>true</c> but if an exception occurs during execution of
        /// the <see cref="RetryableOperation.ExecuteOperation"/> event, it is automatically set to false.</remarks>
        public bool Success { get; set; }
    }
}