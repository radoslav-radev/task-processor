using System;
using System.Diagnostics;
using Radoslav.ServiceLocator;
using Radoslav.TaskProcessor.MessageBus;

namespace Radoslav.TaskProcessor.TaskWorker
{
    /// <summary>
    /// The entry point for the Task Worker application.
    /// </summary>
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Trace.WriteLine("ENTER: Starting task with arguments: " + args.ToString(","));

            if (args.Length == 0)
            {
                throw new ArgumentException("Task ID is not specified.", "args");
            }

            Guid taskId = Guid.Parse(args[0]);

            if (Environment.UserInteractive)
            {
                Console.Title = "Task {0}".FormatInvariant(taskId);
            }

            Exception error = null;

            try
            {
                TaskWorkerBootstrap taskWorker = RadoslavServiceLocator.DefaultInstance.ResolveSingle<TaskWorkerBootstrap>();

                taskWorker.StartTask(taskId);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);

                error = ex;
            }

            if (error != null)
            {
                throw error;
            }
        }
    }
}