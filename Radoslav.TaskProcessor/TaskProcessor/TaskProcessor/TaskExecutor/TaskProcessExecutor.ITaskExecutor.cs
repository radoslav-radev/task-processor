// Separate file because of StypeCop SA1201 rule.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using Radoslav.TaskProcessor.MessageBus;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor
{
    public partial class TaskProcessExecutor : ITaskExecutor
    {
        #region ITaskExecutor Members

        /// <inheritdoc />
        public event EventHandler<TaskEventArgs> TaskCanceled;

        /// <inheritdoc />
        public event EventHandler<TaskEventArgs> TaskFailed;

        /// <inheritdoc />
        public event EventHandler<TaskCompletedEventArgs> TaskCompleted;

        /// <inheritdoc />
        public int ActiveTasksCount
        {
            get
            {
                return this.activeTasksByTaskId.Count;
            }
        }

        /// <inheritdoc />
        public TimeSpan CancelTimeout
        {
            get
            {
                return this.cancelTimeout;
            }

            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Value must not be negative.");
                }

                this.cancelTimeout = value;
            }
        }

        /// <inheritdoc />
        public bool MonitorPerformance
        {
            get
            {
                return this.monitorPerformance;
            }

            set
            {
                if (this.monitorPerformance == value)
                {
                    return;
                }

                this.monitorPerformance = value;

                if (value)
                {
                    this.StartToMonitorPerformance();
                }
                else
                {
                    this.StopToMonitorPerformance();
                }
            }
        }

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Process instance is needed and will be disposed on process exit.")]
        public void StartTask(Guid taskId, TaskPriority priority)
        {
            Trace.WriteLine("ENTER: Starting task '{0}' process ...".FormatInvariant(taskId));

            switch (this.disposeState)
            {
                case DisposeState.Disposing:
                case DisposeState.Disposed:
                    throw new ObjectDisposedException(this.debugName);
            }

            Process process = new Process();

            process.StartInfo.FileName = Path.Combine(Helpers.GetCurrentExeDirectory(), this.executableFilePath);

            process.StartInfo.Arguments = taskId.ToString();

            if (this.IsUserInteractive)
            {
                process.StartInfo.ErrorDialog = true;
            }
            else
            {
                process.StartInfo.UseShellExecute = false;

                process.StartInfo.CreateNoWindow = true;

                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }

            process.EnableRaisingEvents = true;

            process.Exited += this.OnTaskProcessExited;

            process.Start();

            this.childProcessKiller.AddProcess(process);

            this.activeTasksByTaskId.TryAdd(taskId, process);
            this.activeTasksByProcess.TryAdd(process, taskId);

            if (this.monitorPerformance)
            {
                this.StartToMonitorPerformance(taskId, process);
            }

            Trace.WriteLine("Setting task '{0}' priority to {1} ...".FormatInvariant(taskId, priority));

            try
            {
                switch (priority)
                {
                    case TaskPriority.Low:
                        process.PriorityClass = ProcessPriorityClass.BelowNormal;
                        break;

                    case TaskPriority.Normal:
                        process.PriorityClass = ProcessPriorityClass.Normal;
                        break;

                    case TaskPriority.High:
                        process.PriorityClass = ProcessPriorityClass.AboveNormal;
                        break;

                    case TaskPriority.VeryHigh:
                        process.PriorityClass = ProcessPriorityClass.High;
                        break;

                    default:
                        Trace.TraceWarning("Task '{0}' priority {1} is unknown.".FormatInvariant(taskId, priority));
                        break;
                }
            }
            catch (Exception ex)
            {
                if (ex.IsCritical())
                {
                    throw;
                }

                Trace.TraceWarning("Failed to set task '{0}' priority to {1}.".FormatInvariant(taskId, priority));
            }

            Trace.WriteLine("Task '{0}' priority set to {1}.".FormatInvariant(taskId, priority));

            Trace.WriteLine("EXIT: Task '{0}' process started.".FormatInvariant(taskId));
        }

        /// <inheritdoc />
        public void CancelTask(Guid taskId)
        {
            Trace.WriteLine("ENTER: Cancelling task '{0}' ...".FormatInvariant(taskId));

            switch (this.disposeState)
            {
                case DisposeState.Disposing:
                case DisposeState.Disposed:
                    throw new ObjectDisposedException(this.debugName);
            }

            Process process;

            if (!this.activeTasksByTaskId.TryGetValue(taskId, out process))
            {
                Trace.WriteLine("EXIT: Task '{0}' is not executed by me.".FormatInvariant(taskId));

                return;
            }

            this.canceledTasksById.Add(taskId);

            Thread.Sleep(this.cancelTimeout);

            TaskProcessExecutor.TryToKillProcess(taskId, process);

            Trace.WriteLine("EXIT: Task '{0}' canceled.".FormatInvariant(taskId));
        }

        /// <inheritdoc />
        public IEnumerable<TaskPerformanceReport> GetTasksPerformanceInfo()
        {
            Trace.WriteLine("ENTER: Getting tasks performance info ...");

            if (this.disposeState != DisposeState.None)
            {
                throw new ObjectDisposedException(this.debugName);
            }

            foreach (var pair in this.activeTasksByTaskId)
            {
                PerformanceCounter counter;

                if (!this.cpuPercentPerformaceCounters.TryGetValue(pair.Key, out counter))
                {
                    continue;
                }

                pair.Value.Refresh();

                yield return new TaskPerformanceReport(pair.Key)
                {
                    RamPercent = Convert.ToSingle(Math.Round(100.0 * pair.Value.WorkingSet64 / ComputerInfo.TotalPhysicalMemory, 2)),
                    CpuPercent = Convert.ToSingle(Math.Round(counter.NextValue() / Environment.ProcessorCount, 2))
                };
            }

            Trace.WriteLine("EXIT: Tasks performance info returned.");
        }

        #endregion ITaskExecutor Members
    }
}