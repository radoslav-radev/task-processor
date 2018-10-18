using System;
using Radoslav.DateTimeProvider;

namespace Radoslav
{
    public sealed class FakeDateTimeProvider : IDateTimeProvider
    {
        private DateTime? valueUtc;

        #region IDateTimeProvider Members

        public DateTime UtcNow
        {
            get
            {
                return this.valueUtc.GetValueOrDefault(() => DateTime.UtcNow);
            }

            set
            {
                this.valueUtc = value;
            }
        }

        #endregion IDateTimeProvider Members
    }
}