using System;
using System.Diagnostics;
using System.Threading;

namespace Radoslav.Timers
{
    /// <summary>
    /// An implementation of <see cref="ITimer"/> that uses <see cref="Timer"/>.
    /// </summary>
    public sealed class ThreadingTimer : ITimer, IDisposable
    {
        private readonly Timer timer;

        private TimeSpan interval;

        #region Constructor & Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadingTimer"/> class.
        /// </summary>
        public ThreadingTimer()
        {
            Trace.WriteLine("ENTER: Constructing {0} ...".FormatInvariant(this.GetType().Name));

            this.timer = new Timer(this.OnTimerTick);

            Trace.WriteLine("EXIT: {0} constructed.".FormatInvariant(this.GetType().Name));
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ThreadingTimer"/> class.
        /// </summary>
        ~ThreadingTimer()
        {
            Trace.WriteLine("ENTER: Destructing {0} ...".FormatInvariant(this.GetType().Name));

            this.Dispose();

            Trace.WriteLine("EXIT: {0} destructed.".FormatInvariant(this.GetType().Name));
        }

        #endregion Constructor & Destructor

        #region ITimer Members

        /// <inheritdoc />
        public event EventHandler Tick;

        /// <inheritdoc />
        public bool IsActive { get; private set; }

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

                if (this.IsActive)
                {
                    this.timer.Change(this.interval, this.interval);
                }
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

            if (this.interval == TimeSpan.Zero)
            {
                throw new InvalidOperationException("Time interval is not set.");
            }

            this.timer.Change(this.interval, this.interval);

            this.IsActive = true;

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

            this.timer.Change(Timeout.Infinite, Timeout.Infinite);

            this.IsActive = false;

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

        private void OnTimerTick(object target)
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