using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using Radoslav.Diagnostics;

namespace Radoslav.TaskProcessor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly string tempLogFilePath = Path.ChangeExtension(Path.GetTempFileName(), ".log");

        internal string TempLogFilePath
        {
            get { return this.tempLogFilePath; }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (!Debugger.IsAttached)
            {
                MessageBox.Show(e.Exception.ToString(), e.Exception.GetType().Name);

                e.Handled = true;
            }
        }
    }
}