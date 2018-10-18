using System;
using System.Diagnostics;
using System.Timers;

namespace Radoslav.Timers
{
    /// <summary>
    /// An implementation of <see cref="ITimer"/> that uses <see cref="Timer"/>.
    /// </summary>
    public sealed class TimersTimer : ITimer, IDisposable
    {
        private readonly Timer timer = new Timer();

        private TimeSpan interval;

        #region Destructor

        /// <summary>
        /// Finalizes an instance of the <see cref="TimersTimer"/> class.
        /// </summary>
        ~TimersTimer()
        {
            Trace.WriteLine("ENTER: Destructing {0} ...".FormatInvariant(this.GetType().Name));

            this.Dispose();

            Trace.WriteLine("EXIT: {0} destructed.".FormatInvariant(this.GetType().Name));
        }

        #endregion Destructor

        #region ITimer Members

        /// <inheritdoc />
        public event EventHandler Tick;

        /// <inheritdoc />
        public bool IsActive
        {
            get
            {
                return this.timer.Enabled;
            }
        }

        /// <inheritdoc />
        public TimeSpan Interval
        {
            get
            {
                return this.interval;
            }

            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException("value", "Value must be positive.");
                }

                this.interval = value;

                this.timer.Interval = value.TotalMilliseconds;
            }
        }

        /// <inheritdoc />
        public void Start()
        {
            Trace.WriteLine("ENTER: Starting {0} ...".FormatInvariant(this.GetType().Name));

            if (this.IsActive)
            {
                Trace.WriteLine("EXIT: {0} is already started.".FormatInvariant(this.GetType().Name));

                return;
            }

            if (this.Interval == TimeSpan.Zero)
            {
                throw new InvalidOperationException("Timer interval is not set.");
            }

            this.timer.Elapsed += this.OnTimerElapsed;

            this.timer.Start();

            while (!this.timer.Enabled)
            {
                // Wait until timer is started, because operation is asynchronous.
            }

            Trace.WriteLine("EXIT: {0} started.".FormatInvariant(this.GetType().Name));
        }

        /// <inheritdoc />
        public void Stop()
        {
            Trace.WriteLine("ENTER: Stopping {0} ...".FormatInvariant(this.GetType().Name));

            if (!this.IsActive)
            {
                Trace.WriteLine("EXIT: {0} is not started.".FormatInvariant(this.GetType().Name));

                return;
            }

            this.timer.Stop();

            this.timer.Elapsed -= this.OnTimerElapsed;

            while (this.timer.Enabled)
            {
                // Wait until timer is stopped, because operation is asynchronous.
            }

            Trace.WriteLine("EXIT: {0} stopped.".FormatInvariant(this.GetType().Name));
        }

        #endregion ITimer Members

        #region IDisposable Members

        /// <inheritdoc />
        public void Dispose()
        {
            Trace.WriteLine("ENTER: Disposing {0} ...".FormatInvariant(this.GetType().Name));

            this.timer.Dispose();

            GC.SuppressFinalize(this);

            Trace.WriteLine("EXIT: {0} disposed.".FormatInvariant(this.GetType().Name));
        }

        #endregion IDisposable Members

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (this.IsActive)
            {
                EventHandler tickEventHandler = this.Tick;

                if (tickEventHandler != null)
                {
                    tickEventHandler(this, EventArgs.Empty);
                }
            }
        }
    }
}