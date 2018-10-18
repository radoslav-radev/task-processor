namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Basic implementation of a message bus needed by task processors.
    /// </summary>
    public interface ITaskProcessorMessageBus
    {
        /// <summary>
        /// Gets the master commands message queue.
        /// </summary>
        /// <value>The master commands message queue.</value>
        ITaskProcessorMessageQueue MasterCommands { get; }

        /// <summary>
        /// Gets the message bus for task processors events.
        /// </summary>
        /// <value>The message bus for task processors events.</value>
        ITaskProcessorsMessageBus TaskProcessors { get; }

        /// <summary>
        /// Gets the message bus for task events.
        /// </summary>
        /// <value>The message bus for task events.</value>
        ITaskMessageBus Tasks { get; }
    }
}