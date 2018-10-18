using System.Windows;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor
{
    /// <summary>
    /// Interaction logic for SubmitTaskWindow.xaml
    /// </summary>
    public partial class DemoTaskJobSettingsWindow : Window
    {
        public DemoTaskJobSettingsWindow(DemoTaskJobSettings settings)
        {
            this.InitializeComponent();

            this.DataContext = settings;
        }

        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}