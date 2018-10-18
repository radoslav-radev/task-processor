using System;
using System.Collections.Generic;
using System.Diagnostics;
using Radoslav.DateTimeProvider;
using Radoslav.TaskProcessor.Configuration;
using Radoslav.TaskProcessor.MessageBus;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.Repository;
using Radoslav.TaskProcessor.TaskScheduler;

namespace Radoslav.TaskProcessor.Facade
{
    /// <summary>
    /// Class for communication with the task processor.
    /// </summary>
    public sealed class TaskProcessorFacade : ITaskProcessorFacade
    {
        #region Fields

        private readonly ITaskProcessorRepository repository;
        private readonly ITaskProcessorMessageBus messageBus;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly ITaskProcessorClientConfiguration configuration;
        private readonly string debugName = typeof(TaskProcessorFacade).Name;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskProcessorFacade"/> class.
        /// </summary>
        /// <param name="repository">The repository to use.</param>
        /// <param name="messageBus">The message bus to use.</param>
        /// <param name="dateTimeProvider">The date time provider to use.</param>
        /// <param name="appConfigConfigurationProvider">The configuration provider to read from App.config.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="repository"/>, <paramref name="messageBus"/>,
        /// <paramref name="dateTimeProvider"/> or <paramref name="appConfigConfigurationProvider"/> is null.</exception>
        public TaskProcessorFacade(
            ITaskProcessorRepository repository,
            ITaskProcessorMessageBus messageBus,
            IDateTimeProvider dateTimeProvider,
            ITaskProcessorConfigurationProvider appConfigConfigurationProvider)
        {
            Trace.WriteLine("ENTER: Constructing {0} ...".FormatInvariant(this.debugName));

            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            if (messageBus == null)
            {
                throw new ArgumentNullException("messageBus");
            }

            if (dateTimeProvider == null)
            {
                throw new ArgumentNullException("dateTimeProvider");
            }

            if (appConfigConfigurationProvider == null)
            {
                throw new ArgumentNullException("appConfigConfigurationProvider");
            }

            this.repository = repository;
            this.messageBus = messageBus;
            this.dateTimeProvider = dateTimeProvider;

            this.configuration = appConfigConfigurationProvider.GetClientConfiguration();

            Trace.WriteLine("EXIT: {0} constructed.".FormatInvariant(this.debugName));
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets the repository used by the task processor facade.
        /// </summary>
        /// <value>The repository used by the task processor facade.</value>
        public ITaskProcessorRepository Repository
        {
            get { return this.repository; }
        }

        /// <summary>
        /// Gets the message bus used by the task processor facade.
        /// </summary>
        /// <value>The message bus used by the task processor facade.</value>
        public ITaskProcessorMessageBus MessageBus
        {
            get { return this.messageBus; }
        }

        /// <summary>
        /// Gets the date time provider used by the task processor facade.
        /// </summary>
        /// <value>The date time provider used by the task processor facade.</value>
        public IDateTimeProvider DateTimeProvider
        {
            get { return this.dateTimeProvider; }
        }

        #endregion Properties

        #region ITaskProcessorFacade Members

        /// <inheritdoc />
        public Guid SubmitTask(ITask task)
        {
            return this.SubmitTask(task, null, TaskPriority.Normal);
        }

        /// <inheritdoc />
        public Guid SubmitTask(ITask task, ITaskSummary summary)
        {
            return this.SubmitTask(task, summary, TaskPriority.Normal);
        }

        /// <inheritdoc />
        public Guid SubmitTask(ITask task, TaskPriority priority)
        {
            return this.SubmitTask(task, null, priority);
        }

        /// <inheritdoc />
        public Guid SubmitTask(ITask task, ITaskSummary summary, TaskPriority priority)
        {
            Guid taskId = Guid.NewGuid();

            this.SubmitTask(taskId, task, summary, priority);

            return taskId;
        }

        /// <inheritdoc />
        public void CancelTask(Guid taskId)
        {
            Trace.WriteLine("ENTER: Cancelling task '{0}' ...".FormatInvariant(taskId));

            Lazy<DateTime> timestampUtc = new Lazy<DateTime>(() => this.dateTimeProvider.UtcNow);

            ITaskRuntimeInfo taskInfo = this.repository.TaskRuntimeInfo.GetById(taskId);

            if (taskInfo == null)
            {
                throw new KeyNotFoundException("Task '{0}' was not found.".FormatInvariant(taskId));
            }

            switch (taskInfo.Status)
            {
                case TaskStatus.Canceled:
                case TaskStatus.Failed:
                case TaskStatus.Success:
                    throw new InvalidOperationException("Task '{0}' status is {1} and will not be canceled.".FormatInvariant(taskId, taskInfo.Status));
            }

            ITaskMessageBusSender taskMessageBus = this.messageBus.Tasks.GetSender(taskInfo.TaskType);

            taskMessageBus.NotifyTaskCancelRequest(taskId, timestampUtc.Value);

            this.repository.TaskRuntimeInfo.RequestCancel(taskId, timestampUtc.Value);

            Trace.WriteLine("EXIT: Task '{0}' canceled.".FormatInvariant(taskId));
        }

        /// <inheritdoc />
        public void MakeTaskProcessorMaster(Guid taskProcessorId)
        {
            Trace.WriteLine("ENTER: Setting task processor '{0}' as master ... ".FormatInvariant(taskProcessorId));

            this.repository.TaskProcessorRuntimeInfo.SetMaster(taskProcessorId);

            this.messageBus.TaskProcessors.Sender.NotifyMasterModeChangeRequest(taskProcessorId, true);

            Trace.WriteLine("EXIT: Task processor '{0}' set as master. ".FormatInvariant(taskProcessorId));
        }

        /// <inheritdoc />
        public void RequestTaskProcessorToStop(Guid taskProcessorId)
        {
            Trace.WriteLine("ENTER: Requesting task processor '{0}' to stop ...".FormatInvariant(taskProcessorId));

            this.messageBus.TaskProcessors.Sender.NotifyStopRequested(taskProcessorId);

            Trace.WriteLine("ENTER: Stop task processor '{0}' requested.".FormatInvariant(taskProcessorId));
        }

        /// <inheritdoc />
        public IEnumerable<ITaskProcessorRuntimeInfo> GetTaskProcessorRuntimeInfo()
        {
            return this.repository.TaskProcessorRuntimeInfo.GetAll();
        }

        /// <inheritdoc />
        public Guid? GetMasterTaskProcessorId()
        {
            return this.repository.TaskProcessorRuntimeInfo.GetMasterId();
        }

        /// <inheritdoc />
        public ITaskRuntimeInfo GetTaskRuntimeInfo(Guid taskId)
        {
            return this.repository.TaskRuntimeInfo.GetById(taskId);
        }

        /// <inheritdoc />
        public ITaskProcessorRuntimeInfo GetTaskProcessorRuntimeInfo(Guid taskProcessorId)
        {
            return this.repository.TaskProcessorRuntimeInfo.GetById(taskProcessorId);
        }

        /// <inheritdoc />
        public void UpdateTaskProcessorRuntimeInfo(ITaskProcessorRuntimeInfo taskProcessorInfo)
        {
            if (taskProcessorInfo == null)
            {
                throw new ArgumentNullException("taskProcessorInfo");
            }

            Trace.WriteLine("ENTER: Updating {0} ...".FormatInvariant(taskProcessorInfo));

            this.repository.TaskProcessorRuntimeInfo.Update(taskProcessorInfo);

            Trace.WriteLine("EXIT: {0} updated.".FormatInvariant(taskProcessorInfo));
        }

        /// <inheritdoc />
        public ITask GetTask(Guid taskId)
        {
            return this.repository.Tasks.GetById(taskId);
        }

        /// <inheritdoc />
        public ITaskJobSettings GetTaskJobSettings(Type taskType)
        {
            return this.repository.TaskJobSettings.Get(taskType);
        }

        /// <inheritdoc />
        public void SetTaskJobSettings(Type taskType, ITaskJobSettings settings)
        {
            this.repository.TaskJobSettings.Set(taskType, settings);
        }

        /// <inheritdoc />
        public void ClearTaskJobSettings(Type taskType)
        {
            this.repository.TaskJobSettings.Clear(taskType);
        }

        /// <inheritdoc />
        public ITaskSummary GetTaskSummary(Guid taskId)
        {
            return this.repository.TaskSummary.GetById(taskId);
        }

        /// <inheritdoc />
        public void AddScheduledTask(IScheduledTask scheduledTask)
        {
            this.repository.ScheduledTasks.Add(scheduledTask);
        }

        /// <inheritdoc />
        public void UpdateScheduledTask(IScheduledTask scheduledTask)
        {
            this.repository.ScheduledTasks.Update(scheduledTask);
        }

        /// <inheritdoc />
        public void DeleteScheduledTask(Guid scheduledTaskId)
        {
            this.repository.ScheduledTasks.Delete(scheduledTaskId);
        }

        /// <inheritdoc />
        public ISubmitTaskSession CreateSubmitTaskSession()
        {
            return new SubmitTaskSession(this);
        }

        #endregion ITaskProcessorFacade Members

        /// <summary>
        /// Submits a task with the specified parameters.
        /// </summary>
        /// <param name="taskId">The unique ID of the task.</param>
        /// <param name="task">The task to submit.</param>
        /// <param name="summary">The summary to associated with the task, if any.</param>
        /// <param name="priority">The priority of the task.</param>
        internal void SubmitTask(Guid taskId, ITask task, ITaskSummary summary, TaskPriority priority)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            Trace.WriteLine("ENTER: Submitting '{0}' with priority '{1}' ...".FormatInvariant(task.GetType().Name, priority));

            string pollingQueueKey = this.configuration.GetPollingQueueKey(task.GetType());

            this.repository.Tasks.Add(taskId, task);

            if (summary != null)
            {
                this.repository.TaskSummary.Add(taskId, summary);
            }

            ITaskRuntimeInfo taskInfo = this.repository.TaskRuntimeInfo.Create(taskId, task.GetType(), this.dateTimeProvider.UtcNow, priority, pollingQueueKey);

            this.repository.TaskRuntimeInfo.Add(taskInfo);

            ITaskMessageBusSender taskMessageBus = this.messageBus.Tasks.GetSender(task.GetType());

            taskMessageBus.NotifyTaskSubmitted(taskId, this.dateTimeProvider.UtcNow, !string.IsNullOrEmpty(pollingQueueKey));

            Trace.WriteLine("EXIT: Task '{0}' submitted with ID '{1}' ...".FormatInvariant(task, taskId));
        }
    }
}