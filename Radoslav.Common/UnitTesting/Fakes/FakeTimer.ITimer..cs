using System;
using Radoslav.Timers;

namespace Radoslav
{
    public sealed partial class FakeTimer : ITimer
    {
        #region ITimer Members

        public event EventHandler Tick
        {
            add
            {
                this.tickHandlers.Add(value);
            }

            remove
            {
                this.tickHandlers.Remove(value);
            }
        }

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
            this.RecordMethodCall();

            this.IsActive = false;

            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members
    }
}