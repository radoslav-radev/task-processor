using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Radoslav.DateTimeProvider;
using Radoslav.Retryable;
using Radoslav.Retryable.DelayStrategy;
using Radoslav.ServiceLocator;
using Radoslav.TaskProcessor.Configuration;
using Radoslav.TaskProcessor.MessageBus;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.Repository;
using Radoslav.Timers;

namespace Radoslav.TaskProcessor
{
    /// <summary>
    /// Class representing a task processor instance.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This is the central class of the whole application and it is normal to be very complex.")]
    public sealed partial class RadoslavTaskProcessor : IDisposable
    {
        /// <summary>
        /// The default interval between two task processor heartbeats.
        /// </summary>
        public static readonly TimeSpan DefaultHeartbeatInterval = TimeSpan.FromSeconds(5);

        /// <summary>
        /// The default time interval after which the task processor stops performance monitoring
        /// if a message from a client to continue the performance monitoring is not received.
        /// </summary>
        public static readonly TimeSpan DefaultMonitoringTimeout = TimeSpan.FromSeconds(10);

        #region Fields

        private readonly Guid taskProcessorId = Guid.NewGuid();
        private readonly ITaskProcessorConfigurationProvider appConfigConfigurationProvider;
        private readonly ITimer heartbeatTimer;
        private readonly ITaskProcessorMessageBus messageBus;
        private readonly ITaskProcessorRepository repository;
        private readonly ITaskExecutor taskExecutor;
        private readonly IMasterCommandsProcessor masterCommandsProcessor;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly ITimer reportPerformanceTimer;
        private readonly IApplicationKiller applicationKiller;
        private readonly IDelayStrategy retryHeartbeatDelayStrategy;
        private readonly IRadoslavServiceLocator locator;

        private readonly string debugName = typeof(RadoslavTaskProcessor).Name;
        private readonly ConcurrentDictionary<ITimer, PollingJobRuntimeInfo> pollingJobsByTimer = new ConcurrentDictionary<ITimer, PollingJobRuntimeInfo>();
        private readonly ConcurrentDictionary<ITimer, PollingQueueRuntimeInfo> pollingQueuesByTimer = new ConcurrentDictionary<ITimer, PollingQueueRuntimeInfo>();
        private readonly ConcurrentDictionary<Guid, PollingQueueRuntimeInfo> tasksPollingQueues = new ConcurrentDictionary<Guid, PollingQueueRuntimeInfo>();

        private DateTime lastPerformanceMonitoringPingUtc;
        private bool isMaster;
        private TaskProcessorState taskProcessorState;
        private PerformanceCounter cpuPerformanceCounter;
        private PerformanceCounter ramPerformanceCounter;
        private int maxHeartbeatRetries = 3;
        private TimeSpan monitoringTimeout = RadoslavTaskProcessor.DefaultMonitoringTimeout;

        #endregion Fields

