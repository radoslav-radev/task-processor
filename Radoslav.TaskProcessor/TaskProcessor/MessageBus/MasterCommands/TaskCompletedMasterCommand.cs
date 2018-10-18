using System;
using System.ComponentModel;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Master command that a task has completed successfully.
    /// </summary>
    [Serializable]
    public sealed class TaskCompletedMasterCommand : TaskCompletedMasterCommandBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskCompletedMasterCommand"/> class.
        /// </summary>
        [Obsolete("Default constructor is required by XML serialization.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TaskCompletedMasterCommand()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskCompletedMasterCommand"/> class.
        /// </summary>
        /// <param name="taskId">The ID of the completed task.</param>
        /// <param name="taskProcessorId">The ID of the task processor that has completed the task.</param>
        /// <param name="timestampUtc">When the task has completed, in UTC.</param>
        public TaskCompletedMasterCommand(Guid taskId, Guid taskProcessorId, DateTime timestampUtc)
            : base(taskId, taskProcessorId, timestampUtc)
        {
        }

        #endregion Constructors

        #region Properties

        /// <inheritdoc />
        public override TaskStatus TaskStatus
        {
            get
            {
                return TaskStatus.Success;
            }
        }

        /// <summary>
        /// Gets or sets the total processor time used during task execution.
        /// </summary>
        /// <value>The total processor time used during task execution.</value>
        public TimeSpan TotalCpuTime { get; set; }

        /// <summary>
        /// Gets or sets the total processor time (in ticks) used during task execution.
        /// </summary>
        /// <remarks>This property exists because the XML serializer does not support <see cref="TimeSpan"/>.</remarks>
        /// <value>The total processor time used during task execution, in ticks.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public long TotalCpuTimeInTicks
        {
            get
            {
                return this.TotalCpuTime.Ticks;
            }

            set
            {
                this.TotalCpuTime = new TimeSpan(value);
            }
        }

        #endregion Properties
    }
}