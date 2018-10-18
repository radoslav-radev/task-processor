namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Basic functionality of a message bus for task processors events.
    /// </summary>
    public interface ITaskProcessorsMessageBus
    {
        /// <summary>
        /// Gets the sender of task processor events.
        /// </summary>
        /// <value>The sender of task processor events.</value>
        ITaskProcessorMessageBusSender Sender { get; }

        /// <summary>
        /// Gets the receiver of task processor events.
        /// </summary>
        /// <value>The receiver of task processor events.</value>
        ITaskProcessorMessageBusReceiver Receiver { get; }
    }
}