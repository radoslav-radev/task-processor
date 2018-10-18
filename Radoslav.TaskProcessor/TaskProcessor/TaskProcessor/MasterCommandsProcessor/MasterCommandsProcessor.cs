using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using Radoslav.TaskProcessor.MessageBus;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.Repository;
using Radoslav.TaskProcessor.TaskDistributor;

namespace Radoslav.TaskProcessor
{
    /// <summary>
    /// Default implementation of the <see cref="IMasterCommandsProcessor"/>.
    /// </summary>
    public sealed class MasterCommandsProcessor : IMasterCommandsProcessor
    {
        #region Fields

        private readonly ITaskProcessorMessageBus messageBus;
        private readonly ITaskProcessorRepository repository;
        private readonly ITaskDistributor taskDistributor;
        private readonly string debugName = typeof(MasterCommandsProcessor).Name;
        private readonly ConcurrentDictionary<Guid, ManualResetEventSlim> assignTaskWaitHandlers = new ConcurrentDictionary<Guid, ManualResetEventSlim>();

        private TimeSpan assignTaskTimeout = TimeSpan.FromSeconds(10);

        private bool isActive;
        private bool isProcessingMasterCommands;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterCommandsProcessor"/> class.
        /// </summary>
        /// <param name="repository">The repository to use.</param>
        /// <param name="messageBus">The message bus to use.</param>
        /// <param name="taskDistributor">The task distributor to use.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="repository"/>, <paramref name="messageBus"/> or <paramref name="taskDistributor"/> is null.</exception>
        public MasterCommandsProcessor(ITaskProcessorRepository repository, ITaskProcessorMessageBus messageBus, ITaskDistributor taskDistributor)
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

            if (taskDistributor == null)
            {
                throw new ArgumentNullException("taskDistributor");
            }

            this.messageBus = messageBus;
            this.repository = repository;
            this.taskDistributor = taskDistributor;

            Trace.WriteLine("EXIT: {0} constructed.".FormatInvariant(this.debugName));
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets the repository used by the master commands processor.
        /// </summary>
        /// <value>The repository used by the master commands processor.</value>
        public ITaskProcessorRepository Repository
        {
            get { return this.repository; }
        }

        /// <summary>
        /// Gets the message bus used by the master commands processor.
        /// </summary>
        /// <value>The message bus used by the master commands processor.</value>
        public ITaskProcessorMessageBus MessageBus
        {
            get { return this.messageBus; }
        }

        /// <summary>
        /// Gets the task distributor used by the master commands processor.
        /// </summary>
        /// <value>The task distributor used by the master commands processor.</value>
        public ITaskDistributor TaskDistributor
        {
            get { return this.taskDistributor; }
        }

        #endregion Properties

        #region IMasterCommandsProcessor

        /// <inheritdoc />
        public bool IsActive
        {
            get
            {
                return this.isActive;
            }

            set
            {
                Trace.WriteLine("ENTER: Setting {0} IsActive to {1} ...".FormatInvariant(this.debugName, value));

                if (this.isActive == value)
                {
                    return;
                }

                if (value)
                {
                    this.messageBus.Tasks.Receiver.TaskStarted += this.OnAssignedTaskStarted;
                    this.messageBus.MasterCommands.MessageReceived += this.OnMasterCommandReceived;
                }
                else
                {
                    this.messageBus.Tasks.Receiver.TaskStarted -= this.OnAssignedTaskStarted;
                    this.messageBus.MasterCommands.MessageReceived -= this.OnMasterCommandReceived;
                }

                this.isActive = value;

                Trace.WriteLine("EXIT: {0} IsActive set to {1}.".FormatInvariant(this.debugName, value));
            }
        }

        /// <inheritdoc />
        public TimeSpan AssignTaskTimeout
        {
            get
            {
                return this.assignTaskTimeout;
            }

            set
            {
                Trace.WriteLine("ENTER: Setting {0} AssignTaskTimeout to {1} ...".FormatInvariant(this.debugName, value));

                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Value must be positive.");
                }

                this.assignTaskTimeout = value;

                Trace.WriteLine("EXIT: {0} AssignTaskTimeout set to {1}.".FormatInvariant(this.debugName, value));
            }
        }

