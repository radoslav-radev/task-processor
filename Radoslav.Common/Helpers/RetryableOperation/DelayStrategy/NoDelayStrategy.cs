using System;

namespace Radoslav.Retryable.DelayStrategy
{
    /// <summary>
    /// An implementation of <see cref="IDelayStrategy"/> that returns <see cref="TimeSpan.Zero"/> as delay between each two retries.
    /// </summary>
    public sealed class NoDelayStrategy : IDelayStrategy
    {
        /// <inheritdoc />
        public TimeSpan NextDelay()
        {
            return TimeSpan.Zero;
        }
    }
}