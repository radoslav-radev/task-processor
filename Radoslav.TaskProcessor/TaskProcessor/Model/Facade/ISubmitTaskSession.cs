using System;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Facade
{
    /// <summary>
    /// Basic functionality of a submit task session.
    /// </summary>
    public interface ISubmitTaskSession
    {
        /// <summary>
        /// Gets the ID reserved for the task that is being submitted.
        /// </summary>
        /// <value>The ID reserved for the task that is being submitted.</value>
        Guid TaskId { get; }

        /// <summary>
        /// Completes the submission of the task.
        /// </summary>
        /// <param name="task">The task to complete submission.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="task"/> is null.</exception>
        void Complete(ITask task);
    }
}