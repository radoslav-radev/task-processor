using System;
using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakePollingJobConfiguration : FakePollingConfiguration, IPollingJobConfiguration
    {
        internal FakePollingJobConfiguration()
        {
        }

        internal FakePollingJobConfiguration(IPollingJobConfiguration source)
            : base(source)
        {
            this.ImplementationType = source.ImplementationType;
        }

        #region IPollingJobConfiguration Members

        public Type ImplementationType { get; internal set; }

        #endregion IPollingJobConfiguration Members
    }
}