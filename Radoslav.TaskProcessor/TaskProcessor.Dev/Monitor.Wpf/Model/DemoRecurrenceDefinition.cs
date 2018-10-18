using System;
using System.Xml.Serialization;
using Radoslav.TaskProcessor.TaskScheduler;

namespace Radoslav.TaskProcessor.Model
{
    [Serializable]
    public sealed class DemoRecurrenceDefinition : IScheduleDefinition
    {
        public int DelayInSeconds { get; set; }

        public int Occurrences { get; set; }

        #region IRecurrenceDefinition Members

        [XmlIgnore]
        public DateTime? NextExecutionTimeUtc { get; private set; }

        public void CalculateNextExecutionTime(DateTime currentDateTimeUtc)
        {
            if (this.Occurrences <= 0)
            {
                this.NextExecutionTimeUtc = null;

                return;
            }

            this.NextExecutionTimeUtc = currentDateTimeUtc.AddSeconds(this.DelayInSeconds);

            this.Occurrences--;
        }

        #endregion IRecurrenceDefinition Members
    }
}