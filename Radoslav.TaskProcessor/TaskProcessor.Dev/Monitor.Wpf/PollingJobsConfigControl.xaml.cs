using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor
{
    /// <summary>
    /// Interaction logic for PollingJobsConfigControl.xaml
    /// </summary>
    public partial class PollingJobsConfigControl : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty PollingJobsConfigProperty = DependencyProperty.Register(
            "PollingJobsConfig",
            typeof(IPollingJobsConfiguration),
            typeof(PollingJobsConfigControl),
            new FrameworkPropertyMetadata(PollingJobsConfigControl.OnPollingJobsConfigChanged));

        public PollingJobsConfigControl()
        {
            this.InitializeComponent();

            this.LayoutRoot.DataContext = this;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members

        public IPollingJobsConfiguration PollingJobsConfig
        {
            get
            {
                return (IPollingJobsConfiguration)this.GetValue(PollingJobsConfigControl.PollingJobsConfigProperty);
            }

            set
            {
                this.SetValue(PollingJobsConfigControl.PollingJobsConfigProperty, value);
            }
        }

        public IEnumerable<IPollingJobConfiguration> PollingJobs
        {
            get
            {
                if (this.PollingJobsConfig == null)
                {
                    return null;
                }

                return this.PollingJobsConfig.ToList();
            }
        }

        private static void OnPollingJobsConfigChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((PollingJobsConfigControl)sender).RaisePropertyChanged("PollingJobs");
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