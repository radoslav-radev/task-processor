namespace Radoslav.TaskProcessor.Model
{
    /// <summary>
    /// Enumeration for task status.
    /// </summary>
    public enum TaskStatus
    {
        /// <summary>
        /// The task has been requested but is not started yet.
        /// </summary>
        Pending,

        /// <summary>
        /// The task has been started by a task processor and is running.
        /// </summary>
        InProgress,

        /// <summary>
        /// The task has been canceled by a client.
        /// </summary>
        Canceled,

        /// <summary>
        /// The task execution has failed because of an unhandled exception during execution.
        /// </summary>
        Failed,

        /// <summary>
        /// The task has completed successfully.
        /// </summary>
        Success
    }
}