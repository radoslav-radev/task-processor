namespace Radoslav.Timers
{
    /// <summary>
    /// An implementation of <see cref="ITimerFactory"/> that creates <see cref="ThreadingTimer"/> instances.
    /// </summary>
    public sealed class ThreadingTimerFactory : ITimerFactory
    {
        #region ITimerFactory Members

        /// <inheritdoc />
        public ITimer CreateTimer()
        {
            return new ThreadingTimer();
        }

        #endregion ITimerFactory Members
    }
}