        #region Constructor & Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RadoslavTaskProcessor"/> class.
        /// </summary>
        /// <param name="appConfigConfigurationProvider">The provider that should read the configuration from the App.config file.</param>
        /// <param name="repository">The repository to use.</param>
        /// <param name="messageBus">The message bus to use.</param>
        /// <param name="taskExecutor">The task executor to use.</param>
        /// <param name="masterCommandsProcessor">The master commands processor to use.</param>
        /// <param name="dateTimeProvider">The date time provider to use.</param>
        /// <param name="applicationKiller">The application killer used to terminate the entire application if necessarily.</param>
        /// <param name="retryHeartbeatDelayStrategy">The delay strategy for heartbeat retries.</param>
        /// <param name="locator">The service locator to use for creating polling jobs, etc.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="repository"/>, <paramref name="messageBus"/>,
        /// <paramref name="taskExecutor"/>, <paramref name="masterCommandsProcessor"/>, <paramref name="dateTimeProvider"/>,
        /// <paramref name="applicationKiller"/>, <paramref name="retryHeartbeatDelayStrategy"/> or <paramref name="locator"/>  is null.</exception>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = SuppressMessages.FuckOff)]
        public RadoslavTaskProcessor(
            ITaskProcessorConfigurationProvider appConfigConfigurationProvider,
            ITaskProcessorRepository repository,
            ITaskProcessorMessageBus messageBus,
            ITaskExecutor taskExecutor,
            IMasterCommandsProcessor masterCommandsProcessor,
            IDateTimeProvider dateTimeProvider,
            IApplicationKiller applicationKiller,
            IDelayStrategy retryHeartbeatDelayStrategy,
            IRadoslavServiceLocator locator)
        {
            Trace.WriteLine("ENTER: Constructing {0} {1} ...".FormatInvariant(this.debugName, this.taskProcessorId));

            if (appConfigConfigurationProvider == null)
            {
                throw new ArgumentNullException("appConfigConfigurationProvider");
            }

            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            if (messageBus == null)
            {
                throw new ArgumentNullException("messageBus");
            }

            if (taskExecutor == null)
            {
                throw new ArgumentNullException("taskExecutor");
            }

            if (masterCommandsProcessor == null)
            {
                throw new ArgumentNullException("masterCommandsProcessor");
            }

            if (dateTimeProvider == null)
            {
                throw new ArgumentNullException("dateTimeProvider");
            }

            if (applicationKiller == null)
            {
                throw new ArgumentNullException("applicationKiller");
            }

            if (retryHeartbeatDelayStrategy == null)
            {
                throw new ArgumentNullException("retryHeartbeatDelayStrategy");
            }

            if (locator == null)
            {
                throw new ArgumentNullException("locator");
            }

            this.appConfigConfigurationProvider = appConfigConfigurationProvider;
            this.repository = repository;
            this.messageBus = messageBus;
            this.taskExecutor = taskExecutor;
            this.masterCommandsProcessor = masterCommandsProcessor;
            this.dateTimeProvider = dateTimeProvider;
            this.applicationKiller = applicationKiller;
            this.retryHeartbeatDelayStrategy = retryHeartbeatDelayStrategy;
            this.locator = locator;

            this.heartbeatTimer = locator.ResolveSingle<ITimer>();
            this.reportPerformanceTimer = locator.ResolveSingle<ITimer>();

            this.messageBus.TaskProcessors.Receiver.MasterModeChangeRequested += this.OnMasterModeChangeRequested;
            this.messageBus.TaskProcessors.Receiver.MasterModeChanged += this.OnMasterModeChanged;
            this.messageBus.TaskProcessors.Receiver.StopRequested += this.OnTaskProcessorStopRequested;
            this.messageBus.TaskProcessors.Receiver.PerformanceMonitoringRequested += this.OnPerformanceMonitoringStarted;
            this.messageBus.TaskProcessors.Receiver.ConfigurationChanged += this.OnConfigurationChanged;

            this.messageBus.Tasks.Receiver.TaskAssigned += this.OnTaskAssigned;
            this.messageBus.Tasks.Receiver.TaskCancelRequested += this.OnTaskCanceled;

            this.heartbeatTimer.Tick += this.OnHeartbeatTimerTick;
            this.taskExecutor.TaskCanceled += this.OnTaskProcessCanceled;
            this.taskExecutor.TaskFailed += this.OnTaskProcessFailed;
            this.taskExecutor.TaskCompleted += this.OnTaskProcessCompleted;
            this.reportPerformanceTimer.Tick += this.OnReportPerformanceTimerTick;

            this.heartbeatTimer.Interval = RadoslavTaskProcessor.DefaultHeartbeatInterval;

            Trace.WriteLine("EXIT: {0} constructed.".FormatInvariant(this.debugName));
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="RadoslavTaskProcessor"/> class.
        /// </summary>
        ~RadoslavTaskProcessor()
        {
            Trace.WriteLine("ENTER: Destructing {0} ...".FormatInvariant(this.debugName));

            this.Dispose(false);

            Trace.WriteLine("EXIT: {0} destructed.".FormatInvariant(this.debugName));
        }

        #endregion Constructor & Destructor

        #region Events

        /// <summary>
        /// An event raised when the task processor has stopped.
        /// </summary>
        /// <remarks>This happens when task processor has been requested to stop via the message bus and all active tasks have completed.</remarks>
        public event EventHandler Stopped;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets the ID of the task processor.
        /// </summary>
        /// <value>The ID of the task processor.</value>
        public Guid Id
        {
            get { return this.taskProcessorId; }
        }

        /// <summary>
        /// Gets the current state of the task processor.
        /// </summary>
        /// <value>The current state of the task processor.</value>
        public TaskProcessorState State
        {
            get { return this.taskProcessorState; }
        }

        /// <summary>
        /// Gets the repository used by the task processor.
        /// </summary>
        /// <value>The repository used by the task processor.</value>
        public ITaskProcessorRepository Repository
        {
            get { return this.repository; }
        }

        /// <summary>
        /// Gets the message bus used by the task processor.
        /// </summary>
        /// <value>The message bus used by the task processor.</value>
        public ITaskProcessorMessageBus MessageBus
        {
            get { return this.messageBus; }
        }

        /// <summary>
        /// Gets the timer used by the task processor for heart-beating.
        /// </summary>
        /// <value>The timer used by the task processor for heart-beating.</value>
        public ITimer HeartbeatTimer
        {
            get { return this.heartbeatTimer; }
        }

        /// <summary>
        /// Gets the task executor used by the task processor.
        /// </summary>
        /// <value>The task executor used by the task processor.</value>
        public ITaskExecutor TaskExecutor
        {
            get { return this.taskExecutor; }
        }

        /// <summary>
        /// Gets the date time provider used by the task processor.
        /// </summary>
        /// <value>The date time provider used by the task processor.</value>
        public IDateTimeProvider DateTimeProvider
        {
            get { return this.dateTimeProvider; }
        }

        /// <summary>
        /// Gets the master commands processor used by the task processor.
        /// </summary>
        /// <value>The master commands processor used by the task processor.</value>
        public IMasterCommandsProcessor MasterCommandsProcessor
        {
            get { return this.masterCommandsProcessor; }
        }

        /// <summary>
        /// Gets the timer to use for performance monitoring.
        /// </summary>
        /// <value>The timer to use for performance monitoring.</value>
        public ITimer ReportPerformanceTimer
        {
            get { return this.reportPerformanceTimer; }
        }

        /// <summary>
        /// Gets the application killer used to terminate the entire application if necessarily.
        /// </summary>
        /// <value>The application killer used to terminate the entire application if necessarily.</value>
        public IApplicationKiller ApplicationKiller
        {
            get { return this.applicationKiller; }
        }

        /// <summary>
        /// Gets the delay strategy for heartbeat retries.
        /// </summary>
        /// <value>The delay strategy for heartbeat retries.</value>
        public IDelayStrategy RetryHeartbeatDelayStrategy
        {
            get { return this.retryHeartbeatDelayStrategy; }
        }

        /// <summary>
        /// Gets the provider used to retrieve the configuration from the App.config file.
        /// </summary>
        /// <value>The provider used to retrieve the configuration from the App.config file.</value>
        public ITaskProcessorConfigurationProvider AppConfigConfigurationProvider
        {
            get { return this.appConfigConfigurationProvider; }
        }

        /// <summary>
        /// Gets the service locator used to resolve polling jobs, etc.
        /// </summary>
        /// <value>The service locator used to resolve polling jobs, etc.</value>
        public IRadoslavServiceLocator ServiceLocator
        {
            get { return this.locator; }
        }

        /// <summary>
        /// Gets or sets the timeout after which the task processor will stop to send performance report messages via message bus.
        /// </summary>
        /// <value>The timeout after which the task processor will stop to send performance report messages via message bus.</value>
        public TimeSpan MonitoringTimeout
        {
            get
            {
                return this.monitoringTimeout;
            }

            set
            {
                Trace.WriteLine("ENTER: Setting {0} MonitoringTimeout to {1} ...".FormatInvariant(this.debugName, value));

                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Value must be positive.");
                }

                this.monitoringTimeout = value;

                Trace.WriteLine("EXIT: {0} MonitoringTimeout set to {1}.".FormatInvariant(this.debugName, value));
            }
        }

        /// <summary>
        /// Gets or sets the maximum heartbeat retries.
        /// </summary>
        /// <remarks>The default value is 3.</remarks>
        /// <value>The maximum heartbeat retries.</value>
        /// <exception cref="ArgumentOutOfRangeException">Value is less than or equal to 0.</exception>
        public int MaxHeartbeatRetries
        {
            get
            {
                return this.maxHeartbeatRetries;
            }

            set
            {
                Trace.WriteLine("ENTER: Setting {0} MaxHeartbeatRetries to {1} ...".FormatInvariant(this.debugName, value));

                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Value must be positive.");
                }

                this.maxHeartbeatRetries = value;

                Trace.WriteLine("EXIT: {0} MaxHeartbeatRetries set to {1}.".FormatInvariant(this.debugName, value));
            }
        }

        #endregion Properties

        #region IDisposable Members

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members

        #region Start

