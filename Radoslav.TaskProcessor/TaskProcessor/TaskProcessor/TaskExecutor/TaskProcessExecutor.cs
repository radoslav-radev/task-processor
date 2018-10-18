using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Radoslav.Collections;
using Radoslav.Diagnostics;
using Radoslav.TaskProcessor.MessageBus;

namespace Radoslav.TaskProcessor
{
    /// <summary>
    /// An implementation of <see cref="ITaskExecutor"/> that executes tasks in separate processes.
    /// </summary>
    public sealed partial class TaskProcessExecutor : IDisposable
    {
        #region Fields

        private readonly string executableFilePath;
        private readonly ConcurrentDictionary<Guid, Process> activeTasksByTaskId = new ConcurrentDictionary<Guid, Process>();
        private readonly ConcurrentDictionary<Process, Guid> activeTasksByProcess = new ConcurrentDictionary<Process, Guid>();
        private readonly ConcurrentHashSet<Guid> canceledTasksById = new ConcurrentHashSet<Guid>();
        private readonly ConcurrentDictionary<Guid, PerformanceCounter> cpuPercentPerformaceCounters = new ConcurrentDictionary<Guid, PerformanceCounter>();
        private readonly string debugName = typeof(TaskProcessExecutor).Name;
        private readonly IChildProcessKiller childProcessKiller;

        private bool monitorPerformance;
        private TimeSpan cancelTimeout;
        private DisposeState disposeState;

        #endregion Fields

