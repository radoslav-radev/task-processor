using Radoslav.TaskProcessor.MessageBus;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed partial class FakeMessageBus : ITaskProcessorMessageBus
    {
        private readonly FakeMessageQueue masterCommands = new FakeMessageQueue();
        private readonly FakeTaskMessageBus tasks;
        private readonly FakeTaskProcessorsMessageBus taskProcessors;

        internal FakeMessageBus()
        {
            this.tasks = new FakeTaskMessageBus(this.masterCommands);
            this.taskProcessors = new FakeTaskProcessorsMessageBus(this.masterCommands);
        }

        #region ITaskProcessorMessageBus Members

        ITaskProcessorsMessageBus ITaskProcessorMessageBus.TaskProcessors
        {
            get { return this.taskProcessors; }
        }

        ITaskMessageBus ITaskProcessorMessageBus.Tasks
        {
            get { return this.tasks; }
        }

        public ITaskProcessorMessageQueue MasterCommands
        {
            get { return this.masterCommands; }
        }

        #endregion ITaskProcessorMessageBus Members

        internal FakeTaskMessageBus Tasks
        {
            get { return this.tasks; }
        }

        internal FakeTaskProcessorsMessageBus TaskProcessors
        {
            get { return this.taskProcessors; }
        }
    }
}