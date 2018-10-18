using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.MessageBus;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class MasterCommandsUnitTests
    {
        #region Task Failed

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TaskFailedConstructorNullError()
        {
            new TaskFailedMasterCommand(Guid.Empty, Guid.Empty, DateTime.UtcNow, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TaskFailedConstructorEmptyError()
        {
            new TaskFailedMasterCommand(Guid.Empty, Guid.Empty, DateTime.UtcNow, string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TaskFailedSetNullError()
        {
            new TaskFailedMasterCommand(Guid.Empty, Guid.Empty, DateTime.UtcNow, "Error").Error = null;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TaskFailedSetEmptyError()
        {
            new TaskFailedMasterCommand(Guid.Empty, Guid.Empty, DateTime.UtcNow, "Error").Error = string.Empty;
        }

        #endregion Task Failed
    }
}