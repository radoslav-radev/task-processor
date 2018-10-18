using System;
using System.Collections.ObjectModel;
using System.Linq;
using Radoslav.TaskProcessor.Configuration;
using Radoslav.TaskProcessor.Facade;
using Radoslav.TaskProcessor.MessageBus;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.Repository;
using Radoslav.Timers;

namespace Radoslav.TaskProcessor
{
    public sealed class MainViewModel
    {
        internal const string MonitoringToolKey = "Monitor.Wpf";

        #region Fields

        private readonly ITaskProcessorRepository repository;
        private readonly ITaskProcessorMessageBus messageBus;
        private readonly ITaskProcessorFacade taskProcessorFacade;
        private readonly ITimer performanceMonitoringTimer;

        private readonly ObservableCollection<TaskViewModel> pendingTasks = new ObservableCollection<TaskViewModel>();
        private readonly ObservableCollection<TaskProcessorViewModel> taskProcessors = new ObservableCollection<TaskProcessorViewModel>();

        #endregion Fields

        #region Constructor

        public MainViewModel(
            ITaskProcessorRepository repository,
            ITaskProcessorMessageBus messageBus,
            ITaskProcessorFacade taskProcessorFacade,
            ITimer performanceMonitoringTimer)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            if (messageBus == null)
            {
                throw new ArgumentNullException("messageBus");
            }

            if (taskProcessorFacade == null)
            {
                throw new ArgumentNullException("taskProcessorFacade");
            }

            if (performanceMonitoringTimer == null)
            {
                throw new ArgumentNullException("performanceMonitoringTimer");
            }

            this.repository = repository;
            this.messageBus = messageBus;
            this.taskProcessorFacade = taskProcessorFacade;
            this.performanceMonitoringTimer = performanceMonitoringTimer;

            this.messageBus.TaskProcessors.Receiver.MasterModeChanged += this.OnMasterModeChanged;
            this.messageBus.TaskProcessors.Receiver.StateChanged += this.OnTaskProcessorStateChanged;
            this.messageBus.TaskProcessors.Receiver.PerformanceReportReceived += this.OnPerformanceReport;

            this.messageBus.Tasks.Receiver.TaskSubmitted += this.OnTaskSubmitted;
            this.messageBus.Tasks.Receiver.TaskAssigned += this.OnTaskAssignedToProcessor;
            this.messageBus.Tasks.Receiver.TaskStarted += this.OnTaskStarted;
            this.messageBus.Tasks.Receiver.TaskProgress += this.OnTaskProgress;
            this.messageBus.Tasks.Receiver.TaskCancelRequested += this.OnTaskCancelRequested;
            this.messageBus.Tasks.Receiver.TaskCancelCompleted += this.OnTaskCancelCompleted;
            this.messageBus.Tasks.Receiver.TaskFailed += this.OnTaskFailed;
            this.messageBus.Tasks.Receiver.TaskCompleted += this.OnTaskCompleted;

            this.performanceMonitoringTimer.Interval = TimeSpan.FromSeconds(5);

            this.performanceMonitoringTimer.Tick += this.OnPerformanceMonitoringTimerTick;

            this.InitializeTasks();

            this.messageBus.TaskProcessors.Receiver.SubscribeForChannels(
                MessageBusChannel.TaskProcessorState,
                MessageBusChannel.MasterModeChanged,
                MessageBusChannel.PerformanceReport);

            this.messageBus.Tasks.Receiver.SubscribeForChannels(
                MessageBusChannel.TaskSubmitted,
                MessageBusChannel.TaskAssigned,
                MessageBusChannel.TaskStarted,
                MessageBusChannel.TaskProgress,
                MessageBusChannel.TaskCancelRequest,
                MessageBusChannel.TaskCancelCompleted,
                MessageBusChannel.TaskFailed,
                MessageBusChannel.TaskCompleted);
        }

        #endregion Constructor

        #region Properties

        public ObservableCollection<TaskProcessorViewModel> TaskProcessors
        {
            get { return this.taskProcessors; }
        }

        public ObservableCollection<TaskViewModel> PendingTasks
        {
            get { return this.pendingTasks; }
        }

        internal ITaskProcessorMessageBus MessageBus
        {
            get { return this.messageBus; }
        }

        internal ITaskProcessorFacade TaskProcessorFacade
        {
            get { return this.taskProcessorFacade; }
        }

        internal ITaskProcessorRepository Repository
        {
            get { return this.repository; }
        }

        #endregion Properties

        #region Internal API

        internal void StartPerformanceMonitoring()
        {
            this.messageBus.TaskProcessors.Sender.NotifyPerformanceMonitoring(TimeSpan.FromSeconds(1));

            this.performanceMonitoringTimer.Start();
        }

