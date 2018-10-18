using System;
using System.Diagnostics.CodeAnalysis;
using Radoslav.DateTimeProvider;
using Radoslav.TaskProcessor.Configuration;
using Radoslav.TaskProcessor.MessageBus;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.Repository;

namespace Radoslav.TaskProcessor.TaskWorker
{
    /// <summary>
    /// A class responsible to start a task by its ID.
    /// </summary>
    public sealed class TaskWorkerBootstrap
    {
        private readonly ITaskWorkersConfiguration configuration;
        private readonly ITaskProcessorRepository repository;
        private readonly ITaskProcessorMessageBus messageBus;
        private readonly ITaskWorkerFactory taskFactory;
        private readonly IDateTimeProvider dateTimeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskWorkerBootstrap"/> class.
        /// </summary>
        /// <param name="configProvider">The configuration provider to use.</param>
        /// <param name="repository">The repository to use.</param>
        /// <param name="messageBus">The message bus to use.</param>
        /// <param name="taskWorkerFactory">The task worker factory to use.</param>
        /// <param name="dateTimeProvider">The date time provider to use.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="repository"/>, <paramref name="messageBus"/>,
        /// <paramref name="taskWorkerFactory"/> or <paramref name="dateTimeProvider"/> is null.</exception>
        public TaskWorkerBootstrap(ITaskProcessorConfigurationProvider configProvider, ITaskProcessorRepository repository, ITaskProcessorMessageBus messageBus, ITaskWorkerFactory taskWorkerFactory, IDateTimeProvider dateTimeProvider)
        {
            if (configProvider == null)
            {
                throw new ArgumentNullException("configProvider");
            }

            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            if (messageBus == null)
            {
                throw new ArgumentNullException("messageBus");
            }

            if (taskWorkerFactory == null)
            {
                throw new ArgumentNullException("taskWorkerFactory");
            }

            if (dateTimeProvider == null)
            {
                throw new ArgumentNullException("dateTimeProvider");
            }

            this.configuration = configProvider.GetTaskWorkerConfiguration();

            if (this.configuration == null)
            {
                throw new ArgumentException("'{0}' returned null configuration.".FormatInvariant(configProvider.GetType()), nameof(configProvider));
            }

            this.repository = repository;
            this.messageBus = messageBus;
            this.taskFactory = taskWorkerFactory;
            this.dateTimeProvider = dateTimeProvider;
        }

        /// <summary>
        /// Starts a task with the specified ID.
        /// </summary>
        /// <param name="taskId">The ID of the task to start.</param>
        /// <exception cref="ArgumentException">The task with the specified ID is not found or its status is Failed or Success.</exception>
        /// <exception cref="OperationCanceledException">The requested task has been canceled before calling this method.</exception>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = SuppressMessages.FuckOff)]
        public void StartTask(Guid taskId)
        {
            ITask task = this.repository.Tasks.GetById(taskId);

            if (task == null)
            {
                throw new ArgumentException("Task '{0}' was not found in storage.".FormatInvariant(taskId), "taskId");
            }

            ITaskWorker taskWorker = this.taskFactory.CreateTaskWorker(task);

            ITaskMessageBusSender taskMessageBus = this.messageBus.Tasks.GetSender(task.GetType());

            taskWorker.ReportProgress += (sender, e) =>
            {
                this.repository.TaskRuntimeInfo.Progress(taskId, e.Percentage);

                taskMessageBus.NotifyTaskProgress(taskId, e.Percentage);
            };

            this.messageBus.Tasks.Receiver.SubscribeForChannels(MessageBusChannel.TaskCancelRequest);

            bool isCanceled = false;

            this.messageBus.Tasks.Receiver.TaskCancelRequested += (_, e1) =>
            {
                if (!isCanceled && (e1.TaskId == taskId))
                {
                    isCanceled = true;

                    taskWorker.CancelTask();
                }
            };

            ITaskJobSettings settings = null;

            ITaskWorkerConfiguration config = this.configuration[task.GetType()];

            if ((config != null) && config.HasTaskJobSettings)
            {
                settings = this.repository.TaskJobSettings.Get(task.GetType());
            }

            ITaskRuntimeInfo taskInfo = this.repository.TaskRuntimeInfo.GetById(taskId);

            if (taskInfo == null)
            {
                throw new ArgumentException("Task '{0}' not found in storage.".FormatInvariant(taskId), "taskId");
            }

            switch (taskInfo.Status)
            {
                case TaskStatus.Pending:
                case TaskStatus.InProgress:
                    break;

                case TaskStatus.Canceled:
                    throw new OperationCanceledException();

                default:
                    throw new ArgumentException("Task '{0}' status is '{1}'.".FormatInvariant(taskId, taskInfo.Status), "taskId");
            }

            try
            {
                try
                {
                    taskWorker.StartTask(task, settings);
                }
                catch (Exception ex)
                {
                    this.repository.TaskRuntimeInfo.Fail(taskId, this.dateTimeProvider.UtcNow, ex);

                    throw;
                }
            }
            finally
            {
                this.messageBus.Tasks.Receiver.UnsubscribeFromAllChannels();

                if (taskWorker is IDisposable)
                {
                    ((IDisposable)taskWorker).Dispose();
                }
            }
        }
    }
}