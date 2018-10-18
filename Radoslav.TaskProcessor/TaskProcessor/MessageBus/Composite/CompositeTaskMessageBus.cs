using System;
using System.Collections.Generic;
using System.Linq;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// A composite implementation of <see cref="ITaskMessageBus"/>.
    /// </summary>
    public sealed class CompositeTaskMessageBus : ITaskMessageBus
    {
        private readonly CompositeTaskMessageBusReceiver receivers = new CompositeTaskMessageBusReceiver();
        private readonly CompositeTaskMessageBusSender senders = new CompositeTaskMessageBusSender();

        #region Properties

        /// <summary>
        /// Gets the task message bus senders.
        /// </summary>
        /// <value>The task message bus senders.</value>
        public ICollection<ITaskMessageBusSender> Senders
        {
            get { return this.senders; }
        }

        /// <summary>
        /// Gets the task message bus receivers.
        /// </summary>
        /// <value>The task message bus receivers.</value>
        public ICollection<ITaskMessageBusReceiver> Receivers
        {
            get { return this.receivers; }
        }

        #endregion Properties

        #region ITasksMessageBus Members

        /// <inheritdoc />
        ITaskMessageBusReceiver ITaskMessageBus.Receiver
        {
            get { return this.receivers; }
        }

        /// <inheritdoc />
        public ITaskMessageBusSender GetSender(Type taskType)
        {
            return new CompositeTaskMessageBusSender(this.senders
                .Where(s => s.IsSupported(taskType))
                .ToList());
        }

        #endregion ITasksMessageBus Members
    }
}