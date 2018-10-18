using System;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Event arguments for <see cref="ITaskProcessorMessageBusReceiver.MasterModeChanged"/> event.
    /// </summary>
    public sealed class MasterModeChangeEventArgs : TaskProcessorEventArgs
    {
        private readonly bool isMaster;
        private readonly MasterModeChangeReason reason;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterModeChangeEventArgs"/> class.
        /// </summary>
        /// <param name="taskProcessorId">The ID of the task processor who has become master or a slave.</param>
        /// <param name="isMaster">Whether the task processor has become master, or slave.</param>
        /// <param name="reason">The reason for the change.</param>
        public MasterModeChangeEventArgs(Guid taskProcessorId, bool isMaster, MasterModeChangeReason reason)
            : base(taskProcessorId)
        {
            MasterModeChangeEventArgs.ValidateArguments(isMaster, reason);

            this.isMaster = isMaster;
            this.reason = reason;
        }

        /// <summary>
        /// Gets a value indicating whether the task processor has become master, or slave.
        /// </summary>
        /// <value>Whether the task processor has become master, or slave.</value>
        public bool IsMaster
        {
            get { return this.isMaster; }
        }

        /// <summary>
        /// Gets the reason for the change.
        /// </summary>
        /// <value>The reason for the change.</value>
        public MasterModeChangeReason Reason
        {
            get { return this.reason; }
        }

        /// <summary>
        /// Validates arguments for the constructor.
        /// </summary>
        /// <param name="isMaster">Whether the task processor has become master, or slave.</param>
        /// <param name="reason">The reason for the change.</param>
        public static void ValidateArguments(bool isMaster, MasterModeChangeReason reason)
        {
            switch (reason)
            {
                case MasterModeChangeReason.None:
                    throw new ArgumentOutOfRangeException("reason", reason, "Value must not be {0}.".FormatInvariant(reason));

                case MasterModeChangeReason.Start:
                    if (!isMaster)
                    {
                        throw new ArgumentOutOfRangeException("reason", reason, "Value must not be {0} when is master is {1}.".FormatInvariant(reason, isMaster));
                    }

                    break;
            }
        }
    }
}