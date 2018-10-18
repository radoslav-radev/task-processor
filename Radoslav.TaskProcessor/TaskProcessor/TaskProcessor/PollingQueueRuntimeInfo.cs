using System.Threading;

namespace Radoslav.TaskProcessor
{
    public partial class RadoslavTaskProcessor
    {
        internal sealed class PollingQueueRuntimeInfo
        {
            private readonly string key;

            private int activeTasksCount;

            internal PollingQueueRuntimeInfo(string key)
            {
                this.key = key;
            }

            internal string Key
            {
                get { return this.key; }
            }

            internal int MaxWorkers { get; set; }

            internal bool IsConcurrent { get; set; }

            internal bool IsProcessing { get; set; }

            internal int ActiveTasksCount
            {
                get { return this.activeTasksCount; }
            }

            internal void IncrementActiveTasksCount()
            {
                Interlocked.Increment(ref this.activeTasksCount);
            }

            internal void DecrementActiveTasksCount()
            {
                Interlocked.Decrement(ref this.activeTasksCount);
            }
        }
    }
}