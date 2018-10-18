using System;
using Radoslav.TaskProcessor.Configuration;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakeTaskProcessorRuntimeInfo : ITaskProcessorRuntimeInfo
    {
        #region ITaskProcessorRuntimeInfo Members

        public Guid TaskProcessorId { get; internal set; }

        public string MachineName { get; internal set; }

        ITaskProcessorConfiguration ITaskProcessorRuntimeInfo.Configuration
        {
            get { return this.Configuration; }
        }

        #endregion ITaskProcessorRuntimeInfo Members

        internal DateTime ExpireAt { get; set; }

        internal FakeTaskProcessorConfiguration Configuration { get; set; }
    }
}