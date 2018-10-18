namespace Radoslav.TaskProcessor.Model
{
    public sealed class DemoPollingQueueTask : DemoTask
    {
        public DemoPollingQueueTask()
        {
        }

        public DemoPollingQueueTask(int totalDurationInSeconds)
            : base(totalDurationInSeconds)
        {
        }
    }
}