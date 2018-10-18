namespace Radoslav.TaskProcessor.Model
{
    /// <summary>
    /// Enumeration for a task processor state.
    /// </summary>
    public enum TaskProcessorState
    {
        /// <summary>
        /// The task processor is not active, i.e. it has not been started or it has been stopped.
        /// </summary>
        Inactive,

        /// <summary>
        /// The task processor is active and ready to process tasks.
        /// </summary>
        Active,

        /// <summary>
        /// A stop request has been received by the task processor, but there are still running tasks.
        /// Task processor will ignore future task requests and after the running tasks are complete, it will go to <see cref="Inactive"/> state.
        /// </summary>
        Stopping,

        /// <summary>
        /// The task processor has been disposed.
        /// </summary>
        Disposed
    }
}