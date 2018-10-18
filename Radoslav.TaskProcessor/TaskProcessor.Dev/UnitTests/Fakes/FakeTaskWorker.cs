using System;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.TaskWorker;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakeTaskWorker : MockObject, ITaskWorker, IDisposable
    {
        #region ITaskWorker Members

        public event EventHandler<TaskWorkerProgressEventArgs> ReportProgress;

        public void StartTask(ITask task, ITaskJobSettings settings)
        {
            this.RecordMethodCall(task, settings);

            this.ExecutePredefinedMethod(task, settings);
        }

        public void CancelTask()
        {
            this.RecordMethodCall();
        }

        #endregion ITaskWorker Members

        #region IDisposable Members

        public void Dispose()
        {
            this.RecordMethodCall();
        }

        #endregion IDisposable Members

        internal void RaiseReportProgress(double percentage)
        {
            if (this.ReportProgress != null)
            {
                this.ReportProgress(this, new TaskWorkerProgressEventArgs(percentage));
            }
        }
    }
}