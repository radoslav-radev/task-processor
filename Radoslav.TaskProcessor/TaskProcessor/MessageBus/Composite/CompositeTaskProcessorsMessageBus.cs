namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// A composite implementation of <see cref="ITaskProcessorsMessageBus"/>.
    /// </summary>
    public sealed class CompositeTaskProcessorsMessageBus : ITaskProcessorsMessageBus
    {
        private readonly CompositeTaskProcessorMessageBusSender senders = new CompositeTaskProcessorMessageBusSender();
        private readonly CompositeTaskProcessorMessageBusReceiver receivers = new CompositeTaskProcessorMessageBusReceiver();

        #region Properties

        /// <summary>
        /// Gets the task processor message bus receivers.
        /// </summary>
        /// <value>The task processor message bus receivers.</value>
        public CompositeTaskProcessorMessageBusReceiver Receivers
        {
            get { return this.receivers; }
        }

        /// <summary>
        /// Gets the task processor message bus senders.
        /// </summary>
        /// <value>The task processor message bus senders.</value>
        public CompositeTaskProcessorMessageBusSender Senders
        {
            get { return this.senders; }
        }

        #endregion Properties

        #region ITaskProcessorsMessageBus Members

        ITaskProcessorMessageBusReceiver ITaskProcessorsMessageBus.Receiver
        {
            get { return this.receivers; }
        }

        ITaskProcessorMessageBusSender ITaskProcessorsMessageBus.Sender
        {
            get { return this.senders; }
        }

        #endregion ITaskProcessorsMessageBus Members
    }
}