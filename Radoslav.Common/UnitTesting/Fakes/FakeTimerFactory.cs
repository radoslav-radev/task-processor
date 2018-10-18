using System.Collections.Generic;
using Radoslav.Timers;

namespace Radoslav
{
    public sealed class FakeTimerFactory : MockObject, ITimerFactory
    {
        private readonly List<FakeTimer> createdTimers = new List<FakeTimer>();

        public ICollection<FakeTimer> CreatedTimers
        {
            get { return this.createdTimers; }
        }

        #region ITimerFactory Members

        public ITimer CreateTimer()
        {
            this.RecordMethodCall();

            FakeTimer result = new FakeTimer();

            this.createdTimers.Add(result);

            return result;
        }

        #endregion ITimerFactory Members
    }
}