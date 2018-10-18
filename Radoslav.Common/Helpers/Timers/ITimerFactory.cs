namespace Radoslav.Timers
{
    /// <summary>
    /// Basic implementation of an <see cref="ITimer"/> factory.
    /// </summary>
    public interface ITimerFactory
    {
        /// <summary>
        /// Creates a new timer.
        /// </summary>
        /// <returns>A new timer.</returns>
        ITimer CreateTimer();
    }
}