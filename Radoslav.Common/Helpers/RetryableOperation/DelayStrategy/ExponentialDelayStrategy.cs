using System;

namespace Radoslav.Retryable.DelayStrategy
{
    /// <summary>
    /// An implementation of <see cref="IDelayStrategy"/> that doubles the delay between each two retries.
    /// </summary>
    public sealed class ExponentialDelayStrategy : IDelayStrategy
    {
        private readonly TimeSpan initialDelay;

        private TimeSpan currentDelay;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExponentialDelayStrategy"/> class.
        /// </summary>
        /// <param name="initialDelay">The initial (first) delay that will be doubled each time.</param>
        public ExponentialDelayStrategy(TimeSpan initialDelay)
        {
            if (initialDelay < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("initialDelay", initialDelay, "Initial delay must be positive.");
            }

            this.initialDelay = initialDelay;
        }

        /// <inheritdoc />
        public TimeSpan NextDelay()
        {
            if (this.currentDelay == TimeSpan.Zero)
            {
                this.currentDelay = this.initialDelay;
            }
            else
            {
                this.currentDelay = this.currentDelay.Multiply(2);
            }

            return this.currentDelay;
        }
    }
}