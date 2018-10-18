using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Radoslav.DateTimeProvider;
using Radoslav.TaskProcessor.Configuration;
using Radoslav.TaskProcessor.Facade;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.Repository;

namespace Radoslav.TaskProcessor.TaskScheduler
{
    /// <summary>
    /// An implementation of the <see cref="IPollingJob"/> that submit scheduled tasks based on recurrence definitions.
    /// </summary>
    public sealed class TaskSchedulerPollingJob : IPollingJob, IDisposable
    {
        private readonly ITaskProcessorRepository repository;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly ITaskProcessorFacade facade;
        private readonly ITaskSchedulerConfiguration configuration;
        private readonly ConcurrentDictionary<Guid, IScheduledTask> scheduledTasks = new ConcurrentDictionary<Guid, IScheduledTask>();
        private readonly ConcurrentDictionary<Guid, Guid> submittedTasksByScheduledTasks = new ConcurrentDictionary<Guid, Guid>();

        private DateTime lastCheckTasksUtc;

        #region Constructor & Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskSchedulerPollingJob"/> class.
        /// </summary>
        /// <param name="configProvider">The configuration provider to retrieve configuration.</param>
        /// <param name="repository">The repository to use.</param>
        /// <param name="dateTimeProvider">The date time provider to use.</param>
        /// <param name="facade">The task processor facade to use to submit tasks.</param>
        public TaskSchedulerPollingJob(ITaskProcessorConfigurationProvider configProvider, ITaskProcessorRepository repository, IDateTimeProvider dateTimeProvider, ITaskProcessorFacade facade)
        {
            if (configProvider == null)
            {
                throw new ArgumentNullException(nameof(configProvider));
            }

            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (dateTimeProvider == null)
            {
                throw new ArgumentNullException(nameof(dateTimeProvider));
            }

            if (facade == null)
            {
                throw new ArgumentNullException(nameof(facade));
            }

            this.configuration = configProvider.GetTaskSchedulerConfiguration();

            this.repository = repository;
            this.dateTimeProvider = dateTimeProvider;
            this.facade = facade;

            this.repository.ScheduledTasks.Added += this.OnScheduledTaskAdded;
            this.repository.ScheduledTasks.Updated += this.OnScheduledTaskUpdated;
            this.repository.ScheduledTasks.Deleted += this.OnScheduledTaskDeleted;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TaskSchedulerPollingJob"/> class.
        /// </summary>
        ~TaskSchedulerPollingJob()
        {
            this.Dispose(false);
        }

        #endregion Constructor & Destructor

        #region IPollingJob Members

        /// <inheritdoc />
        public void Initialize()
        {
            this.lastCheckTasksUtc = this.dateTimeProvider.UtcNow;

            foreach (IScheduledTask scheduledTask in this.repository.ScheduledTasks.GetAll())
            {
                this.AddScheduledTask(scheduledTask);
            }
        }

        /// <inheritdoc />
        public void Process()
        {
            DateTime checkTasksUtc = this.dateTimeProvider.UtcNow;

            List<Guid> tasksToRemove = new List<Guid>();

            foreach (IScheduledTask scheduledTask in this.scheduledTasks.Values)
            {
                if ((scheduledTask.Schedule.NextExecutionTimeUtc > this.lastCheckTasksUtc) &&
                    (scheduledTask.Schedule.NextExecutionTimeUtc < checkTasksUtc))
                {
                    bool waitForPreviousSubmittedTaskToComplete = false;

                    if (this.configuration != null)
                    {
                        IScheduledTaskConfiguration config = this.configuration.ScheduledTasks[scheduledTask.GetType()];

                        if (config != null)
                        {
                            waitForPreviousSubmittedTaskToComplete = config.WaitForPreviousSubmittedTaskToComplete;
                        }
                    }

                    Guid taskId;

                    bool submitTask = true;

                    if (waitForPreviousSubmittedTaskToComplete && this.submittedTasksByScheduledTasks.TryGetValue(scheduledTask.Id, out taskId))
                    {
                        if (this.repository.TaskRuntimeInfo.CheckIsPendingOrActive(taskId))
                        {
                            Trace.WriteLine("Previous submitted task '{0}' for scheduled task '{1}' has not yet completed.".FormatInvariant(taskId, scheduledTask.Id));

                            submitTask = false;
                        }
                    }

                    if (submitTask)
                    {
                        scheduledTask.PrepareNextTask();

                        taskId = this.facade.SubmitTask(scheduledTask.NextTask, scheduledTask.NextTaskSummary, scheduledTask.NextTaskPriority);

                        if (waitForPreviousSubmittedTaskToComplete)
                        {
                            this.submittedTasksByScheduledTasks[scheduledTask.Id] = taskId;
                        }
                    }

                    scheduledTask.Schedule.CalculateNextExecutionTime(checkTasksUtc);

                    if (!scheduledTask.Schedule.NextExecutionTimeUtc.HasValue)
                    {
                        tasksToRemove.Add(scheduledTask.Id);
                    }
                }
            }

            foreach (Guid scheduledTaskId in tasksToRemove)
            {
                this.scheduledTasks.TryRemove(scheduledTaskId);
            }

            this.lastCheckTasksUtc = checkTasksUtc;
        }

        #endregion IPollingJob Members

        #region IDisposable Members

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool dispose)
        {
            if (this.repository != null)
            {
                this.repository.ScheduledTasks.Added -= this.OnScheduledTaskAdded;
                this.repository.ScheduledTasks.Updated -= this.OnScheduledTaskUpdated;
                this.repository.ScheduledTasks.Deleted -= this.OnScheduledTaskDeleted;
            }
        }

        #endregion IDisposable Members

        private bool CalculateNextExecutionTime(IScheduledTask scheduledTask)
        {
            scheduledTask.Schedule.CalculateNextExecutionTime(this.lastCheckTasksUtc);

            return scheduledTask.Schedule.NextExecutionTimeUtc.HasValue;
        }

        private void AddScheduledTask(IScheduledTask scheduledTask)
        {
            if (this.CalculateNextExecutionTime(scheduledTask))
            {
                this.scheduledTasks.TryAdd(scheduledTask.Id, scheduledTask);
            }
        }

        private void OnScheduledTaskAdded(object sender, ScheduledTaskEventArgs e)
        {
            IScheduledTask scheduledTask = this.repository.ScheduledTasks.GetById(e.ScheduledTaskId);

            if (scheduledTask == null)
            {
                return;
            }

            this.AddScheduledTask(scheduledTask);
        }

        private void OnScheduledTaskUpdated(object sender, ScheduledTaskEventArgs e)
        {
            IScheduledTask scheduledTask = this.repository.ScheduledTasks.GetById(e.ScheduledTaskId);

            if (scheduledTask == null)
            {
                this.RemoveScheduledTask(e.ScheduledTaskId);

                return;
            }

            if (this.scheduledTasks.ContainsKey(e.ScheduledTaskId))
            {
                if (!this.CalculateNextExecutionTime(scheduledTask))
                {
                    this.RemoveScheduledTask(scheduledTask.Id);
                }
            }
            else
            {
                this.AddScheduledTask(scheduledTask);
            }
        }

        private void OnScheduledTaskDeleted(object sender, ScheduledTaskEventArgs e)
        {
            this.RemoveScheduledTask(e.ScheduledTaskId);
        }

        private void RemoveScheduledTask(Guid scheduledTaskId)
        {
            this.scheduledTasks.TryRemove(scheduledTaskId);
            this.submittedTasksByScheduledTasks.TryRemove(scheduledTaskId);
        }
    }
}