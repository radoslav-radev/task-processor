using System;
using Radoslav.TaskProcessor.TaskScheduler;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakeScheduleDefinition : MockObject, IScheduleDefinition
    {
        #region IScheduleDefinition Members

        public DateTime? NextExecutionTimeUtc { get; internal set; }

        public void CalculateNextExecutionTime(DateTime currentDateTimeUtc)
        {
            this.RecordMethodCall(currentDateTimeUtc);

            this.ExecutePredefinedMethod(currentDateTimeUtc);
        }

        #endregion IScheduleDefinition Members
    }
}