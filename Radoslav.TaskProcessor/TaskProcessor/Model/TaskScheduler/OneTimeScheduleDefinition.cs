using System;
using System.Xml.Serialization;

namespace Radoslav.TaskProcessor.TaskScheduler
{
    /// <summary>
    /// An implementation of <see cref="IScheduleDefinition"/> for one-time scheduled tasks.
    /// </summary>
    [Serializable]
    public sealed class OneTimeScheduleDefinition : IScheduleDefinition
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OneTimeScheduleDefinition"/> class.
        /// </summary>
        [Obsolete("Deserialization only", true)]
        public OneTimeScheduleDefinition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneTimeScheduleDefinition"/> class.
        /// </summary>
        /// <param name="scheduledDateTimeUtc">When the scheduled task should be executed, in UTC.</param>
        public OneTimeScheduleDefinition(DateTime scheduledDateTimeUtc)
        {
            this.ScheduledDateTimeUtc = scheduledDateTimeUtc;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets when the scheduled task should be executed, in UTC.
        /// </summary>
        /// <value>When the scheduled task should be executed, in UTC.</value>
        public DateTime ScheduledDateTimeUtc { get; set; }

        #endregion Properties

        #region IScheduleDefinition Members

        /// <inheritdoc />
        [XmlIgnore]
        public DateTime? NextExecutionTimeUtc { get; private set; }

        /// <inheritdoc />
        public void CalculateNextExecutionTime(DateTime currentDateTimeUtc)
        {
            if (currentDateTimeUtc <= this.ScheduledDateTimeUtc)
            {
                this.NextExecutionTimeUtc = this.ScheduledDateTimeUtc;
            }
            else
            {
                this.NextExecutionTimeUtc = null;
            }
        }

        #endregion IScheduleDefinition Members
    }
}