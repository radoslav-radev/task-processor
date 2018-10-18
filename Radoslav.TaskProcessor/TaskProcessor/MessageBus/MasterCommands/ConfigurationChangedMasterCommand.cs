using System;
using System.ComponentModel;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Master command indicating that a task processor configuration has been changed.
    /// </summary>
    [Serializable]
    public sealed class ConfigurationChangedMasterCommand : IUniqueMessage
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationChangedMasterCommand"/> class.
        /// </summary>
        [Obsolete("Default constructor is required by XML serialization.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ConfigurationChangedMasterCommand()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationChangedMasterCommand"/> class.
        /// </summary>
        /// <param name="taskProcessorId">The ID of the task processor whose configuration has been changed.</param>
        public ConfigurationChangedMasterCommand(Guid taskProcessorId)
        {
            this.TaskProcessorId = taskProcessorId;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the ID of the task processor whose configuration has been changed.
        /// </summary>
        /// <value>The ID of the task processor whose configuration has been changed.</value>
        public Guid TaskProcessorId { get; set; }

        #endregion Properties

        #region IUniqueMessage Members

        /// <inheritdoc />
        public string MessageUniqueId
        {
            get
            {
                return this.TaskProcessorId.ToString();
            }
        }

        #endregion IUniqueMessage Members
    }
}