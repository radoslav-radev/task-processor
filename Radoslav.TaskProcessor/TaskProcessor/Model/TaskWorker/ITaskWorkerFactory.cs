using System;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.TaskWorker
{
    /// <summary>
    /// Basic functionality for a task worker factory.
    /// </summary>
    public interface ITaskWorkerFactory
    {
        /// <summary>
        /// Creates a task worker from a task.
        /// </summary>
        /// <param name="task">The task for which to create an executable task.</param>
        /// <returns>Task worker for the specified task.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="task"/> is null.</exception>
        /// <exception cref="NotSupportedException">Parameter <paramref name="task"/> is of type that is not supported.</exception>
        ITaskWorker CreateTaskWorker(ITask task);
    }
}