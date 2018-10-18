namespace Radoslav.TaskProcessor.Repository
{
    /// <summary>
    /// Defines basic functionality of a task processor repository.
    /// </summary>
    public interface ITaskProcessorRepository
    {
        /// <summary>
        /// Gets tasks repository.
        /// </summary>
        /// <value>The task repository.</value>
        ITaskRepository Tasks { get; }

        /// <summary>
        /// Gets the task runtime information repository.
        /// </summary>
        /// <value>The task runtime information repository.</value>
        ITaskRuntimeInfoRepository TaskRuntimeInfo { get; }

        /// <summary>
        /// Gets the task processor runtime information repository.
        /// </summary>
        /// <value>The task processor runtime information repository.</value>
        ITaskProcessorRuntimeInfoRepository TaskProcessorRuntimeInfo { get; }

        /// <summary>
        /// Gets the task summary repository.
        /// </summary>
        /// <value>The task summary repository.</value>
        ITaskSummaryRepository TaskSummary { get; }

        /// <summary>
        /// Gets the task job settings repository.
        /// </summary>
        /// <value>The task job settings repository.</value>
        ITaskJobSettingsRepository TaskJobSettings { get; }

        /// <summary>
        /// Gets the scheduled tasks repository.
        /// </summary>
        /// <value>The scheduled tasks repository.</value>
        IScheduledTaskRepository ScheduledTasks { get; }
    }
}