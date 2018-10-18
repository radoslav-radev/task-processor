using System;
using System.Linq;
using System.Threading;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.TaskWorker;

namespace Radoslav.TaskProcessor.Sample
{
    public class SampleTaskWorker : ITaskWorker
    {
        private bool isCanceled;

        #region ITaskWorker Members

        public event EventHandler<TaskWorkerProgressEventArgs> ReportProgress;

        public void CancelTask()
        {
            this.isCanceled = true;
        }

        public void StartTask(ITask task, ITaskJobSettings settings)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            SampleTask demoTask = (SampleTask)task;

            int totalDurationInSeconds = demoTask.Details.Sum(d => d.DurationInSeconds);

            int pastDurationInSeconds = 0;

            foreach (SampleTaskDetail duration in demoTask.Details)
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

        #endregion ITaskWorker Members
    }
}