using System.Windows;
using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        private readonly ITaskProcessorConfiguration configuration;

        public ConfigurationWindow(ITaskProcessorConfiguration configuration)
        {
            this.InitializeComponent();

            this.configuration = configuration;

            this.DataContext = this;
        }

        public string SaveButtonText
        {
            get
            {
                return (string)this.btnSave.Content;
            }

            set
            {
                this.btnSave.Content = value;
            }
        }

        public ITaskJobsConfiguration Tasks
        {
            get { return this.configuration.Tasks; }
        }

        public IPollingJobsConfiguration PollingJobs
        {
            get { return this.configuration.PollingJobs; }
        }

        public ITaskProcessorPollingQueuesConfiguration PollingQueues
        {
            get { return this.configuration.PollingQueues; }
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}