        /// <summary>
        /// Starts the task processor.
        /// </summary>
        /// <exception cref="InvalidOperationException">Task processor is currently stopping or the heartbeat timer
        /// interval is greater than or equal to the task processor runtime information expiration in repository.</exception>
        /// <exception cref="ObjectDisposedException">Task processor has been disposed.</exception>
        public void Start()
        {
            Trace.WriteLine("ENTER: Starting {0} ...".FormatInvariant(this.debugName));

            switch (this.taskProcessorState)
            {
                case TaskProcessorState.Active:
                    Trace.WriteLine("EXIT: {0} is already started.".FormatInvariant(this.debugName));
                    return;

                case TaskProcessorState.Stopping:
                    throw new InvalidOperationException("{0} is currently stopping.".FormatInvariant(this.debugName));

                case TaskProcessorState.Disposed:
                    throw new ObjectDisposedException(this.debugName);
            }

            if (this.heartbeatTimer.Interval >= this.repository.TaskProcessorRuntimeInfo.Expiration)
            {
                throw new InvalidOperationException("Heartbeat interval {0} is greater than task processor runtime info expiration {1}.".FormatInvariant(this.heartbeatTimer.Interval, this.repository.TaskProcessorRuntimeInfo.Expiration));
            }

            ITaskProcessorRuntimeInfo processorInfo = this.repository.TaskProcessorRuntimeInfo.Create(this.taskProcessorId, Environment.MachineName);

            ITaskProcessorConfiguration configuration = this.appConfigConfigurationProvider.GetTaskProcessorConfiguration();

            if (configuration != null)
            {
                processorInfo.Configuration.MergeWith(configuration);
            }

            this.messageBus.Tasks.Receiver.SubscribeForChannels(
                MessageBusChannel.TaskAssigned,
                MessageBusChannel.TaskCancelRequest);

            this.messageBus.TaskProcessors.Receiver.SubscribeForChannels(
                MessageBusChannel.MasterModeChangeRequest,
                MessageBusChannel.MasterModeChanged,
                MessageBusChannel.StopTaskProcessor,
                MessageBusChannel.PerformanceMonitoringRequest,
                MessageBusChannel.ConfigurationChanged);

            this.repository.TaskProcessorRuntimeInfo.Add(processorInfo);

            this.heartbeatTimer.Start();

            this.messageBus.TaskProcessors.Sender.NotifyStateChanged(this.taskProcessorId, TaskProcessorState.Active);

            this.taskProcessorState = TaskProcessorState.Active;

            this.isMaster = this.repository.TaskProcessorRuntimeInfo.SetMasterIfNotExists(this.taskProcessorId);

            if (this.isMaster)
            {
                Trace.WriteLine("{0} registered as master.".FormatInvariant(this.debugName));

                ThreadPool.QueueUserWorkItem(state => this.OnBecameMaster(MasterModeChangeReason.Start, false));
            }
            else
            {
                Trace.WriteLine("{0} registered as slave.".FormatInvariant(this.debugName));
            }

            this.UpdatePollingQueues(processorInfo.Configuration.PollingQueues);
            this.UpdatePollingJobs(processorInfo.Configuration.PollingJobs);

            Trace.WriteLine("EXIT: {0} started.".FormatInvariant(this.debugName));
        }

        #endregion Start

        #region Assign / Cancel / Complete Task

        private void OnTaskAssigned(object sender, TaskAssignedEventArgs e)
        {
            Trace.WriteLine("ENTER: Task '{0}' assigned to processor '{1}' ...".FormatInvariant(e.TaskId, e.TaskProcessorId));

            if (this.taskProcessorId != e.TaskProcessorId)
            {
                Trace.WriteLine("EXIT: Task '{0}' assigned to processor '{1}' but that's not me.".FormatInvariant(e.TaskId, e.TaskProcessorId));

                return;
            }

            switch (this.taskProcessorState)
            {
                case TaskProcessorState.Inactive:
                case TaskProcessorState.Stopping:
                case TaskProcessorState.Disposed:
                    Trace.WriteLine("EXIT: Task '{0}' assigned to {1} but my state is {2}. Task will be ignored.".FormatInvariant(e.TaskId, this.debugName, this.taskProcessorState));
                    return;
            }

            ITaskRuntimeInfo taskInfo = this.repository.TaskRuntimeInfo.GetById(e.TaskId);

            if (taskInfo == null)
            {
                Trace.WriteLine("EXIT: Task '{0}' not found in storage and will be ignored by {1}.".FormatInvariant(e.TaskId, this.debugName));

                return;
            }

            if (taskInfo.Status != TaskStatus.Pending)
            {
                Trace.WriteLine("Task '{0}' status is {1} and will be ignored.".FormatInvariant(taskInfo.TaskId, taskInfo.Status));

                return;
            }

            if (taskInfo.TaskProcessorId != this.taskProcessorId)
            {
                Trace.WriteLine("Task '{0}' is already assigned to another task processor.".FormatInvariant(taskInfo.TaskId));

                return;
            }

            this.taskExecutor.StartTask(taskInfo.TaskId, taskInfo.Priority);

            DateTime timestampUtc = this.dateTimeProvider.UtcNow;

            this.repository.TaskRuntimeInfo.Start(taskInfo.TaskId, this.taskProcessorId, timestampUtc);

            ITaskMessageBusSender taskMessageBus = this.messageBus.Tasks.GetSender(taskInfo.TaskType);

            taskMessageBus.NotifyTaskStarted(taskInfo.TaskId, this.taskProcessorId, timestampUtc);

            Trace.WriteLine("EXIT: Task '{0}' assigned to processor '{1}'.".FormatInvariant(e.TaskId, e.TaskProcessorId));
        }

        private void OnTaskCanceled(object sender, TaskEventArgs e)
        {
            Trace.WriteLine("ENTER: Handling task '{0}' canceled ...".FormatInvariant(e.TaskId));

            this.taskExecutor.CancelTask(e.TaskId);

            if (this.isMaster)
            {
                this.masterCommandsProcessor.CancelTask(e.TaskId);
            }

            Trace.WriteLine("EXIT: Task '{0}' canceled.".FormatInvariant(e.TaskId));
        }

        private void OnTaskProcessCanceled(object sender, TaskEventArgs e)
        {
            Trace.WriteLine("ENTER: Task '{0}' cancel completed.".FormatInvariant(e.TaskId));

            Type taskType = this.repository.TaskRuntimeInfo.GetTaskType(e.TaskId);

            this.repository.TaskRuntimeInfo.CompleteCancel(e.TaskId, this.dateTimeProvider.UtcNow);

            if (taskType == null)
            {
                Trace.TraceWarning("Task type for task '{0}' was not found.", e.TaskId);
            }
            else
            {
                ITaskMessageBusSender taskMessageBusSender = this.messageBus.Tasks.GetSender(taskType);

                taskMessageBusSender.NotifyTaskCancelCompleted(e.TaskId, this.dateTimeProvider.UtcNow, this.taskProcessorId, this.taskProcessorState == TaskProcessorState.Stopping);
            }

            this.OnTaskProcessExit(e.TaskId);

            Trace.WriteLine("EXIT: Task '{0}' cancel completed.".FormatInvariant(e.TaskId));
        }

