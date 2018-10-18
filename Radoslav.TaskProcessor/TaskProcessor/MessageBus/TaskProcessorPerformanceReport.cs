using System;
using System.Collections.Generic;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Class for performance metrics of a task processor.
    /// </summary>
    public sealed class TaskProcessorPerformanceReport
    {
        private readonly Guid taskProcessorId;
        private readonly List<TaskPerformanceReport> tasksPerformance = new List<TaskPerformanceReport>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskProcessorPerformanceReport"/> class.
        /// </summary>
        /// <param name="taskProcessorId">The ID of the task processor.</param>
        public TaskProcessorPerformanceReport(Guid taskProcessorId)
        {
            this.taskProcessorId = taskProcessorId;
        }

        /// <summary>
        /// Gets the ID of the task processor.
        /// </summary>
        /// <value>The ID of the task processor.</value>
        public Guid TaskProcessorId
        {
            get { return this.taskProcessorId; }
        }

        /// <summary>
        /// Gets or sets the percentage of the used RAM on the machine where the task processor is hosted.
        /// </summary>
        /// <value>The percentage of the used RAM on the machine where the task processor is hosted.</value>
        public int RamPercent { get; set; }

        /// <summary>
        /// Gets or sets the percentage of the used CPU on the machine where the task processor is hosted.
        /// </summary>
        /// <value>The percentage of the used CPU on the machine where the task processor is hosted.</value>
        public int CpuPercent { get; set; }

        /// <summary>
        /// Gets a collection with performance metrics for the tasks currently executed by the task processor.
        /// </summary>
        /// <value>Performance metrics for the tasks currently executed by the task processor.</value>
        public ICollection<TaskPerformanceReport> TasksPerformance
        {
            get { return this.tasksPerformance; }
        }
    }
}