using System;
using Radoslav.Redis;

namespace Radoslav.DateTimeProvider.Redis
{
    /// <summary>
    /// An implementation of <see cref="IDateTimeProvider"/> that retrieves the current date and time from Redis.
    /// </summary>
    public sealed class RedisDateTimeProvider : IDateTimeProvider
    {
        private readonly IRedisProvider provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisDateTimeProvider"/> class.
        /// </summary>
        /// <param name="provider">The Redis provider to use.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="provider"/> is null.</exception>
        public RedisDateTimeProvider(IRedisProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            this.provider = provider;
        }

        #region IDateTimeProvider Members

        /// <inheritdoc />
        public DateTime UtcNow
        {
            get
            {
                return this.provider.GetServerDateTimeUtc();
            }
        }

        #endregion IDateTimeProvider Members
    }
}