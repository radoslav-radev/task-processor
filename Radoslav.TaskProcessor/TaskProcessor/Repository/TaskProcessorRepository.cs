using System;
using System.Diagnostics;

namespace Radoslav.TaskProcessor.Repository
{
    /// <summary>
    /// A composite implementation of <see cref="ITaskProcessorRepository"/>.
    /// </summary>
    public abstract class TaskProcessorRepository : ITaskProcessorRepository
    {
        /// <summary>
        /// The default time interval after which a task processor is considered crashed if it has not heartbeated.
        /// </summary>
        public static readonly TimeSpan DefaultExpirationTimeout = TimeSpan.FromSeconds(15);

        private readonly ITaskRuntimeInfoRepository taskRuntimeInfo;
        private readonly ITaskProcessorRuntimeInfoRepository taskProcessors;
        private ITaskRepository tasks;
        private ITaskSummaryRepository taskSummary;
        private ITaskJobSettingsRepository taskJobSettings;
        private IScheduledTaskRepository scheduledTasks;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskProcessorRepository"/> class.
        /// </summary>
        /// <param name="tasksRepository">The tasks repository.</param>
        /// <param name="taskRuntimeInfoRepository">The task runtime information repository.</param>
        /// <param name="taskProcessorsRepository">The task processor runtime information repository.</param>
        /// <param name="taskSummaryRepository">The task summary repository.</param>
        /// <param name="taskJobSettingsRepository">The task job settings repository.</param>
        /// <param name="scheduledTasksRepository">The scheduled tasks repository.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="tasksRepository"/>,
        /// <paramref name="taskRuntimeInfoRepository"/>, <paramref name="taskProcessorsRepository"/>, <paramref name="taskProcessorsRepository"/>
        /// <paramref name="taskJobSettingsRepository"/> or <paramref name="scheduledTasksRepository"/> is null.</exception>
        protected TaskProcessorRepository(
            ITaskRepository tasksRepository,
            ITaskRuntimeInfoRepository taskRuntimeInfoRepository,
            ITaskProcessorRuntimeInfoRepository taskProcessorsRepository,
            ITaskSummaryRepository taskSummaryRepository,
            ITaskJobSettingsRepository taskJobSettingsRepository,
            IScheduledTaskRepository scheduledTasksRepository)
        {
            Trace.WriteLine("ENTER: Constructing '{0}' ...".FormatInvariant(this.GetType().Name));

            if (tasksRepository == null)
            {
                throw new ArgumentNullException("tasksRepository");
            }

            if (taskRuntimeInfoRepository == null)
            {
                throw new ArgumentNullException("taskRuntimeInfoRepository");
            }

            if (taskProcessorsRepository == null)
            {
                throw new ArgumentNullException("taskProcessorsRepository");
            }

            if (taskSummaryRepository == null)
            {
                throw new ArgumentNullException("taskSummaryRepository");
            }

            if (taskJobSettingsRepository == null)
            {
                throw new ArgumentNullException("taskJobSettingsRepository");
            }

            if (scheduledTasksRepository == null)
            {
                throw new ArgumentNullException("scheduledTasksRepository");
            }

            this.tasks = tasksRepository;
            this.taskRuntimeInfo = taskRuntimeInfoRepository;
            this.taskProcessors = taskProcessorsRepository;
            this.taskSummary = taskSummaryRepository;
            this.taskJobSettings = taskJobSettingsRepository;
            this.scheduledTasks = scheduledTasksRepository;

            Trace.WriteLine("ENTER: Constructing '{0}' ...".FormatInvariant(this.GetType().Name));
        }

        #endregion Constructor

        #region ITaskProcessorRepository Members

        /// <inheritdoc />
        public ITaskRepository Tasks
        {
            get
            {
                return this.tasks;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.tasks = value;
            }
        }

        /// <inheritdoc />
        public ITaskRuntimeInfoRepository TaskRuntimeInfo
        {
            get { return this.taskRuntimeInfo; }
        }

        /// <inheritdoc />
        public ITaskProcessorRuntimeInfoRepository TaskProcessorRuntimeInfo
        {
            get { return this.taskProcessors; }
        }

        /// <inheritdoc />
        public ITaskSummaryRepository TaskSummary
        {
            get
            {
                return this.taskSummary;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.taskSummary = value;
            }
        }

        /// <inheritdoc />
        public ITaskJobSettingsRepository TaskJobSettings
        {
            get
            {
                return this.taskJobSettings;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.taskJobSettings = value;
            }
        }

        /// <inheritdoc />
        public IScheduledTaskRepository ScheduledTasks
        {
            get
            {
                return this.scheduledTasks;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.scheduledTasks = value;
            }
        }

        #endregion ITaskProcessorRepository Members
    }
}