        private void OnTaskProcessFailed(object sender, TaskEventArgs e)
        {
            Trace.WriteLine("ENTER: Task '{0}' failed.".FormatInvariant(e.TaskId));

            ITaskRuntimeInfo taskInfo = this.repository.TaskRuntimeInfo.GetById(e.TaskId);

            if (taskInfo == null)
            {
                Trace.TraceWarning("EXIT: Task '{0}' not found in storage.".FormatInvariant(e.TaskId));

                return;
            }

            ITaskMessageBusSender taskMessageBusSender = this.messageBus.Tasks.GetSender(taskInfo.TaskType);

            taskMessageBusSender.NotifyTaskFailed(e.TaskId, this.dateTimeProvider.UtcNow, this.taskProcessorId, this.taskProcessorState == TaskProcessorState.Stopping, taskInfo.Error);

            this.OnTaskProcessExit(e.TaskId);

            Trace.WriteLine("EXIT: Task '{0}' failed.".FormatInvariant(e.TaskId));
        }

        private void OnTaskProcessCompleted(object sender, TaskCompletedEventArgs e)
        {
            Trace.WriteLine("ENTER: Task '{0}' completed.".FormatInvariant(e.TaskId));

            Type taskType = this.repository.TaskRuntimeInfo.GetTaskType(e.TaskId);

            this.repository.TaskRuntimeInfo.Complete(e.TaskId, this.dateTimeProvider.UtcNow);

            if (taskType == null)
            {
                Trace.TraceWarning("Task type for task '{0}' was not found.", e.TaskId);
            }
            else
            {
                ITaskMessageBusSender taskMessageBusSender = this.messageBus.Tasks.GetSender(taskType);

                taskMessageBusSender.NotifyTaskCompleted(e.TaskId, this.dateTimeProvider.UtcNow, this.taskProcessorId, this.taskProcessorState == TaskProcessorState.Stopping, e.TotalCpuTime);
            }

            this.OnTaskProcessExit(e.TaskId);

            Trace.WriteLine("EXIT: Task '{0}' completed.".FormatInvariant(e.TaskId));
        }

        private void OnTaskProcessExit(Guid taskId)
        {
            this.repository.Tasks.Delete(taskId);

            PollingQueueRuntimeInfo pollingQueueInfo;

            if (this.tasksPollingQueues.TryGetValue(taskId, out pollingQueueInfo))
            {
                pollingQueueInfo.DecrementActiveTasksCount();

                this.tasksPollingQueues.TryRemove(taskId, out pollingQueueInfo);
            }

            switch (this.taskProcessorState)
            {
                case TaskProcessorState.Stopping:
                    if (this.taskExecutor.ActiveTasksCount == 0)
                    {
                        Trace.WriteLine("All active tasks of task processor '{0}' completed.".FormatInvariant(this.debugName));

                        this.OnStopped();
                    }

                    break;
            }
        }

        #endregion Assign / Cancel / Complete Task

        #region Master / Slave

        private void OnBecameMaster(MasterModeChangeReason reason, bool updatePollingJobs)
        {
            Trace.WriteLine("ENTER: {0} became master ...".FormatInvariant(this.debugName));

            this.messageBus.TaskProcessors.Sender.NotifyMasterModeChanged(this.taskProcessorId, true, reason);

            this.messageBus.MasterCommands.ReceiveMessages = true;

            this.messageBus.Tasks.Receiver.SubscribeForChannels(MessageBusChannel.TaskStarted);

            this.masterCommandsProcessor.IsActive = true;

            this.masterCommandsProcessor.ProcessMasterCommands();

            if (updatePollingJobs)
            {
                this.UpdatePollingJobsAndQueues();
            }

            Trace.WriteLine("EXIT: {0} became master.".FormatInvariant(this.debugName));
        }

        private void OnBecameSlave(MasterModeChangeReason reason)
        {
            Trace.WriteLine("ENTER: {0} became slave ...".FormatInvariant(this.debugName));

            this.isMaster = false;

            this.messageBus.MasterCommands.ReceiveMessages = false;

            this.messageBus.Tasks.Receiver.UnsubscribeFromChannels(MessageBusChannel.TaskStarted);

            this.masterCommandsProcessor.IsActive = false;

            this.messageBus.TaskProcessors.Sender.NotifyMasterModeChanged(this.taskProcessorId, false, reason);

            this.UpdatePollingJobsAndQueues();

            Trace.WriteLine("EXIT: {0} became slave.".FormatInvariant(this.debugName));
        }

        private void OnMasterModeChangeRequested(object sender, MasterModeChangeEventArgs e)
        {
            Trace.WriteLine("ENTER: Master mode change request {0} for task processor '{1}' ...".FormatInvariant(e.IsMaster, e.TaskProcessorId));

            switch (this.taskProcessorState)
            {
                case TaskProcessorState.Inactive:
                case TaskProcessorState.Stopping:
                case TaskProcessorState.Disposed:
                    Trace.WriteLine("EXIT: Ignore master mode change request because my state is {0}.".FormatInvariant(this.taskProcessorState));

                    return;
            }

            if (e.IsMaster)
            {
                if (e.TaskProcessorId == this.taskProcessorId)
                {
                    if (this.isMaster)
                    {
                        Trace.WriteLine("EXIT: Task processor '{0}' is already the master.".FormatInvariant(this.taskProcessorId));

                        return;
                    }
                    else
                    {
                        this.isMaster = true;

                        this.OnBecameMaster(MasterModeChangeReason.Explicit, true);
                    }
                }
                else
                {
                    if (this.isMaster)
                    {
                        this.OnBecameSlave(MasterModeChangeReason.Explicit);
                    }
                    else
                    {
                        Trace.WriteLine("EXIT: Master mode change request is not for me and I am not the master.");

                        return;
                    }
                }
            }
            else
            {
                if (e.TaskProcessorId == this.taskProcessorId)
                {
                    if (this.isMaster)
                    {
                        this.OnBecameSlave(MasterModeChangeReason.Explicit);
                    }
                    else
                    {
                        Trace.WriteLine("EXIT: Master mode change request is for me but I am already slave.");

                        return;
                    }
                }
                else
                {
                    Trace.WriteLine("EXIT: Master mode change request to slave is not for me.");

                    return;
                }
            }

            Trace.WriteLine("EXIT: Master mode change request {0} for task processor '{1}'.".FormatInvariant(e.IsMaster, e.TaskProcessorId));
        }

