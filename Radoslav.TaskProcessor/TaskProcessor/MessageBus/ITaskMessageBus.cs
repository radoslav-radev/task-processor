using System;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Basic functionality of a message bus for task events.
    /// </summary>
    public interface ITaskMessageBus
    {
        /// <summary>
        /// Gets the task message bus receiver.
        /// </summary>
        /// <value>The task message bus receiver.</value>
        ITaskMessageBusReceiver Receiver { get; }

        /// <summary>
        /// Gets a task message bus sender for a specific task type.
        /// </summary>
        /// <param name="taskType">The type of the task.</param>
        /// <returns>A task message bus sender for a specified task type, or null if the task type is not supported.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="taskType"/> is null.</exception>
        ITaskMessageBusSender GetSender(Type taskType);
    }
}