using System;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Facade
{
    /// <summary>
    /// An implementation of the <see cref="ISubmitTaskSession"/>.
    /// </summary>
    internal sealed class SubmitTaskSession : ISubmitTaskSession
    {
        private readonly TaskProcessorFacade facade;
        private readonly Guid taskId = Guid.NewGuid();

        /// <summary>
        /// Initializes a new instance of the <see cref="SubmitTaskSession"/> class.
        /// </summary>
        /// <param name="facade">The <see cref="ITaskProcessorFacade"/> on which the <see cref="ITaskProcessorFacade.CreateSubmitTaskSession"/> method was called.</param>
        internal SubmitTaskSession(TaskProcessorFacade facade)
        {
            this.facade = facade;
        }

        #region ISubmitTaskSession Members

        /// <inheritdoc />
        public Guid TaskId
        {
            get { return this.taskId; }
        }

        /// <inheritdoc />
        public void Complete(ITask task)
        {
            this.facade.SubmitTask(this.taskId, task, null, TaskPriority.Normal);
        }

        #endregion ISubmitTaskSession Members
    }
}