        private void OnMasterModeChanged(object sender, MasterModeChangeEventArgs e)
        {
            Trace.WriteLine("ENTER: Master mode changed to {0} for task processor '{1}' with reason {2} ...".FormatInvariant(e.IsMaster, e.TaskProcessorId, e.Reason));

            switch (this.taskProcessorState)
            {
                case TaskProcessorState.Inactive:
                case TaskProcessorState.Stopping:
                case TaskProcessorState.Disposed:
                    Trace.WriteLine("EXIT: Ignore master mode change because my state is {0}.".FormatInvariant(this.taskProcessorState));

                    return;
            }

            if (!this.isMaster && !e.IsMaster && (e.Reason == MasterModeChangeReason.Stop) && (e.TaskProcessorId != this.taskProcessorId))
            {
                if (this.repository.TaskProcessorRuntimeInfo.SetMasterIfNotExists(this.taskProcessorId))
                {
                    this.isMaster = true;

                    this.OnBecameMaster(MasterModeChangeReason.Stop, true);
                }
            }

            Trace.WriteLine("EXIT: Master mode changed to {0} for task processor '{1}' with reason {2}.".FormatInvariant(e.IsMaster, e.TaskProcessorId, e.Reason));
        }

        #endregion Master / Slave

        #region Stop

        private void OnTaskProcessorStopRequested(object sender, TaskProcessorEventArgs e)
        {
            Trace.WriteLine("ENTER: Task processor '{0}' stop requested ...".FormatInvariant(e.TaskProcessorId));

            if (this.taskProcessorId != e.TaskProcessorId)
            {
                Trace.WriteLine("EXIT: Event is not for me.");

                return;
            }

            switch (this.taskProcessorState)
            {
                case TaskProcessorState.Inactive:
                case TaskProcessorState.Stopping:
                case TaskProcessorState.Disposed:
                    Trace.WriteLine("EXIT: {0} state is {1}.".FormatInvariant(this.debugName));

                    return;
            }

            this.taskProcessorState = TaskProcessorState.Stopping;

            this.messageBus.TaskProcessors.Sender.NotifyStateChanged(this.taskProcessorId, TaskProcessorState.Stopping);

            this.masterCommandsProcessor.IsActive = false;

            if (this.taskProcessorId == this.repository.TaskProcessorRuntimeInfo.GetMasterId())
            {
                this.repository.TaskProcessorRuntimeInfo.ClearMaster();

                this.isMaster = false;

                this.messageBus.TaskProcessors.Sender.NotifyMasterModeChanged(this.taskProcessorId, false, MasterModeChangeReason.Stop);
            }

            this.StopPollingQueues();
            this.StopPollingJobs();

            this.repository.TaskProcessorRuntimeInfo.Delete(this.taskProcessorId);

            this.heartbeatTimer.Stop();

            this.messageBus.MasterCommands.ReceiveMessages = false;

            if (this.taskExecutor.ActiveTasksCount > 0)
            {
                Trace.WriteLine("There are {0} active tasks. Waiting them to complete ...".FormatInvariant(this.taskExecutor.ActiveTasksCount));

                this.messageBus.Tasks.Receiver.UnsubscribeFromAllChannelsExcept(MessageBusChannel.TaskCancelRequest);
                this.messageBus.TaskProcessors.Receiver.UnsubscribeFromAllChannelsExcept(MessageBusChannel.PerformanceMonitoringRequest);
            }
            else
            {
                this.OnStopped();
            }

            Trace.WriteLine("EXIT: Task processor '{0}' stop requested.".FormatInvariant(e.TaskProcessorId));
        }

        private void OnStopped()
        {
            this.taskProcessorState = TaskProcessorState.Inactive;

            this.messageBus.TaskProcessors.Receiver.UnsubscribeFromAllChannels();
            this.messageBus.Tasks.Receiver.UnsubscribeFromAllChannels();

            this.messageBus.TaskProcessors.Sender.NotifyStateChanged(this.taskProcessorId, TaskProcessorState.Inactive);

            this.StopPerformanceMonitoring();

            EventHandler stopEventHandler = this.Stopped;

            if (stopEventHandler != null)
            {
                stopEventHandler(this, EventArgs.Empty);
            }

            Trace.WriteLine("EXIT: {0} stopped.".FormatInvariant(this));
        }

        #endregion Stop

        #region Performance Monitoring

        private void OnPerformanceMonitoringStarted(object sender, PerformanceMonitoringEventArgs e)
        {
            Trace.WriteLine("ENTER: Starting performance monitoring and send report every {0} ...".FormatInvariant(e.RefreshInterval));

            switch (this.taskProcessorState)
            {
                case TaskProcessorState.Inactive:
                case TaskProcessorState.Disposed:
                    Trace.WriteLine("EXIT: {0} status is {1}. Performance monitoring ping will be ignored.".FormatInvariant(this, this.taskProcessorState));
                    return;
            }

            this.taskExecutor.MonitorPerformance = true;

            if (this.cpuPerformanceCounter == null)
            {
                this.cpuPerformanceCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            }

            if (this.ramPerformanceCounter == null)
            {
                this.ramPerformanceCounter = new PerformanceCounter("Memory", "Available MBytes", true);
            }

            this.lastPerformanceMonitoringPingUtc = this.dateTimeProvider.UtcNow;

            this.reportPerformanceTimer.Interval = e.RefreshInterval;

            if (!this.reportPerformanceTimer.IsActive)
            {
                this.reportPerformanceTimer.Start();
            }

            Trace.WriteLine("EXIT: Performance monitoring started.");
        }

        private void StopPerformanceMonitoring()
        {
            Trace.WriteLine("ENTER: Stopping performance monitoring ...");

            this.reportPerformanceTimer.Stop();

            this.taskExecutor.MonitorPerformance = false;

            if (this.ramPerformanceCounter != null)
            {
                this.ramPerformanceCounter.Dispose();

                this.ramPerformanceCounter = null;
            }

            if (this.cpuPerformanceCounter != null)
            {
                this.cpuPerformanceCounter.Dispose();

                this.cpuPerformanceCounter = null;
            }

            Trace.WriteLine("EXIT: Performance monitoring stopped.");
        }

