using System.Globalization;
using System.Windows;

namespace Radoslav.TaskProcessor
{
    /// <summary>
    /// Interaction logic for ScheduleTaskWindow.xaml
    /// </summary>
    public partial class ScheduleTaskWindow : Window
    {
        public ScheduleTaskWindow()
        {
            this.InitializeComponent();
        }

        internal int MinDurationInSeconds
        {
            get
            {
                return int.Parse(this.txtMinDuration.Text, CultureInfo.InvariantCulture);
            }
        }

        internal int MaxDurationInSeconds
        {
            get
            {
                return int.Parse(this.txtMaxDuration.Text, CultureInfo.InvariantCulture);
            }
        }

        internal int DelayInSeconds
        {
            get
            {
                return int.Parse(this.txtDelay.Text, CultureInfo.InvariantCulture);
            }
        }

        internal int Occurences
        {
            get
            {
                return int.Parse(this.txtOccurences.Text, CultureInfo.InvariantCulture);
            }
        }

        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}