using System;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakeMasterCommandsProcessor : MockObject, IMasterCommandsProcessor
    {
        #region IMasterCommandsProcessor Members

        public bool IsActive { get; set; }

        public TimeSpan AssignTaskTimeout { get; set; }

        public void ProcessMasterCommands()
        {
            this.RecordMethodCall();
        }

        public void CancelTask(Guid taskId)
        {
            this.RecordMethodCall(taskId);
        }

        #endregion IMasterCommandsProcessor Members
    }
}