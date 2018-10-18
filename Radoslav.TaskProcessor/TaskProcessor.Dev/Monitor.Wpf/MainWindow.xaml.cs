using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Radoslav.ServiceLocator;
using Radoslav.TaskProcessor.Configuration;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private MainViewModel ViewModel
        {
            get
            {
                return (MainViewModel)this.DataContext;
            }
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext == null)
            {
                this.DataContext = RadoslavServiceLocator.DefaultInstance.ResolveSingle<MainViewModel>();

                ITaskProcessorConfigurationProvider configProvider = RadoslavServiceLocator.DefaultInstance.ResolveSingle<ITaskProcessorConfigurationProvider>();

                ITaskProcessorClientConfiguration config = configProvider.GetClientConfiguration();

                btnSubmitDemoTask.Visibility = config.IsSupported<DemoTask>() ? Visibility.Visible : Visibility.Collapsed;
                btnPollingQueueTasks.Visibility = config.IsSupported<DemoPollingQueueTask>() ? Visibility.Visible : Visibility.Collapsed;
                btnScheduleTask.Visibility = btnPollingQueueTasks.Visibility;
            }
        }

        private void OnLogButtonClick(object sender, RoutedEventArgs e)
        {
            Trace.Flush();

            Process.Start(((App)Application.Current).TempLogFilePath);
        }

        private void OnAddTaskProcessorButtonClick(object sender, RoutedEventArgs e)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase),
                    "Radoslav.TaskProcessor.Monitor.ConsoleApp.exe");

                process.StartInfo.ErrorDialog = true;

                process.Start();
            }
        }

        private void OnSubmitDemoTaskButtonClick(object sender, RoutedEventArgs e)
        {
            SubmitTaskWindow dialog = new SubmitTaskWindow();

            if (dialog.ShowDialog() == true)
            {
                DemoTask demoTask = new DemoTask((int)dialog.TaskDuration.TotalSeconds)
                {
                    ThrowError = dialog.ThrowError
                };

                if (dialog.SummaryType.HasValue)
                {
                    ITaskSummary summary = demoTask.CreateTaskSummary(dialog.SummaryType.Value, false);

                    if (dialog.TaskPriority.HasValue)
                    {
                        this.ViewModel.TaskProcessorFacade.SubmitTask(demoTask, summary, dialog.TaskPriority.Value);
                    }
                    else
                    {
                        this.ViewModel.TaskProcessorFacade.SubmitTask(demoTask, summary);
                    }
                }
                else
                {
                    if (dialog.TaskPriority.HasValue)
                    {
                        this.ViewModel.TaskProcessorFacade.SubmitTask(demoTask, dialog.TaskPriority.Value);
                    }
                    else
                    {
                        this.ViewModel.TaskProcessorFacade.SubmitTask(demoTask);
                    }
                }
            }
        }

        private void OnCancelTaskButtonClick(object sender, RoutedEventArgs e)
        {
            TaskViewModel taskToCancel = (TaskViewModel)((FrameworkElement)sender).DataContext;

            taskToCancel.Status = "Cancelling";

            this.ViewModel.TaskProcessorFacade.CancelTask(taskToCancel.TaskId);
        }

        private void OnStopTaskProcessorButtonClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = (FrameworkElement)sender;

            TaskProcessorViewModel processorViewModel = (TaskProcessorViewModel)button.DataContext;

            this.ViewModel.TaskProcessorFacade.RequestTaskProcessorToStop(processorViewModel.TaskProcessorId);

            button.IsEnabled = false;
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (this.ViewModel.TaskProcessors.Any(p => p.State == TaskProcessorState.Active))
            {
                MessageBoxResult result = MessageBox.Show("There are active task processors. Do you want to stop them?", "Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        e.Cancel = true;

                        this.ViewModel.MessageBus.TaskProcessors.Receiver.StateChanged += (sender1, e1) =>
                        {
                            if (e1.TaskProcessorState == TaskProcessorState.Inactive)
                            {
                                this.Dispatcher.InvokeAsync(() =>
                                {
                                    if (this.ViewModel.TaskProcessors.All(p => p.State == TaskProcessorState.Inactive))
                                    {
                                        this.Close();
                                    }
                                });
                            }
                        };

                        this.ViewModel.TaskProcessors
                            .Where(p => p.State == TaskProcessorState.Active)
                            .ForEach(false, p => this.ViewModel.MessageBus.TaskProcessors.Sender.NotifyStopRequested(p.TaskProcessorId));
                        break;

                    case MessageBoxResult.No:
                        break;

                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void OnTaskProcessorConfigurationClick(object sender, RoutedEventArgs e)
        {
            TaskProcessorViewModel processorViewModel = (TaskProcessorViewModel)((FrameworkElement)sender).DataContext;

            ITaskProcessorRuntimeInfo processorInfo = this.ViewModel.TaskProcessorFacade.GetTaskProcessorRuntimeInfo(processorViewModel.TaskProcessorId);

            ConfigurationWindow dialog = new ConfigurationWindow(processorInfo.Configuration)
            {
                Title = "Task Processor Configuration",
                SaveButtonText = "Save"
            };

            if (dialog.ShowDialog() == true)
            {
                this.ViewModel.TaskProcessorFacade.UpdateTaskProcessorRuntimeInfo(processorInfo);
            }
        }

        private void OnStartMonitoringButtonClick(object sender, RoutedEventArgs e)
        {
            this.ViewModel.StartPerformanceMonitoring();
        }

        private void OnStopMonitoringButtonClick(object sender, RoutedEventArgs e)
        {
            this.ViewModel.StopPerformanceMonitoring();
        }

        private void OnChangeMasterModeClick(object sender, RoutedEventArgs e)
        {
            TaskProcessorViewModel taskProcessorViewModel = (TaskProcessorViewModel)((FrameworkElement)sender).DataContext;

            if (!taskProcessorViewModel.IsMaster)
            {
                this.ViewModel.TaskProcessorFacade.MakeTaskProcessorMaster(taskProcessorViewModel.TaskProcessorId);
            }
        }

        private void OnTasksArchiveButtonClick(object sender, RoutedEventArgs e)
        {
            ArchiveWindow window = new ArchiveWindow(this.ViewModel);

            window.Show();
        }

        private void OnSubmitPollingQueueTasksButtonClick(object sender, RoutedEventArgs e)
        {
            SubmitPollingQueueTasksWindow dialog = new SubmitPollingQueueTasksWindow();

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            DemoPollingQueueTask demoTask = new DemoPollingQueueTask((int)dialog.TaskDuration.TotalSeconds);

            for (int i = 0; i < dialog.TasksCount; i++)
            {
                this.ViewModel.TaskProcessorFacade.SubmitTask(demoTask);
            }
        }

        private void OnShowTaskErrorClick(object sender, RoutedEventArgs e)
        {
            TaskViewModel taskViewModel = (TaskViewModel)((FrameworkElement)sender).DataContext;

            ITaskRuntimeInfo taskInfo = this.ViewModel.TaskProcessorFacade.GetTaskRuntimeInfo(taskViewModel.TaskId);

            MessageBox.Show(taskInfo.Error, "Task Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void OnShowTaskSummaryClick(object sender, RoutedEventArgs e)
        {
            TaskViewModel taskViewModel = (TaskViewModel)((FrameworkElement)sender).DataContext;

            ITaskSummary summary = this.ViewModel.TaskProcessorFacade.GetTaskSummary(taskViewModel.TaskId);

            taskViewModel.ShowTaskSummary(summary);
        }

        private void OnDemoTaskJobSettingsClick(object sender, RoutedEventArgs e)
        {
            DemoTaskJobSettings settings = this.ViewModel.TaskProcessorFacade.GetTaskJobSettings<DemoTask, DemoTaskJobSettings>();

            if (settings == null)
            {
                settings = new DemoTaskJobSettings();
            }

            DemoTaskJobSettingsWindow dialog = new DemoTaskJobSettingsWindow(settings);

            if (dialog.ShowDialog() == true)
            {
                this.ViewModel.TaskProcessorFacade.SetTaskJobSettings<DemoTask>(settings);
            }
        }

        private void OnScheduleTaskButtonClick(object sender, RoutedEventArgs e)
        {
            ScheduleTaskWindow dialog = new ScheduleTaskWindow();

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            DemoScheduledTask scheduledTask = new DemoScheduledTask()
            {
                Id = Guid.NewGuid(),
                MinDurationInSeconds = dialog.MinDurationInSeconds,
                MaxDurationInSeconds = dialog.MaxDurationInSeconds,
                RecurrenceDefinition = new DemoRecurrenceDefinition()
                {
                    DelayInSeconds = dialog.DelayInSeconds,
                    Occurrences = dialog.Occurences
                }
            };

            this.ViewModel.TaskProcessorFacade.AddScheduledTask(scheduledTask);
        }

        private void OnMonitoringWpfLogsClick(object sender, RoutedEventArgs e)
        {
            string path = Path.Combine(Helpers.GetCurrentExeDirectory(), "Logs", "Monitor.Wpf");

            Process.Start(path);
        }

        private void OnTaskProcessorLogsClick(object sender, RoutedEventArgs e)
        {
            string path = Path.Combine(Helpers.GetCurrentExeDirectory(), "Logs", "Task Processor");

            Process.Start(path);
        }

        private void OnTasksLogsClick(object sender, RoutedEventArgs e)
        {
            string path = Path.Combine(Helpers.GetCurrentExeDirectory(), "Logs", "Tasks");

            Process.Start(path);
        }
    }
}