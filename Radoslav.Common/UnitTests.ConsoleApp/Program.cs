using System;
using System.Diagnostics;
using System.IO;
using Radoslav.Diagnostics;

namespace Radoslav.Common.UnitTests.ConsoleApp
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            File.Delete("Temp.txt");

            Trace.Listeners.Add(new ConsoleTraceListener());

            Trace.Listeners.Add(new TextWriterTraceListener("Temp.txt"));

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
                {
                    Trace.TraceError(e.ExceptionObject.ToString());

                    Trace.Flush();

                    Process.Start("Temp.txt");
                };

            Trace.WriteLine("Starting with arguments: {0}".FormatInvariant(args.ToString(" ", v => "\"{0}\"".FormatInvariant(v))));

            switch (args[0])
            {
                case "ChildProcessKiller":
                    {
                        Type childProcessKillerType = Type.GetType(args[1], true);

                        IChildProcessKiller killer = (IChildProcessKiller)Activator.CreateInstance(childProcessKillerType);

                        int childProcessCount;

                        if (args.Length > 3)
                        {
                            childProcessCount = int.Parse(args[3]);
                        }
                        else
                        {
                            childProcessCount = 1;
                        }

                        for (int i = 0; i < childProcessCount; i++)
                        {
                            killer.AddProcess(Process.Start(args[2]));
                        }

                        Console.ReadLine();
                    }

                    break;
            }

            Trace.Flush();
        }
    }
}