using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Repository.Redis
{
    /// <summary>
    /// An implementation of <see cref="ITaskRuntimeInfo"/> suitable for Redis.
    /// </summary>
    /// <remarks>This class must be serializable to binary, XML, JSON and/or other formats.</remarks>
    [Serializable]
    [DataContract]
    public sealed class RedisTaskRuntimeInfo : ITaskRuntimeInfo
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTaskRuntimeInfo"/> class.
        /// </summary>
        [Obsolete("For XML serialization only.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public RedisTaskRuntimeInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTaskRuntimeInfo"/> class.
        /// </summary>
        /// <param name="taskId">The unique ID of the task.</param>
        /// <param name="taskType">The type of the task.</param>
        /// <param name="submittedUtc">When the task was submitted, in UTC.</param>
        /// <param name="status">The status of the task.</param>
        internal RedisTaskRuntimeInfo(Guid taskId, Type taskType, DateTime submittedUtc, TaskStatus status)
        {
            this.TaskId = taskId;
            this.TaskType = taskType;
            this.SubmittedUtc = submittedUtc;
            this.Status = status;
        }

        #endregion Constructors

        #region ITaskRuntimeInfo Members

        /// <inheritdoc />
        [DataMember]
        public Guid TaskId { get; set; }

        /// <inheritdoc />
        [XmlIgnore]
        [IgnoreDataMember]
        public Type TaskType { get; private set; }

        /// <inheritdoc />
        [DataMember]
        public string PollingQueue { get; set; }

        /// <inheritdoc />
        [DataMember]
        public TaskPriority Priority { get; set; }

        /// <inheritdoc />
        [DataMember]
        public DateTime SubmittedUtc { get; set; }

        /// <inheritdoc />
        [DataMember]
        public TaskStatus Status { get; set; }

        /// <inheritdoc />
        [DataMember]
        public Guid? TaskProcessorId { get; set; }

        /// <inheritdoc />
        [DataMember]
        public double Percentage { get; set; }

        /// <inheritdoc />
        [DataMember]
        public DateTime? StartedUtc { get; set; }

        /// <inheritdoc />
        [DataMember]
        public DateTime? CanceledUtc { get; set; }

        /// <inheritdoc />
        [DataMember]
        public DateTime? CompletedUtc { get; set; }

        /// <inheritdoc />
        [DataMember]
        public string Error { get; set; }

        #endregion ITaskRuntimeInfo Members

        /// <summary>
        /// Gets or sets the type of the task as string for serialization.
        /// </summary>
        /// <value>The type of the task as string.</value>
        [DataMember]
        public string TaskTypeName
        {
            get
            {
                return this.TaskType.AssemblyQualifiedName;
            }

            set
            {
                this.TaskType = Type.GetType(value, true);
            }
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.TaskId.GetHashCode();
        }
    }
}