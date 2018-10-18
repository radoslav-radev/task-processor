using System;

namespace Radoslav.TaskProcessor.TaskScheduler
{
    /// <summary>
    /// Basic definition of a schedule definition used for scheduling tasks.
    /// </summary>
    public interface IScheduleDefinition
    {
        /// <summary>
        /// Gets next task execution time according to schedule, in UTC.
        /// </summary>
        /// <value>Next task execution time according to schedule, in UTC, or null if the task should not be executed any more.</value>
        DateTime? NextExecutionTimeUtc { get; }

        /// <summary>
        /// Calculates the next execution time according to schedule.
        /// </summary>
        /// <remarks>If the task should not be executed any more, this method should set <see cref="NextExecutionTimeUtc"/> to null.</remarks>
        /// <param name="currentDateTimeUtc">Current date time, in UTC.</param>
        void CalculateNextExecutionTime(DateTime currentDateTimeUtc);
    }
}