        /// <inheritdoc />
        public void ProcessMasterCommands()
        {
            Trace.WriteLine("ENTER: Processing master commands ...");

            if (this.isProcessingMasterCommands)
            {
                Trace.WriteLine("EXIT: Master commands are currently processes.");

                return;
            }

            this.isProcessingMasterCommands = true;

            while (true)
            {
                if (!this.isActive)
                {
                    Trace.WriteLine("EXIT: {0} deactivated.".FormatInvariant(this));

                    return;
                }

                IUniqueMessage command = this.messageBus.MasterCommands.PopFirst();

                if (command == null)
                {
                    Trace.WriteLine("EXIT: No more master commands to process.");

                    break;
                }

                Trace.WriteLine("Processing {0} ...".FormatInvariant(command));

                if (command is TaskSubmittedMasterCommand)
                {
                    this.OnTaskSubmittedMasterCommand((TaskSubmittedMasterCommand)command);
                }
                else if (command is TaskCancelCompletedMasterCommand)
                {
                    this.OnTaskCancelCompletedMasterCommand((TaskCancelCompletedMasterCommand)command);
                }
                else if (command is TaskFailedMasterCommand)
                {
                    this.OnTaskFailedMasterCommand((TaskFailedMasterCommand)command);
                }
                else if (command is TaskCompletedMasterCommand)
                {
                    this.OnTaskCompletedMasterCommand((TaskCompletedMasterCommand)command);
                }
                else if (command is ConfigurationChangedMasterCommand)
                {
                    this.OnConfigurationChanged((ConfigurationChangedMasterCommand)command);
                }
                else if (command is TaskProcessorRegisteredMasterCommand)
                {
                    this.OnTaskProcessorRegisteredMasterCommand((TaskProcessorRegisteredMasterCommand)command);
                }
                else
                {
                    Trace.TraceWarning("{0} is not supported and will be ignored.".FormatInvariant(command));
                }
            }

            this.isProcessingMasterCommands = false;

            Trace.WriteLine("EXIT: Master commands processed.");
        }

        /// <inheritdoc />
        public void CancelTask(Guid taskId)
        {
        }

        #endregion IMasterCommandsProcessor

        #region Private Methods

        private void OnMasterCommandReceived(object sender, EventArgs e)
        {
            Trace.WriteLine("ENTER: Receiving master command ...");

            this.ProcessMasterCommands();

            Trace.WriteLine("EXIT: Master command received.");
        }

        private void OnTaskProcessorRegisteredMasterCommand(TaskProcessorRegisteredMasterCommand command)
        {
            Trace.WriteLine("ENTER: Handling task processor '{0}' registered ...".FormatInvariant(command.TaskProcessorId));

            this.AssignTasksToProcessor(command.TaskProcessorId);

            Trace.WriteLine("EXIT: Task processor '{0}' registered handled.".FormatInvariant(command.TaskProcessorId));
        }

        private void OnTaskSubmittedMasterCommand(TaskSubmittedMasterCommand command)
        {
            Trace.WriteLine("ENTER: Handling task '{0}' request ...".FormatInvariant(command.TaskId));

            ITaskRuntimeInfo taskInfo = this.repository.TaskRuntimeInfo.GetById(command.TaskId);

            if (taskInfo == null)
            {
                Trace.TraceWarning("EXIT: Task '{0}' not found.".FormatInvariant(command.TaskId));

                return;
            }

            switch (taskInfo.Status)
            {
                case TaskStatus.Pending:
                    break;

                case TaskStatus.Canceled:
                    return;

                default:
                    Trace.TraceWarning("EXIT: Task '{0}' status is '{1}'.".FormatInvariant(command.TaskId, taskInfo.Status));

                    return;
            }

            foreach (ITaskProcessorRuntimeInfo bestProcessor in this.taskDistributor.ChooseProcessorForTask(taskInfo))
            {
                if (this.AssignTaskToProcessor(taskInfo.TaskId, taskInfo.TaskType, bestProcessor.TaskProcessorId))
                {
                    Trace.WriteLine("EXIT: Task '{0}' assigned to processor '{1}'.".FormatInvariant(command.TaskId, bestProcessor.TaskProcessorId));

                    return;
                }
            }

            this.repository.TaskRuntimeInfo.Assign(taskInfo.TaskId, null);

            Trace.TraceWarning("EXIT: All task processors failed to handle task '{0}' assignment.".FormatInvariant(command.TaskId));
        }

