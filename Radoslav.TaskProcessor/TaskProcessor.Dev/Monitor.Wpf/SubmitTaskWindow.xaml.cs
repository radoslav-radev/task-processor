using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor
{
    /// <summary>
    /// Interaction logic for SubmitTaskWindow.xaml
    /// </summary>
    public partial class SubmitTaskWindow : Window
    {
        public SubmitTaskWindow()
        {
            this.InitializeComponent();
        }

        internal TaskPriority? TaskPriority
        {
            get
            {
                return (TaskPriority?)this.cmbPriority.SelectedItem;
            }
        }

        internal TimeSpan TaskDuration
        {
            get
            {
                return TimeSpan.FromSeconds(int.Parse(this.txtDuration.Text, CultureInfo.InvariantCulture));
            }
        }

        internal bool ThrowError
        {
            get
            {
                return this.chbThrowError.IsChecked.Value;
            }
        }

        internal SummaryType? SummaryType
        {
            get
            {
                return (SummaryType?)this.cmbSummary.SelectedItem;
            }
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext == null)
            {
                cmbPriority.ItemsSource = Enum.GetValues(typeof(TaskPriority))
                    .OfType<TaskPriority>()
                    .OrderByDescending(p => (int)p);

                cmbSummary.ItemsSource = Enum.GetValues(typeof(SummaryType));
            }
        }

        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}