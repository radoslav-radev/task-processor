namespace Radoslav
{
    /// <summary>
    /// Enumeration to indicate the dispose state of an instance.
    /// </summary>
    public enum DisposeState
    {
        /// <summary>
        /// The instance is not disposed yet.
        /// </summary>
        None,

        /// <summary>
        /// The instance is currently disposing, but dispose process is not complete.
        /// </summary>
        Disposing,

        /// <summary>
        /// The instance has been disposed.
        /// </summary>
        Disposed
    }
}