        #region Constructor & Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskProcessExecutor"/> class.
        /// </summary>
        /// <param name="executableFilePath">The path to the executable file to be started in separate process when a new task is started.</param>
        /// <param name="childProcessKiller">The child process killer that is responsible for killing task process on task processor exit or crash.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="executableFilePath"/> is null or empty string, or <paramref name="childProcessKiller"/> is null.</exception>
        public TaskProcessExecutor(string executableFilePath, IChildProcessKiller childProcessKiller)
        {
            Trace.WriteLine("ENTER: Constructing {0} ...".FormatInvariant(this.debugName));

            if (string.IsNullOrWhiteSpace(executableFilePath))
            {
                throw new ArgumentNullException("executableFilePath");
            }

            if (childProcessKiller == null)
            {
                throw new ArgumentNullException("childProcessKiller");
            }

            this.executableFilePath = executableFilePath;
            this.childProcessKiller = childProcessKiller;

            Trace.WriteLine("EXIT: {0} constructed.".FormatInvariant(this.debugName));
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TaskProcessExecutor"/> class.
        /// </summary>
        ~TaskProcessExecutor()
        {
            Trace.WriteLine("ENTER: Destructing {0} ...".FormatInvariant(this.debugName));

            this.Dispose();

            Trace.WriteLine("EXIT: {0} destructed.".FormatInvariant(this.debugName));
        }

        #endregion Constructor & Destructor

        /// <summary>
        /// Gets the active task processes by task ID.
        /// </summary>
        /// <value>The active task processes by task ID.</value>
        public IReadOnlyDictionary<Guid, int> ActiveTaskProcesses
        {
            get
            {
                return this.activeTasksByTaskId.ToDictionary(p => p.Key, p => p.Value.Id);
            }
        }

        /// <summary>
        /// Gets the child process killer that is responsible for killing task process on task processor exit or crash.
        /// </summary>
        /// <value>The child process killer that is responsible for killing task process on task processor exit or crash.</value>
        public IChildProcessKiller ChildProcessKiller
        {
            get { return this.childProcessKiller; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the task processor executor is user interactive.
        /// </summary>
        /// <remarks>
        /// <para>If value is <c>true</c>, the task worker processes will be started as console applications (useful for debugging).</para>
        /// <para>Otherwise they will be started as windows processes without UI (normally in production).</para>
        /// <para>The default value is <see cref="Environment"/>.<see cref="Environment.UserInteractive"/>.</para>
        /// </remarks>
        /// <value>Whether the task processor executor is user interactive.</value>
        public bool IsUserInteractive { get; set; } = Environment.UserInteractive;

        /// <inheritdoc />
        public override string ToString()
        {
            return this.debugName;
        }

        #region IDisposable Members

        /// <inheritdoc />
        public void Dispose()
        {
            Trace.WriteLine("ENTER: Disposing {0} ...".FormatInvariant(this.debugName));

            switch (this.disposeState)
            {
                case DisposeState.Disposing:
                case DisposeState.Disposed:
                    Trace.WriteLine("EXIT: {0} state is {1}.".FormatInvariant(this.debugName, this.disposeState));
                    return;
            }

            this.disposeState = DisposeState.Disposing;

            Helpers.TryToExecute(() => this.MonitorPerformance = false);

            foreach (var task in this.activeTasksByTaskId)
            {
                TaskProcessExecutor.TryToKillProcess(task.Key, task.Value);
            }

            this.activeTasksByTaskId.Clear();
            this.activeTasksByProcess.Clear();
            this.canceledTasksById.Clear();

            GC.SuppressFinalize(this);

            this.disposeState = DisposeState.Disposed;

            Trace.WriteLine("EXIT: {0} disposed.".FormatInvariant(this.debugName));
        }

        #endregion IDisposable Members

        private static void TryToKillProcess(Guid taskId, Process process)
        {
            Trace.WriteLine("ENTER: Killing process for task '{0}' ...".FormatInvariant(taskId));

            try
            {
                process.Kill();

                Trace.WriteLine("EXIT: Process for task '{0}' killed.".FormatInvariant(taskId));
            }
            catch (InvalidOperationException)
            {
                Trace.WriteLine("EXIT: Process for task '{0}' already exited.".FormatInvariant(taskId));
            }
            catch (Exception ex)
            {
                if (ex.IsCritical())
                {
                    throw;
                }

                Trace.TraceWarning("EXIT: Failed to kill process for task '{0}'. {1}".FormatInvariant(taskId, ex));
            }
        }

        private void OnTaskProcessExited(object sender, EventArgs e)
        {
            Trace.WriteLine("ENTER: Task process exited.");

            Process process = (Process)sender;

            switch (this.disposeState)
            {
                case DisposeState.Disposing:
                case DisposeState.Disposed:
                    Trace.WriteLine("EXIT: {0} state is {1}.".FormatInvariant(this.debugName, this.disposeState));
                    return;
            }

            Guid taskId;

            if (!this.activeTasksByProcess.TryRemove(process, out taskId))
            {
                Trace.TraceWarning("EXIT: Process not found in active tasks list.");

                return;
            }

            this.activeTasksByTaskId.TryRemove(taskId, out process);

            bool canceled = this.canceledTasksById.Remove(taskId);

            if (this.monitorPerformance)
            {
                this.StopToMonitorPerformance(taskId);
            }

            if (canceled)
            {
                var handler = this.TaskCanceled;

                if (handler != null)
                {
                    handler(this, new TaskEventArgs(taskId));
                }
            }
            else if (process.ExitCode == 0)
            {
                var handler = this.TaskCompleted;

                if (handler != null)
                {
                    handler(this, new TaskCompletedEventArgs(taskId, DateTime.MinValue, process.TotalProcessorTime));
                }
            }
            else
            {
                var handler = this.TaskFailed;

                if (handler != null)
                {
                    handler(this, new TaskEventArgs(taskId));
                }
            }

            process.Dispose();

            Trace.WriteLine("EXIT: Task '{0}' process exited.".FormatInvariant(taskId));
        }

        private void StartToMonitorPerformance()
        {
            Trace.WriteLine("ENTER: Starting performance monitoring ...");

            foreach (var pair in this.activeTasksByTaskId)
            {
                this.StartToMonitorPerformance(pair.Key, pair.Value);
            }

            Trace.WriteLine("EXIT: Performance monitoring started.");
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Instance is needed for performance monitoring and will be disposed when performance monitoring is stopped.")]
        private void StartToMonitorPerformance(Guid taskId, Process process)
        {
            Trace.WriteLine("ENTER: Starting performance monitoring for task {0} ...".FormatInvariant(taskId));

            string processInstanceName = Helpers.GetProcessInstanceName(process.Id);

            if (!string.IsNullOrEmpty(processInstanceName))
            {
                this.cpuPercentPerformaceCounters.TryAdd(taskId, new PerformanceCounter("Process", "% Processor Time", processInstanceName, true));
            }

            Trace.WriteLine("EXIT: Performance monitoring for task {0} started.".FormatInvariant(taskId));
        }

        private void StopToMonitorPerformance()
        {
            Trace.WriteLine("ENTER: Stopping performance monitoring ...");

            this.cpuPercentPerformaceCounters.ForEach(false, c => c.Value.Dispose());

            this.cpuPercentPerformaceCounters.Clear();

            Trace.WriteLine("EXIT: Performance monitoring stopped.");
        }

        private void StopToMonitorPerformance(Guid taskId)
        {
            Trace.WriteLine("ENTER: Stopping performance monitoring for task {0} ...".FormatInvariant(taskId));

            PerformanceCounter counter;

            if (this.cpuPercentPerformaceCounters.TryGetValue(taskId, out counter))
            {
                counter.Dispose();

                this.cpuPercentPerformaceCounters.TryRemove(taskId, out counter);
            }

            Trace.WriteLine("EXIT: Performance monitoring for task {0} stopped.".FormatInvariant(taskId));
        }
    }
}