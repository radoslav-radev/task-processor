using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.MessageBus;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class MessageBusEventArgsTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TaskProgressNegativePercentage()
        {
            new TaskProgressEventArgs(Guid.Empty, -1);
        }

        [TestMethod]
        public void TaskProgress0()
        {
            new TaskProgressEventArgs(Guid.Empty, 0);
        }

        [TestMethod]
        public void TaskProgress100()
        {
            new TaskProgressEventArgs(Guid.Empty, 100);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TaskProgress101()
        {
            new TaskProgressEventArgs(Guid.Empty, 101);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void PerformanceMonitoringNegative()
        {
            new PerformanceMonitoringEventArgs(TimeSpan.FromSeconds(-1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void PerformanceMonitoring0()
        {
            new PerformanceMonitoringEventArgs(TimeSpan.Zero);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TaskProcessorPerformanceNullProcessor()
        {
            new TaskProcessorPerformanceEventArgs(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NotifyMasterModeChangedNone()
        {
            new MasterModeChangeEventArgs(Guid.Empty, false, MasterModeChangeReason.None);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NotifyMasterModeChangedStartSlave()
        {
            new MasterModeChangeEventArgs(Guid.Empty, false, MasterModeChangeReason.Start);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TaskFailedNullError()
        {
            new TaskFailedMasterCommand(Guid.Empty, Guid.Empty, DateTime.UtcNow, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TaskFailedEmptyError()
        {
            new TaskFailedMasterCommand(Guid.Empty, Guid.Empty, DateTime.UtcNow, string.Empty);
        }
    }
}