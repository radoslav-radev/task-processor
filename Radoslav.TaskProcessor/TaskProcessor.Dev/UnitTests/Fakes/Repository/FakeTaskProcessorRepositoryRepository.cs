using Radoslav.TaskProcessor.Repository;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed partial class FakeTaskProcessorRepository : ITaskProcessorRepository
    {
        private readonly FakeTaskRuntimeInfoRepository taskInfos = new FakeTaskRuntimeInfoRepository();
        private readonly FakeTaskRepository tasks = new FakeTaskRepository();
        private readonly FakeTaskProcessorRuntimeInfoRepository taskProcessors = new FakeTaskProcessorRuntimeInfoRepository();
        private readonly FakeTaskSummaryRepository taskSummaries = new FakeTaskSummaryRepository();
        private readonly FakeTaskJobSettingsRepository taskJobSettings = new FakeTaskJobSettingsRepository();
        private readonly FakeScheduledTaskRepository scheduledTasks = new FakeScheduledTaskRepository();

        #region ITaskProcessorRepository Members

        ITaskRepository ITaskProcessorRepository.Tasks
        {
            get { return this.tasks; }
        }

        ITaskRuntimeInfoRepository ITaskProcessorRepository.TaskRuntimeInfo
        {
            get { return this.taskInfos; }
        }

        ITaskProcessorRuntimeInfoRepository ITaskProcessorRepository.TaskProcessorRuntimeInfo
        {
            get { return this.taskProcessors; }
        }

        ITaskSummaryRepository ITaskProcessorRepository.TaskSummary
        {
            get { return this.taskSummaries; }
        }

        ITaskJobSettingsRepository ITaskProcessorRepository.TaskJobSettings
        {
            get { return this.taskJobSettings; }
        }

        public IScheduledTaskRepository ScheduledTasks
        {
            get { return this.scheduledTasks; }
        }

        #endregion ITaskProcessorRepository Members

        internal FakeTaskRuntimeInfoRepository TaskRuntimeInfo
        {
            get { return this.taskInfos; }
        }

        internal FakeTaskRepository Tasks
        {
            get { return this.tasks; }
        }

        internal FakeTaskProcessorRuntimeInfoRepository TaskProcessorRuntimeInfo
        {
            get { return this.taskProcessors; }
        }

        internal FakeTaskSummaryRepository TaskSummary
        {
            get { return this.taskSummaries; }
        }

        internal ITaskJobSettingsRepository TaskJobSettings
        {
            get { return this.taskJobSettings; }
        }
    }
}