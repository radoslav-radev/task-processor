using System;
using Radoslav.Timers;

namespace Radoslav.Common.UnitTests
{
    public sealed class FakeTimer : MockObject, ITimer
    {
        #region ITimer Members

        public event EventHandler Tick;

        public bool IsActive { get; private set; }

        public TimeSpan Interval { get; set; }

        public void Start()
        {
            this.RecordMethodCall();

            this.IsActive = true;
        }

        public void Stop()
        {
            this.RecordMethodCall();

            this.IsActive = false;
        }

        #endregion ITimer Members

        #region IDisposable Members

        public void Dispose()
        {
            this.IsActive = false;

            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members

        public void RaiseTick()
        {
            if (this.Tick != null)
            {
                this.Tick(this, EventArgs.Empty);
            }
        }
    }
}