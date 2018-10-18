using System;

namespace Radoslav.Retryable.DelayStrategy
{
    /// <summary>
    /// An implementation of <see cref="IDelayStrategy"/> that uses the same delay between each two retries.
    /// </summary>
    public sealed class ConstantDelayStrategy : IDelayStrategy
    {
        private readonly TimeSpan delayBetweenRetries;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantDelayStrategy"/> class.
        /// </summary>
        /// <param name="delayBetweenRetries">The delay each between retries.</param>
        /// <exception cref="ArgumentOutOfRangeException">Parameter <paramref name="delayBetweenRetries"/> is less than <see cref="TimeSpan.Zero"/>.</exception>
        public ConstantDelayStrategy(TimeSpan delayBetweenRetries)
        {
            if (delayBetweenRetries < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("delayBetweenRetries", delayBetweenRetries, "Delay between retries must be positive.");
            }

            this.delayBetweenRetries = delayBetweenRetries;
        }

        /// <inheritdoc />
        public TimeSpan NextDelay()
        {
            return this.delayBetweenRetries;
        }
    }
}