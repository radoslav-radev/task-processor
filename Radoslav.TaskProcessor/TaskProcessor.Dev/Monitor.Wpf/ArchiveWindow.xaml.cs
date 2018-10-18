using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor
{
    /// <summary>
    /// Interaction logic for ArchiveWindow.xaml
    /// </summary>
    public partial class ArchiveWindow : Window
    {
        private readonly MainViewModel viewModel;

        public ArchiveWindow(MainViewModel viewModel)
        {
            this.viewModel = viewModel;

            this.InitializeComponent();

            this.DataContext = this;
        }

        public IEnumerable<TaskViewModel> ArchiveTasks
        {
            get
            {
                return this.viewModel.Repository.TaskRuntimeInfo.GetArchive()
                    .Select(t => new TaskViewModel(t));
            }
        }

        public IEnumerable<TaskViewModel> FailedTasks
        {
            get
            {
                return this.viewModel.Repository.TaskRuntimeInfo.GetFailed()
                    .Select(t => new TaskViewModel(t));
            }
        }

        private void OnTaskSummaryButtonClick(object sender, RoutedEventArgs e)
        {
            TaskViewModel taskViewModel = (TaskViewModel)((FrameworkElement)sender).DataContext;

            ITaskSummary summary = this.viewModel.TaskProcessorFacade.GetTaskSummary(taskViewModel.TaskId);

            taskViewModel.ShowTaskSummary(summary);
        }
    }
}