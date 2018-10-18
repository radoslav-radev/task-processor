using System;
using System.ComponentModel;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Master command that a task has failed.
    /// </summary>
    [Serializable]
    public sealed class TaskFailedMasterCommand : TaskCompletedMasterCommandBase
    {
        private string error;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskFailedMasterCommand"/> class.
        /// </summary>
        [Obsolete("Default constructor is required by XML serialization.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TaskFailedMasterCommand()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskFailedMasterCommand"/> class.
        /// </summary>
        /// <param name="taskId">The ID of the failed task.</param>
        /// <param name="taskProcessorId">The ID of the task processor that has been executing the task.</param>
        /// <param name="timestampUtc">When the task has failed, in UTC.</param>
        /// <param name="error">Description of the error that occurred during task execution.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="error"/> is null or empty string.</exception>
        public TaskFailedMasterCommand(Guid taskId, Guid taskProcessorId, DateTime timestampUtc, string error)
            : base(taskId, taskProcessorId, timestampUtc)
        {
            this.Error = error;
        }

        #endregion Constructors

        #region Properties

        /// <inheritdoc />
        public override TaskStatus TaskStatus
        {
            get
            {
                return TaskStatus.Failed;
            }
        }

        /// <summary>
        /// Gets or sets description of the error that occurred during task execution.
        /// </summary>
        /// <value>Description of the error that occurred during task execution.</value>
        /// <exception cref="ArgumentNullException">Value is null or empty string.</exception>
        public string Error
        {
            get
            {
                return this.error;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this.error = value;
            }
        }

        #endregion Properties
    }
}