using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Sample
{
    public sealed class SamplePollingQueueTask : ITask
    {
        public SampleTaskDetail[] Details { get; set; }
    }
}