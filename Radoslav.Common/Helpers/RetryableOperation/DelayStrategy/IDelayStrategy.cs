using System;

namespace Radoslav.Retryable.DelayStrategy
{
    /// <summary>
    /// Interface for implementing a delay strategy for a <see cref="RetryableOperation"/>.
    /// </summary>
    public interface IDelayStrategy
    {
        /// <summary>
        /// Gets the next delay calculated by the strategy.
        /// </summary>
        /// <returns>The next delay calculated by the strategy.</returns>
        TimeSpan NextDelay();
    }
}