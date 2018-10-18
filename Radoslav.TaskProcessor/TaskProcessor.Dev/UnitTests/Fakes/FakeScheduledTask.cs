namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed partial class FakeScheduledTask : MockObject
    {
        private readonly FakeScheduleDefinition schedule = new FakeScheduleDefinition();

        public long Value { get; set; }

        internal FakeScheduleDefinition Schedule
        {
            get { return this.schedule; }
        }
    }
}