using System;
using System.ComponentModel;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Master command that a task processor has been registered.
    /// </summary>
    [Serializable]
    public sealed class TaskProcessorRegisteredMasterCommand : IUniqueMessage
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskProcessorRegisteredMasterCommand"/> class.
        /// </summary>
        [Obsolete("Default constructor is required by XML serialization.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TaskProcessorRegisteredMasterCommand()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskProcessorRegisteredMasterCommand"/> class.
        /// </summary>
        /// <param name="taskProcessorId">The ID of the task processor that has been registered.</param>
        public TaskProcessorRegisteredMasterCommand(Guid taskProcessorId)
        {
            this.TaskProcessorId = taskProcessorId;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the ID of the task processor that has been registered.
        /// </summary>
        /// <value>The ID of the task processor that has been registered.</value>
        public Guid TaskProcessorId { get; set; }

        #endregion Properties

        #region IUniqueMessage Members

        /// <inheritdoc />
        public string MessageUniqueId
        {
            get
            {
                return this.TaskProcessorId.ToString();
            }
        }

        #endregion IUniqueMessage Members
    }
}