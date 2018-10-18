using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Basic functionality for a strongly-typed message queue.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "This is a message queue implementation.")]
    public interface ITaskProcessorMessageQueue : IEnumerable<IUniqueMessage>
    {
        /// <summary>
        /// Event that is raised when a new message has been received by the message queue.
        /// </summary>
        event EventHandler MessageReceived;

        /// <summary>
        /// Gets or sets a value indicating whether to listen for messages and raise <see cref="MessageReceived"/> event.
        /// </summary>
        /// <value>Whether to listen for messages and raise <see cref="MessageReceived"/> event.</value>
        bool ReceiveMessages { get; set; }

        /// <summary>
        /// Pushes a message to the message queue.
        /// </summary>
        /// <param name="message">The message to push.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="message"/> is null.</exception>
        void Push(IUniqueMessage message);

        /// <summary>
        /// Removes the first message from the queue and returns it.
        /// </summary>
        /// <returns>The removed first message from the queue, or null if the queue is empty.</returns>
        IUniqueMessage PopFirst();
    }
}