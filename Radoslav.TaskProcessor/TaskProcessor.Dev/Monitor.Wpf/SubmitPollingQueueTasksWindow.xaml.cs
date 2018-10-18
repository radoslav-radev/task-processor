using System;
using System.Globalization;
using System.Windows;

namespace Radoslav.TaskProcessor
{
    /// <summary>
    /// Interaction logic for SubmitTaskWindow.xaml
    /// </summary>
    public partial class SubmitPollingQueueTasksWindow : Window
    {
        public SubmitPollingQueueTasksWindow()
        {
            this.InitializeComponent();
        }

        internal int TasksCount
        {
            get
            {
                return int.Parse(this.txtTasksCount.Text, CultureInfo.InvariantCulture);
            }
        }

        internal TimeSpan TaskDuration
        {
            get
            {
                return TimeSpan.FromSeconds(int.Parse(this.txtDuration.Text, CultureInfo.InvariantCulture));
            }
        }

        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}