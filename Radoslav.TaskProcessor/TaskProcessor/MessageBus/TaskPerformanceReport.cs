using System;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Class for performance metrics of a task.
    /// </summary>
    public sealed class TaskPerformanceReport
    {
        private readonly Guid taskId;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskPerformanceReport"/> class.
        /// </summary>
        /// <param name="taskId">The ID of the task for which is the performance report.</param>
        public TaskPerformanceReport(Guid taskId)
        {
            this.taskId = taskId;
        }

        /// <summary>
        /// Gets the ID of the task for which is the performance report.
        /// </summary>
        /// <value>The ID of the task for which is the performance report.</value>
        public Guid TaskId
        {
            get { return this.taskId; }
        }

        /// <summary>
        /// Gets or sets the percentage of the RAM used by the task.
        /// </summary>
        /// <value>The percentage of the RAM used by the task.</value>
        public float RamPercent { get; set; }

        /// <summary>
        /// Gets or sets the percentage of the CPU used by the task.
        /// </summary>
        /// <value>The percentage of the CPU used by the task.</value>
        public float CpuPercent { get; set; }
    }
}