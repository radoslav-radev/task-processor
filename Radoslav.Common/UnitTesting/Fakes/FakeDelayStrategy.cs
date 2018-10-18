using System;
using Radoslav.Retryable.DelayStrategy;

namespace Radoslav
{
    public sealed class FakeDelayStrategy : MockObject, IDelayStrategy
    {
        private TimeSpan nextDelay;

        #region IDelayStrategy Members

        public TimeSpan NextDelay()
        {
            this.RecordMethodCall();

            return this.nextDelay;
        }

        #endregion IDelayStrategy Members

        public void SetNextDelay(TimeSpan value)
        {
            this.nextDelay = value;
        }
    }
}