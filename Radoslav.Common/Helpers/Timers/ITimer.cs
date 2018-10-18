using System;

namespace Radoslav.Timers
{
    /// <summary>
    /// Basic functionality of a timer.
    /// </summary>
    public interface ITimer : IDisposable
    {
        /// <summary>
        /// An event to be raised when the time interval specified in the <see cref="Interval"/> property has elapsed.
        /// </summary>
        event EventHandler Tick;

        /// <summary>
        /// Gets a value indicating whether the timer is active.
        /// </summary>
        /// <value>Whether the timer is active.</value>
        bool IsActive { get; }

        /// <summary>
        /// Gets or sets the time interval between two timer <see cref="Tick"/> events.
        /// </summary>
        /// <value>The time interval between two timer <see cref="Tick"/> events.</value>
        /// <exception cref="ArgumentOutOfRangeException">Value is less than or equal to <see cref="TimeSpan.Zero"/>.</exception>
        TimeSpan Interval { get; set; }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <exception cref="InvalidOperationException"><see cref="Interval"/> property is not set.</exception>
        void Start();

        /// <summary>
        /// Stops the timer.
        /// </summary>
        void Stop();
    }
}