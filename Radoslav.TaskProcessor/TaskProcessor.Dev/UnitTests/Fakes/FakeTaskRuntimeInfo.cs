using System;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakeTaskRuntimeInfo : ITaskRuntimeInfo, IEquatable<ITaskRuntimeInfo>
    {
        #region ITaskRuntimeInfo Members

        public Guid TaskId { get; internal set; }

        public Type TaskType { get; internal set; }

        public string PollingQueue { get; internal set; }

        public TaskPriority Priority { get; internal set; }

        public TaskStatus Status { get; internal set; }

        public Guid? TaskProcessorId { get; internal set; }

        public double Percentage { get; internal set; }

        public DateTime SubmittedUtc { get; internal set; }

        public DateTime? StartedUtc { get; internal set; }

        public DateTime? CanceledUtc { get; internal set; }

        public DateTime? CompletedUtc { get; internal set; }

        public string Error { get; internal set; }

        #endregion ITaskRuntimeInfo Members

        #region IEquatable<FakeTaskRuntimeInfo> Members

        public bool Equals(ITaskRuntimeInfo other)
        {
            return UnitTestHelpers.AreEqualByPublicScalarProperties(this, other);
        }

        #endregion IEquatable<FakeTaskRuntimeInfo> Members

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ITaskRuntimeInfo);
        }

        public override int GetHashCode()
        {
            return this.TaskId.GetHashCode();
        }
    }
}