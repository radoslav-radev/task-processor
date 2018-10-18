using System;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// A composite implementation of <see cref="ITaskProcessorMessageBus"/>.
    /// </summary>
    public partial class CompositeTaskProcessorMessageBus : ITaskProcessorMessageBus
    {
        private readonly ITaskProcessorMessageQueue masterCommands;

        private readonly CompositeTaskMessageBus taskMessageBus = new CompositeTaskMessageBus();
        private readonly CompositeTaskProcessorsMessageBus taskProcessorMessageBus = new CompositeTaskProcessorsMessageBus();

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeTaskProcessorMessageBus"/> class.
        /// </summary>
        /// <param name="masterCommands">The master commands queue to use.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="masterCommands"/> is null.</exception>
        public CompositeTaskProcessorMessageBus(ITaskProcessorMessageQueue masterCommands)
        {
            if (masterCommands == null)
            {
                throw new ArgumentNullException(nameof(masterCommands));
            }

            this.masterCommands = masterCommands;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets the composite task processor message bus.
        /// </summary>
        /// <value>The composite task processor message bus.</value>
        public CompositeTaskProcessorsMessageBus TaskProcessors
        {
            get { return this.taskProcessorMessageBus; }
        }

        /// <summary>
        /// Gets the composite task message bus.
        /// </summary>
        /// <value>The composite task message bus.</value>
        public CompositeTaskMessageBus Tasks
        {
            get { return this.taskMessageBus; }
        }

        #endregion Properties

        #region ITaskProcessorMessageBus Members

        /// <inheritdoc />
        public ITaskProcessorMessageQueue MasterCommands
        {
            get { return this.masterCommands; }
        }

        /// <inheritdoc />
        ITaskProcessorsMessageBus ITaskProcessorMessageBus.TaskProcessors
        {
            get { return this.taskProcessorMessageBus; }
        }

        /// <inheritdoc />
        ITaskMessageBus ITaskProcessorMessageBus.Tasks
        {
            get { return this.taskMessageBus; }
        }

        #endregion ITaskProcessorMessageBus Members
    }
}