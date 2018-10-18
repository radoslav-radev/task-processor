using System;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor
{
    public partial class RadoslavTaskProcessor
    {
        internal sealed class PollingJobRuntimeInfo
        {
            private readonly IPollingJob pollingJob;
            private readonly Type implementationType;

            internal PollingJobRuntimeInfo(Type implementationType, IPollingJob pollingJob)
            {
                this.implementationType = implementationType;
                this.pollingJob = pollingJob;
            }

            internal Type ImplementationType
            {
                get { return this.implementationType; }
            }

            internal IPollingJob PollingJob
            {
                get { return this.pollingJob; }
            }

            internal bool IsConcurrent { get; set; }

            internal bool IsProcessing { get; set; }
        }
    }
}