        private void OnReportPerformanceTimerTick(object sender, EventArgs e)
        {
            Trace.WriteLine("ENTER: Reporting performance ...");

            switch (this.taskProcessorState)
            {
                case TaskProcessorState.Inactive:
                case TaskProcessorState.Disposed:
                    Trace.WriteLine("EXIT: {0} status is {1}. Performance monitoring timer tick will be ignored.".FormatInvariant(this, this.taskProcessorState));
                    return;
            }

            if (this.dateTimeProvider.UtcNow - this.lastPerformanceMonitoringPingUtc > this.monitoringTimeout)
            {
                Trace.WriteLine("EXIT: Nobody is interested in performance for the last {0}. Monitoring will be stopped.".FormatInvariant(this.monitoringTimeout));

                this.StopPerformanceMonitoring();

                return;
            }

            ulong freeRamInBytes = Convert.ToUInt64(Math.Round(1024 * 1024 * this.ramPerformanceCounter.NextValue()));

            TaskProcessorPerformanceReport taskProcessorPerformanceInfo = new TaskProcessorPerformanceReport(this.taskProcessorId)
            {
                CpuPercent = Convert.ToInt32(Math.Round(this.cpuPerformanceCounter.NextValue())),
                RamPercent = Convert.ToInt32(Math.Round(100.0 * (ComputerInfo.TotalPhysicalMemory - freeRamInBytes) / ComputerInfo.TotalPhysicalMemory)),
            };

            foreach (TaskPerformanceReport taskPerformance in this.taskExecutor.GetTasksPerformanceInfo())
            {
                taskProcessorPerformanceInfo.TasksPerformance.Add(taskPerformance);
            }

            this.messageBus.TaskProcessors.Sender.NotifyPerformanceReport(taskProcessorPerformanceInfo);

            Trace.WriteLine("EXIT: Performance reported.");
        }

        #endregion Performance Monitoring

        #region Heartbeat

        private bool TryToHearbeat(bool isMasterHeartbeat)
        {
            Trace.WriteLine("ENTER: Try to heartbeat {0} ...".FormatInvariant(isMasterHeartbeat ? "as master" : string.Empty));

            bool result = false;

            using (RetryableOperation operation = new RetryableOperation())
            {
                operation.ExecuteOperation += (sender, e) =>
                    {
                        if (isMasterHeartbeat)
                        {
                            result = this.repository.TaskProcessorRuntimeInfo.MasterHeartbeat();
                        }
                        else
                        {
                            result = this.repository.TaskProcessorRuntimeInfo.Heartbeat(this.taskProcessorId);
                        }
                    };

                operation.RetryFailed += (sender, e) =>
                    {
                        Trace.WriteLine("Heartbeat failed {0} of {1}: {2}".FormatInvariant(e.FailedRetriesCount, this.maxHeartbeatRetries, e.Error));
                    };

                if (operation.Execute(this.maxHeartbeatRetries, this.retryHeartbeatDelayStrategy))
                {
                    Trace.WriteLine("EXIT: {0} heartbeat succeeded.".FormatInvariant(isMasterHeartbeat ? "Master" : string.Empty));
                }
                else
                {
                    Trace.TraceWarning("EXIT: {0} heartbeat failed {1} times.".FormatInvariant(isMasterHeartbeat ? "Master" : string.Empty, this.maxHeartbeatRetries));
                }
            }

            return result;
        }

        private void OnHeartbeatTimerTick(object sender, EventArgs e)
        {
            Trace.WriteLine("ENTER: {0} heart beating ...".FormatInvariant(this.debugName));

            switch (this.taskProcessorState)
            {
                case TaskProcessorState.Inactive:
                case TaskProcessorState.Stopping:
                case TaskProcessorState.Disposed:
                    Trace.WriteLine("EXIT: {0} status is {1}. Heartbeat will be ignored.".FormatInvariant(this.debugName, this.taskProcessorState));
                    return;
            }

            if (!this.TryToHearbeat(false))
            {
                Trace.WriteLine("EXIT: Heartbeat failed for {0}. Application will be terminated.".FormatInvariant(this.debugName));

                this.applicationKiller.Kill();
            }

            Guid? currentMasterId = this.repository.TaskProcessorRuntimeInfo.GetMasterId();

            if (currentMasterId.HasValue)
            {
                if (currentMasterId.Value == this.taskProcessorId)
                {
                    if (!this.TryToHearbeat(true))
                    {
                        Trace.WriteLine("EXIT: Master heartbeat failed for {0}. Application will be terminated.".FormatInvariant(this.debugName));

                        this.applicationKiller.Kill();
                    }
                }
                else if (this.isMaster)
                {
                    Trace.WriteLine("EXIT: {0} is not longer master".FormatInvariant(this.debugName));

                    this.OnBecameSlave(MasterModeChangeReason.Heartbeat);
                }
            }
            else
            {
                Trace.WriteLine("There is no current master so I try to become ...");

                this.isMaster = this.repository.TaskProcessorRuntimeInfo.SetMasterIfNotExists(this.taskProcessorId);

                if (this.isMaster)
                {
                    Trace.WriteLine("I became the master.");

                    this.OnBecameMaster(MasterModeChangeReason.Heartbeat, true);
                }
                else
                {
                    Trace.WriteLine("Someone else has already become the master.");
                }
            }

            Trace.WriteLine("EXIT: {0} heartbeat completed.".FormatInvariant(this.debugName));
        }

        #endregion Heartbeat

        #region Configuration Changed

        private void OnConfigurationChanged(object sender, TaskProcessorEventArgs e)
        {
            Trace.WriteLine("ENTER: Configuration for task processor '{0}' has been changed ...".FormatInvariant(e.TaskProcessorId));

            if (e.TaskProcessorId != this.taskProcessorId)
            {
                Trace.WriteLine("EXIT: Configuration change is not for me.");

                return;
            }

            this.UpdatePollingJobsAndQueues();

            Trace.WriteLine("EXIT: Configuration for task processor '{0}' changed.".FormatInvariant(e.TaskProcessorId));
        }

        #endregion Configuration Changed

        #region Polling Jobs

        private void StartPollingJob(IPollingJobConfiguration configuration)
        {
            Trace.WriteLine("ENTER: Starting {0} polling job '{1}' ...".FormatInvariant(
                configuration.IsMaster ? "master" : string.Empty,
                configuration.ImplementationType.Name));

            IPollingJob pollingJob;

            if (this.locator.CanResolve(configuration.ImplementationType))
            {
                pollingJob = this.locator.ResolveSingle<IPollingJob>(configuration.ImplementationType);
            }
            else
            {
                pollingJob = configuration.ImplementationType.CreateInstance<IPollingJob>();
            }

            pollingJob.Initialize();

            PollingJobRuntimeInfo pollingJobInfo = new PollingJobRuntimeInfo(configuration.ImplementationType, pollingJob)
            {
                IsConcurrent = configuration.IsConcurrent
            };

            ITimer timer = this.locator.ResolveSingle<ITimer>();

            timer.Interval = configuration.PollInterval;

            timer.Tick += this.OnPollingJobTimerTick;

            this.pollingJobsByTimer.TryAdd(timer, pollingJobInfo);

            timer.Start();

            Trace.WriteLine("EXIT: {0} polling job '{1}' started.".FormatInvariant(
                configuration.IsMaster ? "master" : string.Empty,
                configuration.ImplementationType.Name));
        }