        internal void StopPerformanceMonitoring()
        {
            this.performanceMonitoringTimer.Stop();
        }

        #endregion Internal API

        #region Private Methods

        private void InitializeTasks()
        {
            Guid? masterTaskProcessorId = this.taskProcessorFacade.GetMasterTaskProcessorId();

            foreach (ITaskProcessorRuntimeInfo processorInfo in this.taskProcessorFacade.GetTaskProcessorRuntimeInfo())
            {
                TaskProcessorViewModel processorViewModel = new TaskProcessorViewModel(processorInfo.TaskProcessorId, processorInfo.MachineName)
                {
                    State = TaskProcessorState.Active
                };

                if (masterTaskProcessorId.HasValue)
                {
                    processorViewModel.IsMaster = processorViewModel.TaskProcessorId == masterTaskProcessorId.Value;
                }

                this.taskProcessors.Add(processorViewModel);
            }

            foreach (ITaskRuntimeInfo taskInfo in this.repository.TaskRuntimeInfo.GetPending(true))
            {
                this.pendingTasks.Add(new TaskViewModel(taskInfo));
            }

            foreach (ITaskRuntimeInfo taskInfo in this.repository.TaskRuntimeInfo.GetActive())
            {
                TaskProcessorViewModel processor = this.taskProcessors.FirstOrDefault(p => p.TaskProcessorId == taskInfo.TaskProcessorId);

                if (processor != null)
                {
                    processor.ActiveTasks.Add(new TaskViewModel(taskInfo));
                }
            }
        }

        #endregion Private Methods

        #region Message Bus Events

        private void OnTaskProcessorStateChanged(object sender, TaskProcessorStateEventArgs e)
        {
            switch (e.TaskProcessorState)
            {
                case TaskProcessorState.Active:
                    {
                        ITaskProcessorRuntimeInfo processorInfo = this.taskProcessorFacade.GetTaskProcessorRuntimeInfo(e.TaskProcessorId);

                        if (processorInfo == null)
                        {
                            return;
                        }

                        TaskProcessorViewModel processorViewModel = new TaskProcessorViewModel(processorInfo.TaskProcessorId, processorInfo.MachineName)
                        {
                            State = TaskProcessorState.Active
                        };

                        Guid? masterId = this.taskProcessorFacade.GetMasterTaskProcessorId();

                        if (masterId.HasValue && masterId.Value == e.TaskProcessorId)
                        {
                            processorViewModel.IsMaster = true;
                        }

                        App.Current.Dispatcher.InvokeAsync(() =>
                        {
                            this.taskProcessors.Add(processorViewModel);
                        });
                    }

                    break;

                case TaskProcessorState.Stopping:
                case TaskProcessorState.Inactive:
                    {
                        TaskProcessorViewModel processorViewModel = this.taskProcessors.FirstOrDefault(p => p.TaskProcessorId == e.TaskProcessorId);

                        if (processorViewModel == null)
                        {
                            return;
                        }

                        App.Current.Dispatcher.InvokeAsync(() =>
                        {
                            processorViewModel.State = e.TaskProcessorState;
                        });
                    }

                    break;
            }
        }

        private void OnMasterModeChanged(object sender, MasterModeChangeEventArgs e)
        {
            TaskProcessorViewModel taskProcessorViewModel = this.taskProcessors.FirstOrDefault(p => p.TaskProcessorId == e.TaskProcessorId);

            if (taskProcessorViewModel != null)
            {
                App.Current.Dispatcher.InvokeAsync(() =>
                {
                    taskProcessorViewModel.IsMaster = e.IsMaster;
                });
            }
        }

