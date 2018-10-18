namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Enumeration for the message bus channels supported by the task processor message bus.
    /// </summary>
    public enum MessageBusChannel
    {
        /// <summary>
        /// No channel.
        /// </summary>
        None,

        /// <summary>
        /// Channel for task processor state changes.
        /// </summary>
        TaskProcessorState,

        /// <summary>
        /// Channel for making a task processor master or slave.
        /// </summary>
        MasterModeChangeRequest,

        /// <summary>
        /// Channel to notify that a task processor has become master or slave.
        /// </summary>
        MasterModeChanged,

        /// <summary>
        /// Channel to notify that a task processor configuration has been changed.
        /// </summary>
        ConfigurationChanged,

        /// <summary>
        /// Channel used to request a task processor to shut down.
        /// </summary>
        StopTaskProcessor,

        /// <summary>
        /// Channel used to notify that a new task has been submitted by a user.
        /// </summary>
        TaskSubmitted,

        /// <summary>
        /// Channel used to notify that a task has been assigned to a task processor by the master task processor.
        /// </summary>
        TaskAssigned,

        /// <summary>
        /// Channel used to notify that a task has been started by a task processor.
        /// </summary>
        TaskStarted,

        /// <summary>
        /// Channel used to notify a task progress.
        /// </summary>
        TaskProgress,

        /// <summary>
        /// Channel used to notify that a client has requested the task processor to cancel a task.
        /// </summary>
        TaskCancelRequest,

        /// <summary>
        /// Channel used to notify that a task has been canceled by the task processor executing it.
        /// </summary>
        TaskCancelCompleted,

        /// <summary>
        /// Channel used to notify that an error occurred during task execution.
        /// </summary>
        TaskFailed,

        /// <summary>
        /// Channel used to notify that a task execution completed successfully.
        /// </summary>
        TaskCompleted,

        /// <summary>
        /// Channel used to request a task processor to start or continue performance monitoring.
        /// </summary>
        PerformanceMonitoringRequest,

        /// <summary>
        /// Channel used by a task processor to send a performance report.
        /// </summary>
        PerformanceReport
    }
}