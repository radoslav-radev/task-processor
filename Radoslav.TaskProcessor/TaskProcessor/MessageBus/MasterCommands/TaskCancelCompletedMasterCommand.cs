using System;
using System.ComponentModel;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Master command that a task has been canceled by the executing task processor.
    /// </summary>
    [Serializable]
    public sealed class TaskCancelCompletedMasterCommand : TaskCompletedMasterCommandBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskCancelCompletedMasterCommand"/> class.
        /// </summary>
        [Obsolete("Default constructor is required by XML serialization.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TaskCancelCompletedMasterCommand()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskCancelCompletedMasterCommand"/> class.
        /// </summary>
        /// <param name="taskId">The ID of the canceled task.</param>
        /// <param name="taskProcessorId">The ID of the task processor that has canceled the task.</param>
        /// <param name="timestampUtc">When the task has been canceled, in UTC.</param>
        public TaskCancelCompletedMasterCommand(Guid taskId, Guid taskProcessorId, DateTime timestampUtc)
            : base(taskId, taskProcessorId, timestampUtc)
        {
        }

        #endregion Constructors

        /// <inheritdoc />
        public override TaskStatus TaskStatus
        {
            get
            {
                return TaskStatus.Canceled;
            }
        }
    }
}