        private void OnTaskSubmitted(object sender, TaskEventArgs e)
        {
            ITaskRuntimeInfo taskInfo = this.taskProcessorFacade.GetTaskRuntimeInfo(e.TaskId);

            if (taskInfo != null)
            {
                App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        this.pendingTasks.Add(new TaskViewModel(taskInfo));
                    });
            }
        }

        private void OnTaskAssignedToProcessor(object sender, TaskAssignedEventArgs e)
        {
            TaskViewModel taskViewModel = this.pendingTasks.FirstOrDefault(t => t.TaskId == e.TaskId);

            if (taskViewModel != null)
            {
                App.Current.Dispatcher.InvokeAsync(() =>
                {
                    taskViewModel.Status = "Assigned";
                    taskViewModel.TaskProcessorId = e.TaskProcessorId;
                });
            }
        }

        private void OnTaskStarted(object sender, TaskStartedEventArgs e)
        {
            TaskViewModel taskViewModel = this.pendingTasks.FirstOrDefault(t => t.TaskId == e.TaskId);

            if (taskViewModel == null)
            {
                return;
            }

            TaskProcessorViewModel processorViewModel = this.taskProcessors.FirstOrDefault(p => p.TaskProcessorId == e.TaskProcessorId);

            if (processorViewModel == null)
            {
                return;
            }

            App.Current.Dispatcher.InvokeAsync(() =>
            {
                taskViewModel.Status = TaskStatus.InProgress.ToString();
                taskViewModel.TaskProcessorId = e.TaskProcessorId;
                taskViewModel.StartedUtc = e.TimestampUtc;

                this.pendingTasks.Remove(taskViewModel);

                processorViewModel.ActiveTasks.Add(taskViewModel);
            });
        }

        private void OnTaskProgress(object sender, TaskProgressEventArgs e)
        {
            TaskViewModel taskViewModel = this.taskProcessors
                .SelectMany(p => p.ActiveTasks)
                .FirstOrDefault(t => t.TaskId == e.TaskId);

            if (taskViewModel != null)
            {
                App.Current.Dispatcher.InvokeAsync(() => taskViewModel.Percentage = e.Percentage);
            }
        }

        private void OnTaskCancelRequested(object sender, TaskEventEventArgs e)
        {
            TaskViewModel taskViewModel = this.taskProcessors
               .SelectMany(p => p.ActiveTasks)
               .Concat(this.pendingTasks)
               .FirstOrDefault(t => t.TaskId == e.TaskId);

            if (taskViewModel == null)
            {
                return;
            }

            App.Current.Dispatcher.InvokeAsync(() =>
                {
                    taskViewModel.Status = TaskStatus.Canceled.ToString();
                    taskViewModel.CanceledUtc = e.TimestampUtc;
                });
        }

        private void OnTaskCancelCompleted(object sender, TaskEventEventArgs e)
        {
            TaskViewModel taskViewModel = this.taskProcessors
                .SelectMany(p => p.ActiveTasks)
                .Concat(this.pendingTasks)
                .FirstOrDefault(t => t.TaskId == e.TaskId);

            if (taskViewModel == null)
            {
                return;
            }

            App.Current.Dispatcher.InvokeAsync(() =>
            {
                taskViewModel.Status = TaskStatus.Canceled.ToString();
                taskViewModel.CompletedUtc = e.TimestampUtc;
            });
        }

        private void OnTaskFailed(object sender, TaskEventEventArgs e)
        {
            TaskViewModel taskViewModel = this.taskProcessors
               .SelectMany(p => p.ActiveTasks)
               .Concat(this.pendingTasks)
               .FirstOrDefault(t => t.TaskId == e.TaskId);

            ITaskRuntimeInfo taskInfo = this.repository.TaskRuntimeInfo.GetById(e.TaskId);

            if ((taskInfo == null) || (taskViewModel == null))
            {
                return;
            }

            App.Current.Dispatcher.InvokeAsync(() =>
            {
                taskViewModel.Status = TaskStatus.Failed.ToString();
                taskViewModel.CompletedUtc = e.TimestampUtc;
                taskViewModel.Error = taskInfo.Error;
            });
        }

        private void OnTaskCompleted(object sender, TaskCompletedEventArgs e)
        {
            TaskViewModel taskViewModel = this.taskProcessors
               .SelectMany(p => p.ActiveTasks)
               .Concat(this.pendingTasks)
               .FirstOrDefault(t => t.TaskId == e.TaskId);

            if (taskViewModel == null)
            {
                return;
            }

            App.Current.Dispatcher.InvokeAsync(() =>
            {
                taskViewModel.Percentage = 100;
                taskViewModel.Status = TaskStatus.Success.ToString();
                taskViewModel.CompletedUtc = e.TimestampUtc;
                taskViewModel.CpuTime = e.TotalCpuTime;
            });
        }

        private void OnPerformanceMonitoringTimerTick(object sender, EventArgs e)
        {
            this.messageBus.TaskProcessors.Sender.NotifyPerformanceMonitoring(TimeSpan.FromSeconds(1));
        }

        private void OnPerformanceReport(object sender, TaskProcessorPerformanceEventArgs e)
        {
            TaskProcessorViewModel processor = this.taskProcessors.FirstOrDefault(p => p.TaskProcessorId == e.PerformanceInfo.TaskProcessorId);

            if (processor == null)
            {
                return;
            }

            processor.CpuPercent = e.PerformanceInfo.CpuPercent;
            processor.RamPercent = e.PerformanceInfo.RamPercent;

            foreach (TaskPerformanceReport taskPerformance in e.PerformanceInfo.TasksPerformance)
            {
                TaskViewModel task = processor.ActiveTasks.FirstOrDefault(t => t.TaskId == taskPerformance.TaskId);

                if (task == null)
                {
                    continue;
                }

                task.CpuPercent = taskPerformance.CpuPercent;
                task.RamPercent = taskPerformance.RamPercent;
            }
        }

        #endregion Message Bus Events
    }
}