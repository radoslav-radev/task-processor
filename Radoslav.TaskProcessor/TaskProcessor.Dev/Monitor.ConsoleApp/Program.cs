using System;
using System.Diagnostics;
using Radoslav.ServiceLocator;

namespace Radoslav.TaskProcessor
{
    /// <summary>
    /// An entry point for the application.
    /// </summary>
    internal static class Program
    {
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => Trace.WriteLine(e.ExceptionObject);

            Trace.WriteLine("Starting with arguments: " + args.ToString(", "));

            RadoslavTaskProcessor processor;

            try
            {
                processor = RadoslavServiceLocator.DefaultInstance.ResolveSingle<RadoslavTaskProcessor>();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);

                Trace.Close();

                return;
            }

            Console.Title = "Task Processor {0}".FormatInvariant(processor.Id);

            Helpers.WaitForEvent(
                handler => processor.Stopped += handler,
                processor.Start);

            if (processor != null)
            {
                processor.Dispose();
            }

            Trace.Close();
        }
    }
}