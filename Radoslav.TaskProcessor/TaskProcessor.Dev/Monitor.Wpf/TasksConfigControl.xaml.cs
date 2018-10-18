using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor
{
    /// <summary>
    /// Interaction logic for TasksConfigControl.xaml
    /// </summary>
    public partial class TasksConfigControl : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TaskJobsConfigProperty = DependencyProperty.Register(
            "TaskJobsConfig",
            typeof(ITaskJobsConfiguration),
            typeof(TasksConfigControl),
            new FrameworkPropertyMetadata(TasksConfigControl.OnTaskJobsConfigChanged));

        public TasksConfigControl()
        {
            this.InitializeComponent();

            this.LayoutRoot.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ITaskJobsConfiguration TaskJobsConfig
        {
            get
            {
                return (ITaskJobsConfiguration)this.GetValue(TasksConfigControl.TaskJobsConfigProperty);
            }

            set
            {
                this.SetValue(TasksConfigControl.TaskJobsConfigProperty, value);
            }
        }

        public int? MaxTasks
        {
            get
            {
                if (this.TaskJobsConfig == null)
                {
                    return null;
                }

                return this.TaskJobsConfig.MaxWorkers;
            }

            set
            {
                this.TaskJobsConfig.MaxWorkers = value;
            }
        }

        public IEnumerable<ITaskJobConfiguration> Tasks
        {
            get
            {
                if (this.TaskJobsConfig == null)
                {
                    return null;
                }

                return this.TaskJobsConfig.ToList();
            }
        }

        private static void OnTaskJobsConfigChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((TasksConfigControl)sender).RaisePropertyChanged("Tasks");
            ((TasksConfigControl)sender).RaisePropertyChanged("MaxTasks");
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}