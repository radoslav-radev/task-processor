using System;
using System.Linq;
using System.Threading;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.TaskWorker
{
    /// <summary>
    /// An implementation of <see cref="ITaskWorker"/> for <see cref="IDemoTask"/> tasks.
    /// </summary>
    public sealed class DemoTaskWorker : ITaskWorker
    {
        private bool isCanceled;

        #region ITaskWorker Members

        /// <inheritdoc />
        public event EventHandler<TaskWorkerProgressEventArgs> ReportProgress;

        /// <inheritdoc />
        public void StartTask(ITask task, ITaskJobSettings settings)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            DemoTask demoTask = (DemoTask)task;

            if (demoTask.ThrowError)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));

                throw new DemoException("Task configured to throw error.");
            }

            int totalDurationInSeconds = demoTask.Durations.Sum(d => d.DurationInSeconds);

            int pastDurationInSeconds = 0;

            foreach (DemoTaskDuration duration in demoTask.Durations)
            {
                if (this.isCanceled)
                {
                    break;
                }

                Thread.Sleep(TimeSpan.FromSeconds(duration.DurationInSeconds));

                pastDurationInSeconds += duration.DurationInSeconds;

                var reportProgressEventHandler = this.ReportProgress;

                if (reportProgressEventHandler != null)
                {
                    double percent = 100.0 * pastDurationInSeconds / totalDurationInSeconds;

                    reportProgressEventHandler(this, new TaskWorkerProgressEventArgs(percent));
                }
            }
        }

        /// <inheritdoc />
        public void CancelTask()
        {
            this.isCanceled = true;
        }

        #endregion ITaskWorker Members
    }
}