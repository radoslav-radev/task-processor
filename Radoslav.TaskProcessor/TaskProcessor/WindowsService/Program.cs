using System.ServiceProcess;

namespace Radoslav.TaskProcessor.WindowsService
{
    /// <summary>
    /// The entry point of the Task Processor windows service.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            ServiceBase.Run(new TaskProcessorWindowsService());
        }
    }
}