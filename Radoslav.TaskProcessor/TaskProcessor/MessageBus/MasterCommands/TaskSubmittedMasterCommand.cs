using System;
using System.ComponentModel;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Master command that a new task has been submitted to the task processor.
    /// </summary>
    [Serializable]
    public sealed class TaskSubmittedMasterCommand : IUniqueMessage
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskSubmittedMasterCommand"/> class.
        /// </summary>
        [Obsolete("Default constructor is required by XML serialization.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TaskSubmittedMasterCommand()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskSubmittedMasterCommand"/> class.
        /// </summary>
        /// <param name="taskId">The ID of the requested task.</param>
        public TaskSubmittedMasterCommand(Guid taskId)
        {
            this.TaskId = taskId;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the ID of the requested task.
        /// </summary>
        /// <value>The ID of the requested task.</value>
        public Guid TaskId { get; set; }

        #endregion Properties

        #region IUniqueMessage Members

        /// <inheritdoc />
        public string MessageUniqueId
        {
            get
            {
                return this.TaskId.ToString();
            }
        }

        #endregion IUniqueMessage Members
    }
}