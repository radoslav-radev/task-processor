namespace Radoslav
{
    /// <summary>
    /// Basic functionality of an application killer.
    /// </summary>
    /// <remarks>
    /// The implementations of this interface are supposed to immediately terminate the current application.
    /// </remarks>
    public interface IApplicationKiller
    {
        /// <summary>
        /// Immediately terminates the current application.
        /// </summary>
        void Kill();
    }
}