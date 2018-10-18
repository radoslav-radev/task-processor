using System;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Radoslav.TaskProcessor
{
    /// <summary>
    /// An entry point for the application.
    /// </summary>
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Guid taskId;

            Console.WriteLine("Start with arguments: " + args.ToString(", "));

            if (!Guid.TryParse(args[0], out taskId))
            {
                throw new ArgumentException("No task ID specified.");
            }

            TimeSpan duration;

            string durationAsString = args.FirstOrDefault(a => a.StartsWith("/duration:", StringComparison.Ordinal));

            if (durationAsString != null)
            {
                durationAsString = durationAsString.Substring("/duration:".Length);

                duration = TimeSpan.FromSeconds(int.Parse(durationAsString, CultureInfo.InvariantCulture));
            }
            else if (taskId == Guid.Parse("D68A83A4-6D67-4EDF-B2C5-BCC899624BA7"))
            {
                duration = TimeSpan.FromSeconds(4);
            }
            else
            {
                duration = TimeSpan.FromSeconds(1);
            }

            Thread.Sleep(duration);

            if (taskId == Guid.Empty)
            {
                throw new InvalidOperationException();
            }
        }
    }
}