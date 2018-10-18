using System.Diagnostics;
using System.ServiceProcess;
using Radoslav.ServiceLocator;

namespace Radoslav.TaskProcessor.WindowsService
{
    /// <summary>
    /// The windows service hosting the task processor.
    /// </summary>
    public partial class TaskProcessorWindowsService : ServiceBase
    {
        private RadoslavTaskProcessor processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskProcessorWindowsService"/> class.
        /// </summary>
        public TaskProcessorWindowsService()
        {
            this.InitializeComponent();
        }

        /// <inheritdoc />
        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            this.processor = RadoslavServiceLocator.DefaultInstance.ResolveSingle<RadoslavTaskProcessor>();

            this.processor.Stopped += (sender, e) => this.Stop();

            this.processor.Start();
        }

        /// <inheritdoc />
        protected override void OnStop()
        {
            base.OnStop();

            this.processor.Dispose();

            Trace.Flush();
        }

        /// <inheritdoc />
        protected override void OnShutdown()
        {
            base.OnShutdown();

            Trace.Flush();
        }
    }
}