        private void StopPollingJob(ITimer timer, Type implementationType)
        {
            Trace.WriteLine("ENTER: Stopping polling job '{0}' ...".FormatInvariant(implementationType.Name));

            timer.Tick -= this.OnPollingJobTimerTick;

            timer.Dispose();

            PollingJobRuntimeInfo pollingJobInfo;

            if (this.pollingJobsByTimer.TryRemove(timer, out pollingJobInfo))
            {
                if (pollingJobInfo.PollingJob is IDisposable)
                {
                    ((IDisposable)pollingJobInfo.PollingJob).Dispose();
                }
            }

            Trace.WriteLine("EXIT: Polling job '{0}' stopped.".FormatInvariant(implementationType.Name));
        }

        private void UpdatePollingJobs(IPollingJobsConfiguration configuration)
        {
            Trace.WriteLine("ENTER: Updating polling jobs ...");

            foreach (IPollingJobConfiguration queueConfig in configuration)
            {
                var pair = this.pollingJobsByTimer.FirstOrDefault(p => p.Value.ImplementationType == queueConfig.ImplementationType);

                if ((pair.Key == default(ITimer)) || (pair.Value == default(PollingJobRuntimeInfo)))
                {
                    if (queueConfig.IsActive && (!queueConfig.IsMaster || this.isMaster))
                    {
                        this.StartPollingJob(queueConfig);
                    }
                }
                else if (!queueConfig.IsActive || (queueConfig.IsMaster && !this.isMaster))
                {
                    this.StopPollingJob(pair.Key, pair.Value.ImplementationType);
                }
                else
                {
                    pair.Value.IsConcurrent = queueConfig.IsConcurrent;
                }
            }

            Trace.WriteLine("EXIT: Polling jobs updated.");
        }

        private void StopPollingJobs()
        {
            Trace.WriteLine("ENTER: Stopping all polling jobs ...");

            foreach (var pair in this.pollingJobsByTimer)
            {
                this.StopPollingJob(pair.Key, pair.Value.ImplementationType);
            }

            this.pollingJobsByTimer.Clear();

            Trace.WriteLine("EXIT: All polling jobs stopped.");
        }

        private void OnPollingJobTimerTick(object sender, EventArgs e)
        {
            Trace.WriteLine("ENTER: Polling job timer tick ...");

            ITimer timer = (ITimer)sender;

            PollingJobRuntimeInfo pollingJobInfo;

            if (!this.pollingJobsByTimer.TryGetValue(timer, out pollingJobInfo))
            {
                timer.Dispose();

                return;
            }

            if (pollingJobInfo.IsProcessing && !pollingJobInfo.IsConcurrent)
            {
                Trace.WriteLine("EXIT: Polling job timer {0} is not concurrent and is currently processing.".FormatInvariant(pollingJobInfo.PollingJob.GetType().Name));

                return;
            }

            pollingJobInfo.IsProcessing = true;

            Helpers.TryToExecute(pollingJobInfo.PollingJob.Process);

            pollingJobInfo.IsProcessing = false;

            Trace.WriteLine("EXIT: Polling job timer {0} tick.".FormatInvariant(pollingJobInfo.PollingJob.GetType().Name));
        }

        private void UpdatePollingJobsAndQueues()
        {
            Trace.WriteLine("ENTER: Updating polling jobs and queues ...");

            ITaskProcessorRuntimeInfo processorInfo = this.repository.TaskProcessorRuntimeInfo.GetById(this.taskProcessorId);

            if (processorInfo == null)
            {
                Trace.WriteLine("EXIT: Task processor '{0}' not found in repository.".FormatInvariant(this.taskProcessorId));

                this.applicationKiller.Kill();

                return;
            }

            this.UpdatePollingQueues(processorInfo.Configuration.PollingQueues);
            this.UpdatePollingJobs(processorInfo.Configuration.PollingJobs);

            Trace.WriteLine("EXIT: Polling jobs and queues updated.");
        }

        #endregion Polling Jobs

        #region Polling Queues

        private void StartPollingQueue(ITaskProcessorPollingQueueConfiguration configuration)
        {
            Trace.WriteLine("ENTER: Starting {0} polling queue '{1}' ...".FormatInvariant(
                configuration.IsMaster ? "master" : string.Empty,
                configuration.Key));

            PollingQueueRuntimeInfo pollingQueueInfo = new PollingQueueRuntimeInfo(configuration.Key)
            {
                IsConcurrent = configuration.IsConcurrent,
                MaxWorkers = configuration.MaxWorkers
            };

            ITimer timer = this.locator.ResolveSingle<ITimer>();

            timer.Interval = configuration.PollInterval;

            timer.Tick += this.OnPollingQueueTimerTick;

            this.pollingQueuesByTimer.TryAdd(timer, pollingQueueInfo);

            timer.Start();

            Trace.WriteLine("EXIT: {0} polling queue {1} started.".FormatInvariant(
                configuration.IsMaster ? "master" : string.Empty,
                configuration.Key));
        }

        private void StopPollingQueue(ITimer timer, string pollingQueueKey)
        {
            Trace.WriteLine("ENTER: Stopping polling queue '{0}' ...".FormatInvariant(pollingQueueKey));

            timer.Tick -= this.OnPollingJobTimerTick;

            timer.Dispose();

            this.pollingQueuesByTimer.TryRemove(timer);

            Trace.WriteLine("EXIT: Polling queue '{0}' stopped.".FormatInvariant(pollingQueueKey));
        }

        private void StopPollingQueues()
        {
            Trace.WriteLine("ENTER: Stopping all polling queues ...");

            foreach (var pair in this.pollingQueuesByTimer)
            {
                this.StopPollingQueue(pair.Key, pair.Value.Key);
            }

            this.pollingQueuesByTimer.Clear();

            Trace.WriteLine("EXIT: All polling queues stopped.");
        }

