using System;
using System.Collections.Generic;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakePollingJob : MockObject, IPollingJob, IDisposable
    {
        [ThreadStatic]
        private static List<FakePollingJob> createdFakePollingJobsInThread;

        public FakePollingJob()
        {
            FakePollingJob.InitializeFakePollingJobsInThread();

            FakePollingJob.createdFakePollingJobsInThread.Add(this);
        }

        ~FakePollingJob()
        {
            this.Dispose(false);
        }

        internal static IEnumerable<FakePollingJob> CreatedFakePollingJobsInThread
        {
            get
            {
                return FakePollingJob.createdFakePollingJobsInThread;
            }
        }

        #region IPollingJob Members

        public void Initialize()
        {
            this.RecordMethodCall();
        }

        public void Process()
        {
            this.RecordMethodCall();

            this.ExecutePredefinedMethod();
        }

        #endregion IPollingJob Members

        #region IDisposable Members

        public void Dispose()
        {
            this.RecordMethodCall();

            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members

        internal static void InitializeFakePollingJobsInThread()
        {
            if (FakePollingJob.createdFakePollingJobsInThread == null)
            {
                FakePollingJob.createdFakePollingJobsInThread = new List<FakePollingJob>();
            }
        }

        internal static void ClearFakePollingJobsInThread()
        {
            if (FakePollingJob.createdFakePollingJobsInThread != null)
            {
                FakePollingJob.createdFakePollingJobsInThread.Clear();
            }
        }

        private void Dispose(bool disposing)
        {
        }
    }
}