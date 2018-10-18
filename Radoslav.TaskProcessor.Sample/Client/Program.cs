using System;
using System.IO;
using System.Linq;
using Radoslav.ServiceLocator;
using Radoslav.TaskProcessor.Facade;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Sample
{
    internal static class Program
    {
        internal static void Main(string[] args)
        {
            ITaskProcessorFacade facade = RadoslavServiceLocator.DefaultInstance.ResolveSingle<ITaskProcessorFacade>();

            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Choose an option from the menu: ");
            Program.PrintOption("T", "Submit task.");
            Program.PrintOption("PQ", "Submit polling queue task.");
            Program.PrintOption("PL", "Submit task with low priority.");
            Program.PrintOption("PH", "Submit task with high priority.");
            Program.PrintOption("SS", "Submit task with a string summary.");
            Program.PrintOption("DS", "Submit task with a dictionary summary.");
            Program.PrintOption("CS", "Submit task with a custom summary.");
            Program.PrintOption("GDTJS", "Get dictionary task job settings.");
            Program.PrintOption("SDTJS", "Set dictionary task job settings.");
            Program.PrintOption("GCTJS", "Get custom task job settings.");
            Program.PrintOption("SCTJS", "Set custom task job settings.");
            Program.PrintOption("EXIT", "Exit application.");

            while (true)
            {
                string option = Console.ReadLine();

                ITask task;

                switch (option.ToUpperInvariant())
                {
                    case "PQ":
                        task = Program.CreateTask(true);
                        break;

                    case "EXIT":
                        return;

                    default:
                        task = Program.CreateTask(false);
                        break;
                }

                Guid? taskId = null;

                switch (option.ToUpperInvariant())
                {
                    // Submit task.
                    case "T":
                        taskId = facade.SubmitTask(task);
                        break;

                    // Submit polling queue task.
                    case "PQ":
                        taskId = facade.SubmitTask(task);
                        break;

                    // Submit task with low priority.
                    case "PL":
                        taskId = facade.SubmitTask(task, TaskPriority.Low);
                        break;

                    // Submit task with high priority.
                    case "PH":
                        taskId = facade.SubmitTask(task, TaskPriority.High);
                        break;

                    // Submit task with a text summary.
                    case "SS":
                        taskId = facade.SubmitTask(task, new StringTaskSummary("Sample text task summary."));
                        break;

                    // Submit task with a dictionary summary.
                    case "DS":
                        taskId = facade.SubmitTask(task, new DictionaryTaskSummary()
                    {
                        { "Key 1", "Value 1" },
                        { "Key 2", "Value 2" }
                    });
                        break;

                    // Submit task with a custom summary.
                    case "CS":
                        taskId = facade.SubmitTask(task, new SampleTaskSummary()
                        {
                            Description = "Custom task summary",
                            TaskId = 11,
                            TenantId = 1
                        });
                        break;

                    case "GDTJS":
                        {
                            DictionaryTaskJobSettings settings = facade.GetTaskJobSettings<SampleTask, DictionaryTaskJobSettings>();

                            if (settings == null)
                            {
                                Console.WriteLine("'{0}' job settings not found.", typeof(SampleTaskJobSettings).Name);
                            }
                            else
                            {
                                Console.WriteLine("{0}: {1}.", settings.GetType().Name, string.Join(", ",
                                    settings.Select(p => "{0} = {1}".FormatInvariant(p.Key, p.Value))));
                            }
                        }

                        break;

                    case "SDTJS":
                        {
                            DictionaryTaskJobSettings settings = new DictionaryTaskJobSettings()
                            {
                                { "Username", Path.GetFileNameWithoutExtension(Path.GetTempFileName()) },
                                { "Password", Path.GetFileNameWithoutExtension(Path.GetTempFileName()) }
                            };

                            facade.SetTaskJobSettings<SampleTask>(settings);

                            Console.WriteLine("{0} set: {1}.", settings.GetType().Name, string.Join(", ",
                                settings.Select(p => "{0} = {1}".FormatInvariant(p.Key, p.Value))));
                        }

                        break;

                    case "GCTJS":
                        {
                            SampleTaskJobSettings settings = facade.GetTaskJobSettings<SampleTask, SampleTaskJobSettings>();

                            if (settings == null)
                            {
                                Console.WriteLine("'{0}' job settings not found.", typeof(SampleTaskJobSettings).Name);
                            }
                            else
                            {
                                Console.WriteLine("{0}: username '{1}' and password '{2}'.", settings.GetType().Name, settings.Username, settings.Password);
                            }
                        }

                        break;

                    case "SCTJS":
                        {
                            SampleTaskJobSettings settings = new SampleTaskJobSettings()
                            {
                                Username = Path.GetFileNameWithoutExtension(Path.GetTempFileName()),
                                Password = Path.GetFileNameWithoutExtension(Path.GetTempFileName())
                            };

                            facade.SetTaskJobSettings<SampleTask>(settings);

                            Console.WriteLine("{0} set with username '{1}' and password '{2}'.", settings.GetType().Name, settings.Username, settings.Password);
                        }

                        break;

                    default:
                        throw new NotSupportedException<string>(option);
                }

                if (taskId.HasValue)
                {
                    Console.WriteLine("Task submitted with ID '{0}'.", taskId);
                }
            }
        }

        private static ITask CreateTask(bool pollingQueueTask)
        {
            if (pollingQueueTask)
            {
                return new SamplePollingQueueTask()
                {
                    Details = new SampleTaskDetail[]
                    {
                        new SampleTaskDetail()
                        {
                            DurationInSeconds = 1
                        },
                        new SampleTaskDetail()
                        {
                            DurationInSeconds = 2
                        }
                    }
                };
            }
            else
            {
                return new SampleTask()
                {
                    Details = new SampleTaskDetail[]
                    {
                        new SampleTaskDetail()
                        {
                            DurationInSeconds = 1
                        },
                        new SampleTaskDetail()
                        {
                            DurationInSeconds = 2
                        }
                    }
                };
            }
        }

        private static void PrintOption(string command, string description)
        {
            Console.Write("- ");

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.Write(command);

            Console.ForegroundColor = ConsoleColor.White;

            Console.Write(": ");

            Console.WriteLine(description);
        }
    }
}