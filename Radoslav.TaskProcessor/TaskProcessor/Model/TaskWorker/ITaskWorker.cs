using System;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.TaskWorker
{
    /// <summary>
    /// Basic functionality of a task worker, i.e. the class who should execute/process a task.
    /// </summary>
    public interface ITaskWorker
    {
        /// <summary>
        /// An event that is raised to report task progress.
        /// </summary>
        event EventHandler<TaskWorkerProgressEventArgs> ReportProgress;

        /// <summary>
        /// Starts a task.
        /// </summary>
        /// <remarks>
        /// <para>This method should be called only once.</para>
        /// <para>Parameter <paramref name="settings"/> could be null.</para>
        /// </remarks>
        /// <param name="task">The task to be started by the task worker.</param>
        /// <param name="settings">The task job settings associated with this task type.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="task"/> is null.</exception>
        void StartTask(ITask task, ITaskJobSettings settings);

        /// <summary>
        /// Cancels a task.
        /// </summary>
        /// <remarks>This method should be called only once.</remarks>
        void CancelTask();
    }
}