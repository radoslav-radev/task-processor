using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor
{
    /// <summary>
    /// Interaction logic for PollingQueuesConfigControl.xaml
    /// </summary>
    public partial class PollingQueuesConfigControl : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty PollingQueuesConfigProperty = DependencyProperty.Register(
            "PollingQueuesConfig",
            typeof(ITaskProcessorPollingQueuesConfiguration),
            typeof(PollingQueuesConfigControl),
            new FrameworkPropertyMetadata(PollingQueuesConfigControl.OnPollingQueuesConfigChanged));

        public PollingQueuesConfigControl()
        {
            this.InitializeComponent();

            this.LayoutRoot.DataContext = this;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members

        public ITaskProcessorPollingQueuesConfiguration PollingQueuesConfig
        {
            get
            {
                return (ITaskProcessorPollingQueuesConfiguration)this.GetValue(PollingQueuesConfigControl.PollingQueuesConfigProperty);
            }

            set
            {
                this.SetValue(PollingQueuesConfigControl.PollingQueuesConfigProperty, value);
            }
        }

        public IEnumerable<ITaskProcessorPollingQueueConfiguration> PollingQueues
        {
            get
            {
                if (this.PollingQueuesConfig == null)
                {
                    return null;
                }

                return this.PollingQueuesConfig.ToList();
            }
        }

        private static void OnPollingQueuesConfigChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((PollingQueuesConfigControl)sender).RaisePropertyChanged(nameof(PollingQueues));
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