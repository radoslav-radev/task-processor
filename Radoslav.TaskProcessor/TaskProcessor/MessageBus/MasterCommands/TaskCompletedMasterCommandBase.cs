using System;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Basic class for master commands for tasks that completed successfully, failed or have been canceled.
    /// </summary>
    [Serializable]
    public abstract class TaskCompletedMasterCommandBase : IUniqueMessage
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskCompletedMasterCommandBase"/> class.
        /// </summary>
        protected TaskCompletedMasterCommandBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskCompletedMasterCommandBase"/> class.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="taskProcessorId">The ID of the task processor that has been executing the task.</param>
        /// <param name="timestampUtc">When the task has completed, in UTC.</param>
        protected TaskCompletedMasterCommandBase(Guid taskId, Guid taskProcessorId, DateTime timestampUtc)
        {
            this.TaskId = taskId;
            this.TaskProcessorId = taskProcessorId;
            this.TimestampUtc = timestampUtc;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the ID of the completed task.
        /// </summary>
        /// <value>The ID of the completed task.</value>
        public Guid TaskId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the task processor that has completed the task.
        /// </summary>
        /// <value>The ID of the task processor that has completed the task.</value>
        public Guid TaskProcessorId { get; set; }

        /// <summary>
        /// Gets the status of the task (how the task completed - successfully, with error or canceled).
        /// </summary>
        /// <value>The status of the task (how the task completed - successfully, with error or canceled)..</value>
        public abstract TaskStatus TaskStatus { get; }

        /// <summary>
        /// Gets or sets when the task has completed, in UTC.
        /// </summary>
        /// <value>When the task has completed in UTC.</value>
        public DateTime TimestampUtc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the task processor that has completed the task is stopping.
        /// </summary>
        /// <value>Whether the task processor that has completed the task is stopping.</value>
        public bool IsTaskProcessorStopping { get; set; }

        #endregion Properties

        #region IUniqueMessage Members

        /// <inheritdoc />
        public string MessageUniqueId
        {
            get
            {
                // No need to include task status because it depends of the class.
                return string.Join("$", this.TaskId, this.TaskProcessorId);
            }
        }

        #endregion IUniqueMessage Members
    }
}