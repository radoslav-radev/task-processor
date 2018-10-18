using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.MessageBus;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class MessageBusEventArgsUnitTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TaskCompletedPending()
        {
            new TaskCompletedEventArgs(0, TaskStatus.Pending, DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TaskCompletedInProgress()
        {
            new TaskCompletedEventArgs(0, TaskStatus.InProgress, DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TaskCompletedNegativeCpuTime()
        {
            new TaskCompletedEventArgs(0, TaskStatus.InProgress, DateTime.UtcNow, TimeSpan.FromSeconds(-1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TaskProgressNegativePercentage()
        {
            new TaskProgressEventArgs(0, -1);
        }

        [TestMethod]
        public void TaskProgress0()
        {
            new TaskProgressEventArgs(0, 0);
        }

        [TestMethod]
        public void TaskProgress100()
        {
            new TaskProgressEventArgs(0, 100);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TaskProgress101()
        {
            new TaskProgressEventArgs(0, 101);
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
    }
}