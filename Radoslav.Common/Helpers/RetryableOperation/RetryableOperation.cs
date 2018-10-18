using System;
using System.Threading;
using Radoslav.Retryable.DelayStrategy;

namespace Radoslav.Retryable
{
    /// <include file='..\Documentation\RetryableOperation.xml' path='Documentation/Class[@Name="RetryableOperation"]/*'/>
    public sealed partial class RetryableOperation : IDisposable
    {
        /// <summary>
        /// Finalizes an instance of the <see cref="RetryableOperation"/> class.
        /// </summary>
        ~RetryableOperation()
        {
            this.Dispose();
        }

        /// <summary>
        /// An event that is raised when the operation must be executed.
        /// </summary>
        public event EventHandler<RetryableOperationEventArgs> ExecuteOperation;

        /// <summary>
        /// An event that is raised when a retry attempt has failed.
        /// </summary>
        public event EventHandler<RetryableOperationFailedEventArgs> RetryFailed;

        /// <summary>
        /// Tries to execute the operation provided by the <see cref="ExecuteOperation"/> event several times.
        /// </summary>
        /// <param name="maxRetryAttempts">The maximum number of allowed retry attempts.</param>
        /// <param name="delayStrategy">Strategy to provide delay between two retry attempts.</param>
        /// <returns>True if operation was executed successfully before the maximum number of retry attempts is reached; otherwise, false.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="maxRetryAttempts"/> is less than or equal to zero.</exception>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="delayStrategy"/> is null.</exception>
        /// <exception cref="InvalidOperationException">There is no subscription for the <see cref="ExecuteOperation"/> event.</exception>
        /// <remarks>
        /// Tries to execute the operation by raising the <see cref="ExecuteOperation"/> event.
        /// If it fails, a new attempt is made after delay provided by the specified delay strategy.
        /// When the first time operation succeeds method returns true.
        /// If the maximum number of attempts is reached, method returns false.
        /// </remarks>
        public bool Execute(int maxRetryAttempts, IDelayStrategy delayStrategy)
        {
            if (maxRetryAttempts <= 0)
            {
                throw new ArgumentOutOfRangeException("maxRetryAttempts", maxRetryAttempts, "Max retry attempts must be a positive number.");
            }

            if (delayStrategy == null)
            {
                throw new ArgumentNullException("delayStrategy");
            }

            if (this.ExecuteOperation == null)
            {
                throw new InvalidOperationException("Execute operation is not specified.");
            }

            int failedRetriesCount = 0;

            Exception lastError;

            EventHandler<RetryableOperationFailedEventArgs> failedEventHandler;

            while (true)
            {
                lastError = null;

                try
                {
                    RetryableOperationEventArgs args = new RetryableOperationEventArgs(failedRetriesCount)
                    {
                        Success = true
                    };

                    this.ExecuteOperation(this, args);

                    if (args.Success)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.IsCritical())
                    {
                        throw;
                    }

                    lastError = ex;
                }

                failedRetriesCount++;

                if (failedRetriesCount == maxRetryAttempts)
                {
                    return false;
                }

                failedEventHandler = this.RetryFailed;

                if (failedEventHandler != null)
                {
                    failedEventHandler(this, new RetryableOperationFailedEventArgs(failedRetriesCount, lastError));
                }

                var delay = delayStrategy.NextDelay();

                Thread.Sleep(delay);
            }
        }

        #region IDisposable Members

        /// <inheritdoc />
        public void Dispose()
        {
            if (this.ExecuteOperation != null)
            {
                foreach (EventHandler<RetryableOperationEventArgs> handler in this.ExecuteOperation.GetInvocationList())
                {
                    this.ExecuteOperation -= handler;
                }
            }

            if (this.RetryFailed != null)
            {
                foreach (EventHandler<RetryableOperationFailedEventArgs> handler in this.RetryFailed.GetInvocationList())
                {
                    this.RetryFailed -= handler;
                }
            }

            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members
    }
}