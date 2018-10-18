using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.TaskWorker;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakeTaskWorkerFactory : MockObject, ITaskWorkerFactory
    {
        #region ITaskWorkerFactory Members

        public ITaskWorker CreateTaskWorker(ITask task)
        {
            this.RecordMethodCall(task);

            return this.GetPredefinedResult<ITaskWorker>(task);
        }

        #endregion ITaskWorkerFactory Members
    }
}