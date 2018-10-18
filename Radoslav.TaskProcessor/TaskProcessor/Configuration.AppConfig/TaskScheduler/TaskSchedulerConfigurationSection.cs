using System.Configuration;

namespace Radoslav.TaskProcessor.Configuration.AppConfig
{
    /// <summary>
    /// An implementation of the <see cref="ITaskSchedulerConfiguration"/> interface that reads from App.config file.
    /// </summary>
    public sealed class TaskSchedulerConfigurationSection : ConfigurationSection, ITaskSchedulerConfiguration
    {
        /// <summary>
        /// The section name in App.config file.
        /// </summary>
        public const string SectionName = "Radoslav.TaskProcessor.TaskScheduler";

        #region ITaskSchedulerConfiguration Members

        /// <inheritdoc />
        IScheduledTasksConfiguration ITaskSchedulerConfiguration.ScheduledTasks
        {
            get { return this.ScheduledTasks; }
        }

        #endregion ITaskSchedulerConfiguration Members

        [ConfigurationProperty("scheduledTasks")]
        private ScheduledTasksConfigurationCollection ScheduledTasks
        {
            get
            {
                return (ScheduledTasksConfigurationCollection)this["scheduledTasks"];
            }
        }
    }
}