        private void AssignTasksToProcessor(Guid taskProcessorId)
        {
            Trace.WriteLine("ENTER: Assigning tasks to processor '{0}' ...".FormatInvariant(taskProcessorId));

            foreach (ITaskRuntimeInfo nextTaskInfo in this.taskDistributor.ChooseNextTasksForProcessor(taskProcessorId))
            {
                this.AssignTaskToProcessor(nextTaskInfo.TaskId, nextTaskInfo.TaskType, taskProcessorId);
            }

            Trace.WriteLine("EXIT: Tasks assigned to processor '{0}'.".FormatInvariant(taskProcessorId));
        }

        private bool AssignTaskToProcessor(Guid taskId, Type taskType, Guid taskProcessorId)
        {
            Trace.WriteLine("ENTER: Assigning task '{0}' to processor '{1}' ...".FormatInvariant(taskId, taskProcessorId));

            this.repository.TaskRuntimeInfo.Assign(taskId, taskProcessorId);

            using (ManualResetEventSlim blocker = new ManualResetEventSlim())
            {
                this.assignTaskWaitHandlers.TryAdd(taskId, blocker);

                ITaskMessageBusSender taskMessageBus = this.messageBus.Tasks.GetSender(taskType);

                ThreadPool.QueueUserWorkItem(state =>
                {
                    taskMessageBus.NotifyTaskAssigned(taskId, taskProcessorId);
                });

                bool result = blocker.Wait(this.assignTaskTimeout);

                ManualResetEventSlim blocker1;

                this.assignTaskWaitHandlers.TryRemove(taskId, out blocker1);

                if (result)
                {
                    Trace.WriteLine("EXIT: Task '{0}' assigned successfully to processor '{1}'.".FormatInvariant(taskId, taskProcessorId));
                }
                else
                {
                    Trace.WriteLine("EXIT: Failed to assign task '{0}' to processor '{1}'. Response was not receive after {2} timeout.".FormatInvariant(taskId, taskProcessorId, this.assignTaskTimeout));
                }

                return result;
            }
        }

        private void OnAssignedTaskStarted(object sender, TaskStartedEventArgs e)
        {
            Trace.WriteLine("ENTER: Handling task '{0}' started by processor '{1}' ...".FormatInvariant(e.TaskId, e.TaskProcessorId));

            ManualResetEventSlim blocker;

            if (this.assignTaskWaitHandlers.TryGetValue(e.TaskId, out blocker))
            {
                blocker.Set();
            }

            Trace.WriteLine("EXIT: Task '{0}' started by processor '{1}' message handled.".FormatInvariant(e.TaskId, e.TaskProcessorId));
        }

        private void OnTaskCancelCompletedMasterCommand(TaskCancelCompletedMasterCommand message)
        {
            Trace.WriteLine("ENTER: Task '{0}' canceled by processor '{1}' ...".FormatInvariant(message.TaskId, message.TaskProcessorId));

            if (!message.IsTaskProcessorStopping)
            {
                this.AssignTasksToProcessor(message.TaskProcessorId);
            }

            Trace.WriteLine("EXIT: Task '{0}' canceled by processor '{1}'.".FormatInvariant(message.TaskId, message.TaskProcessorId));
        }

        private void OnTaskFailedMasterCommand(TaskFailedMasterCommand message)
        {
            Trace.WriteLine("ENTER: Task '{0}' in processor '{1}' failed ...".FormatInvariant(message.TaskId, message.TaskProcessorId));

            if (!message.IsTaskProcessorStopping)
            {
                this.AssignTasksToProcessor(message.TaskProcessorId);
            }

            Trace.WriteLine("EXIT: Task '{0}' failed in processor '{1}' failed.".FormatInvariant(message.TaskId, message.TaskProcessorId));
        }

        private void OnTaskCompletedMasterCommand(TaskCompletedMasterCommand message)
        {
            Trace.WriteLine("ENTER: Task '{0}' completed by processor '{1}' ...".FormatInvariant(message.TaskId, message.TaskProcessorId));

            if (!message.IsTaskProcessorStopping)
            {
                this.AssignTasksToProcessor(message.TaskProcessorId);
            }

            Trace.WriteLine("EXIT: Task '{0}' completed by processor '{1}'.".FormatInvariant(message.TaskId, message.TaskProcessorId));
        }

        private void OnConfigurationChanged(ConfigurationChangedMasterCommand command)
        {
            Trace.WriteLine("ENTER: Task processor '{0}' configuration changed ...".FormatInvariant(command.TaskProcessorId));

            this.AssignTasksToProcessor(command.TaskProcessorId);

            Trace.WriteLine("EXIT: Task processor '{0}' configuration changed.".FormatInvariant(command.TaskProcessorId));
        }

        #endregion Private Methods
    }
}