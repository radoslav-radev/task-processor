using System;
using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakeTaskWorkerConfiguration : ITaskWorkerConfiguration
    {
        #region ITaskWorkerConfiguration Members

        public Type TaskType
        {
            get { throw new NotSupportedException(); }
        }

        public Type WorkerType
        {
            get { throw new NotSupportedException(); }
        }

        public bool HasTaskJobSettings { get; set; }

        #endregion ITaskWorkerConfiguration Members
    }
}