        private void UpdatePollingQueues(ITaskProcessorPollingQueuesConfiguration configuration)
        {
            Trace.WriteLine("ENTER: Updating polling queues ...");

            foreach (ITaskProcessorPollingQueueConfiguration queueConfig in configuration)
            {
                var pair = this.pollingQueuesByTimer.FirstOrDefault(p => p.Value.Key == queueConfig.Key);

                if ((pair.Key == default(ITimer)) || (pair.Value == default(PollingQueueRuntimeInfo)))
                {
                    if (queueConfig.IsActive && (!queueConfig.IsMaster || this.isMaster))
                    {
                        this.StartPollingQueue(queueConfig);
                    }
                }
                else if (!queueConfig.IsActive || (queueConfig.IsMaster && !this.isMaster))
                {
                    this.StopPollingQueue(pair.Key, pair.Value.Key);
                }
                else
                {
                    pair.Key.Interval = queueConfig.PollInterval;

                    pair.Value.IsConcurrent = queueConfig.IsConcurrent;
                    pair.Value.MaxWorkers = queueConfig.MaxWorkers;
                }
            }

            Trace.WriteLine("EXIT: Polling queues updated.");
        }

        private void OnPollingQueueTimerTick(object sender, EventArgs e)
        {
            Trace.WriteLine("ENTER: Polling queue timer tick ...");

            ITimer timer = (ITimer)sender;

            PollingQueueRuntimeInfo pollingQueueInfo;

            if (!this.pollingQueuesByTimer.TryGetValue(timer, out pollingQueueInfo))
            {
                timer.Dispose();

                return;
            }

            if (pollingQueueInfo.IsProcessing && !pollingQueueInfo.IsConcurrent)
            {
                Trace.WriteLine("EXIT: Polling queue timer '{0}' is not concurrent and is currently processing.".FormatInvariant(pollingQueueInfo.Key));

                return;
            }

            pollingQueueInfo.IsProcessing = true;

            Helpers.TryToExecute(() => this.ProcessPollingQueue(pollingQueueInfo));

            pollingQueueInfo.IsProcessing = false;

            Trace.WriteLine("EXIT: Polling queue timer '{0}' tick.".FormatInvariant(pollingQueueInfo.Key));
        }

        private void ProcessPollingQueue(PollingQueueRuntimeInfo pollingQueueInfo)
        {
            Trace.WriteLine("ENTER: Processing polling queue '{0}' ...".FormatInvariant(pollingQueueInfo.Key));

            int maxResults = pollingQueueInfo.MaxWorkers - pollingQueueInfo.ActiveTasksCount;

            if (maxResults <= 0)
            {
                Trace.WriteLine("EXIT: No free task slots for polling queue '{0}'.".FormatInvariant(pollingQueueInfo.Key));

                return;
            }

            IEnumerable<ITaskRuntimeInfo> pendingTasksInfo = this.repository.TaskRuntimeInfo.ReservePollingQueueTasks(pollingQueueInfo.Key, maxResults);

            DateTime timestampUtc = this.dateTimeProvider.UtcNow;

            foreach (ITaskRuntimeInfo taskInfo in pendingTasksInfo)
            {
                this.taskExecutor.StartTask(taskInfo.TaskId, taskInfo.Priority);

                this.tasksPollingQueues.TryAdd(taskInfo.TaskId, pollingQueueInfo);

                pollingQueueInfo.IncrementActiveTasksCount();

                this.repository.TaskRuntimeInfo.Start(taskInfo.TaskId, this.taskProcessorId, timestampUtc);

                ITaskMessageBusSender taskMessageBus = this.messageBus.Tasks.GetSender(taskInfo.TaskType);

                taskMessageBus.NotifyTaskStarted(taskInfo.TaskId, this.taskProcessorId, timestampUtc);
            }

            Trace.WriteLine("EXIT: Polling queue '{0}' processed.".FormatInvariant(pollingQueueInfo.Key));
        }

        #endregion Polling Queues

        #region Other Methods

        private void Dispose(bool disposing)
        {
            Trace.WriteLine("ENTER: Disposing {0} ...".FormatInvariant(this));

            if (this.taskProcessorState == TaskProcessorState.Disposed)
            {
                Trace.WriteLine("EXIT: {0} is already disposed.");

                return;
            }

            this.StopPollingQueues();
            this.StopPollingJobs();

            if (this.heartbeatTimer != null)
            {
                this.heartbeatTimer.Stop();
            }

            if (this.reportPerformanceTimer != null)
            {
                this.reportPerformanceTimer.Stop();
            }

            if (this.masterCommandsProcessor != null)
            {
                this.masterCommandsProcessor.IsActive = false;
            }

            if (this.taskExecutor != null)
            {
                this.taskExecutor.TaskCanceled -= this.OnTaskProcessCanceled;
                this.taskExecutor.TaskFailed -= this.OnTaskProcessFailed;
                this.taskExecutor.TaskCompleted -= this.OnTaskProcessCompleted;

                this.taskExecutor.MonitorPerformance = false;
            }

            if (this.messageBus != null)
            {
                this.messageBus.TaskProcessors.Receiver.MasterModeChangeRequested -= this.OnMasterModeChangeRequested;
                this.messageBus.TaskProcessors.Receiver.MasterModeChanged -= this.OnMasterModeChanged;
                this.messageBus.TaskProcessors.Receiver.StopRequested -= this.OnTaskProcessorStopRequested;
                this.messageBus.TaskProcessors.Receiver.PerformanceMonitoringRequested -= this.OnPerformanceMonitoringStarted;
                this.messageBus.TaskProcessors.Receiver.ConfigurationChanged -= this.OnConfigurationChanged;

                this.messageBus.Tasks.Receiver.TaskAssigned -= this.OnTaskAssigned;
                this.messageBus.Tasks.Receiver.TaskCancelRequested -= this.OnTaskCanceled;

                this.messageBus.TaskProcessors.Receiver.UnsubscribeFromAllChannels();
                this.messageBus.Tasks.Receiver.UnsubscribeFromAllChannels();
            }

            if (this.ramPerformanceCounter != null)
            {
                this.ramPerformanceCounter.Dispose();

                this.ramPerformanceCounter = null;
            }

            if (this.cpuPerformanceCounter != null)
            {
                this.cpuPerformanceCounter.Dispose();

                this.cpuPerformanceCounter = null;
            }

            this.taskProcessorState = TaskProcessorState.Disposed;

            if (this.messageBus != null)
            {
                Helpers.TryToExecute(() => this.messageBus.TaskProcessors.Sender.NotifyStateChanged(this.taskProcessorId, TaskProcessorState.Disposed));
            }

            Trace.WriteLine("EXIT: {0} disposed.".FormatInvariant(this));
        }

        #endregion Other Methods
    }
}