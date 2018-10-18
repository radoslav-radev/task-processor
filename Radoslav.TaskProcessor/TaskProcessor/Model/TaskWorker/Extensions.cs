using System;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.TaskWorker
{
    /// <summary>
    /// Class for task worker extensions.
    /// </summary>
    public static class TaskWorkerExtensions
    {
        /// <summary>
        /// Starts a task.
        /// </summary>
        /// <remarks>This method should be called only once.</remarks>
        /// <param name="worker">The task worker to extend.</param>
        /// <param name="task">The task to be started by the task worker.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="worker"/> or <paramref name="task"/> is null.</exception>
        public static void StartTask(this ITaskWorker worker, ITask task)
        {
            if (worker == null)
            {
                throw new ArgumentNullException(nameof(worker));
            }

            worker.StartTask(task, null);
        }
    }
}