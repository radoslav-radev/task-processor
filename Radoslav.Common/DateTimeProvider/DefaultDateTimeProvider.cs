using System;

namespace Radoslav.DateTimeProvider
{
    /// <summary>
    /// Default implementation of <see cref="IDateTimeProvider" /> that uses <see cref="DateTime" />.<see cref="DateTime.Now" />.
    /// </summary>
    public sealed class DefaultDateTimeProvider : IDateTimeProvider
    {
        #region IDateTimeProvider Members

        /// <inheritdoc />
        public DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }

        #endregion IDateTimeProvider Members
    }
}