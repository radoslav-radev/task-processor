using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Sample
{
    public class SampleTask : ITask
    {
        public SampleTaskDetail[] Details { get; set; }
    }
}