using System;
using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal abstract class FakePollingConfiguration : IPollingConfiguration
    {
        internal FakePollingConfiguration()
        {
        }

        internal FakePollingConfiguration(IPollingConfiguration source)
        {
            this.PollInterval = source.PollInterval;
            this.IsActive = source.IsActive;
            this.IsMaster = source.IsMaster;
            this.IsConcurrent = source.IsConcurrent;
        }

        #region IPollingConfiguration Members

        public TimeSpan PollInterval { get; set; }

        public bool IsMaster { get; set; }

        public bool IsActive { get; set; }

        public bool IsConcurrent { get; set; }

        #endregion IPollingConfiguration Members
    }
}