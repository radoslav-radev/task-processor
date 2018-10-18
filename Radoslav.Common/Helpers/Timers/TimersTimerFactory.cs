namespace Radoslav.Timers
{
    /// <summary>
    /// An implementation of <see cref="ITimerFactory"/> that creates <see cref="TimersTimer"/> instances.
    /// </summary>
    public sealed class TimersTimerFactory : ITimerFactory
    {
        #region ITimerFactory Members

        /// <inheritdoc />
        public ITimer CreateTimer()
        {
            return new TimersTimer();
        }

        #endregion ITimerFactory Members
    }
}