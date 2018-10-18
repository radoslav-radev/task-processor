using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.MessageBus;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class TaskProcessExecutorUnitTests
    {
        public TestContext TestContext { get; set; }

        private TaskProcessExecutor TaskExecutor
        {
            get
            {
                return (TaskProcessExecutor)this.TestContext.Properties["TaskExecutor"];
            }
        }

        private FakeChildProcessKiller ChildProcessKiller
        {
            get
            {
                return (FakeChildProcessKiller)this.TaskExecutor.ChildProcessKiller;
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.TestContext.Properties.Add("TaskExecutor", new TaskProcessExecutor("Radoslav.TaskProcessor.UnitTests.ConsoleApp.exe", new FakeChildProcessKiller()));

            this.TaskExecutor.IsUserInteractive = false;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.TaskExecutor.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullExecutableFilePath()
        {
            using (new TaskProcessExecutor(null, new FakeChildProcessKiller()))
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorEmptyExecutableFilePath()
        {
            using (new TaskProcessExecutor(string.Empty, new FakeChildProcessKiller()))
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullChildProcessKiller()
        {
            using (new TaskProcessExecutor("Dummy", null))
            {
            }
        }

        [TestMethod]
        public void StartTask()
        {
            this.TaskExecutor.StartTask(Guid.NewGuid(), TaskPriority.Normal);

            Assert.AreEqual(1, this.TaskExecutor.ActiveTasksCount);

            int processId = this.TaskExecutor.ActiveTaskProcesses.Single().Value;

            Process process = Process.GetProcessById(processId);

            Assert.IsNotNull(process);

            this.ChildProcessKiller.EqualityComparerFactory.RegisterCallback<Process>((p1, p2) => p1.Id == p2.Id);

            this.ChildProcessKiller.AssertMethodCallOnceWithArguments(k => k.AddProcess(process));
        }

        [TestMethod]
        [ExpectedException(typeof(Win32Exception))]
        public void ExecutableFileNotFound()
        {
            using (TaskProcessExecutor executor = new TaskProcessExecutor("404.exe", new FakeChildProcessKiller()))
            {
                executor.IsUserInteractive = false; // Otherwise process start hangs.

                executor.StartTask(Guid.Empty, TaskPriority.Normal);
            }
        }

        [TestMethod]
        public void KillProcessesOnDispose()
        {
            this.TaskExecutor.StartTask(Guid.NewGuid(), TaskPriority.Normal);

            int processId = this.TaskExecutor.ActiveTaskProcesses.Single().Value;

            this.TaskExecutor.Dispose();

            Thread.Sleep(TimeSpan.FromSeconds(1));

            try
            {
                Process.GetProcessById(processId);

                Assert.Fail("Task process {0} is still alive.".FormatInvariant(processId));
            }
            catch (ArgumentException)
            {
            }

            Assert.AreEqual(0, this.TaskExecutor.ActiveTasksCount);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CancelTimeoutNegative()
        {
            this.TaskExecutor.CancelTimeout = TimeSpan.FromSeconds(-1);
        }

        [TestMethod]
        public void CancelTimeoutZero()
        {
            this.TaskExecutor.CancelTimeout = TimeSpan.Zero;
        }

        [TestMethod]
        public void SuccessfulTask()
        {
            Guid taskId = Guid.NewGuid();

            TaskCompletedEventArgs args = Helpers.WaitForEvent<TaskCompletedEventArgs>(
                TimeSpan.FromSeconds(10),
                handler => this.TaskExecutor.TaskCompleted += handler,
                () => this.TaskExecutor.StartTask(taskId, TaskPriority.Normal));

            Assert.AreEqual(taskId, args.TaskId);

            Assert.AreEqual(0, this.TaskExecutor.ActiveTasksCount);
        }

        [TestMethod]
        public void FailedTask()
        {
            this.TaskExecutor.StartTask(Guid.Empty, TaskPriority.Normal);

            TaskEventArgs args = Helpers.WaitForEvent<TaskEventArgs>(
                TimeSpan.FromSeconds(5),
                handler => this.TaskExecutor.TaskFailed += handler);

            Assert.AreEqual(Guid.Empty, args.TaskId);

            Assert.AreEqual(0, this.TaskExecutor.ActiveTasksCount);
        }

        [TestMethod]
        public void CancelTask()
        {
            Guid taskId = Guid.NewGuid();

            this.TaskExecutor.StartTask(taskId, TaskPriority.Normal);

            TaskEventArgs args = Helpers.WaitForEvent<TaskEventArgs>(
                TimeSpan.FromSeconds(2),
                handler => this.TaskExecutor.TaskCanceled += handler,
                () => this.TaskExecutor.CancelTask(taskId));

            Assert.AreEqual(taskId, args.TaskId);

            Assert.AreEqual(0, this.TaskExecutor.ActiveTasksCount);
        }

        [TestMethod]
        public void LowPriority()
        {
            this.TaskPriorityTest(TaskPriority.Low, ProcessPriorityClass.BelowNormal);
        }

        [TestMethod]
        public void NormalPriority()
        {
            this.TaskPriorityTest(TaskPriority.Normal, ProcessPriorityClass.Normal);
        }

        [TestMethod]
        public void HighPriority()
        {
            this.TaskPriorityTest(TaskPriority.High, ProcessPriorityClass.AboveNormal);
        }

        [TestMethod]
        public void VeryHighPriority()
        {
            this.TaskPriorityTest(TaskPriority.VeryHigh, ProcessPriorityClass.High);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void DoNotRaiseTaskCompletedOnDispose()
        {
            this.TaskExecutor.StartTask(Guid.NewGuid(), TaskPriority.Normal);

            Helpers.WaitForEvent<TaskCompletedEventArgs>(
                TimeSpan.FromSeconds(2),
                handler => this.TaskExecutor.TaskCompleted += handler,
                () => this.TaskExecutor.Dispose());
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void StartTaskAfterDispose()
        {
            this.TaskExecutor.Dispose();

            this.TaskExecutor.StartTask(Guid.NewGuid(), TaskPriority.Normal);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CancelTaskAfterDispose()
        {
            this.TaskExecutor.Dispose();

            this.TaskExecutor.CancelTask(Guid.NewGuid());
        }

        [TestMethod]
        public void DisposeTwice()
        {
            this.TaskExecutor.Dispose();
            this.TaskExecutor.Dispose();
        }

        [TestMethod]
        public void StopMonitorPerformanceOnDispose()
        {
            this.TaskExecutor.MonitorPerformance = true;

            this.TaskExecutor.Dispose();

            Assert.IsFalse(this.TaskExecutor.MonitorPerformance);
        }

        [TestMethod]
        public void CancelTimeoutKillTask()
        {
            Guid taskId = Guid.Parse("D68A83A4-6D67-4EDF-B2C5-BCC899624BA7");

            this.TaskExecutor.CancelTimeout = TimeSpan.FromSeconds(2);

            this.TaskExecutor.StartTask(taskId, TaskPriority.Normal);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            ThreadPool.QueueUserWorkItem(state => this.TaskExecutor.CancelTask(taskId));

            Thread.Sleep(TimeSpan.FromSeconds(1.5));

            Assert.AreEqual(1, this.TaskExecutor.ActiveTasksCount);

            Thread.Sleep(TimeSpan.FromSeconds(1));

            Assert.AreEqual(0, this.TaskExecutor.ActiveTasksCount);
        }

        [TestMethod]
        public void CancelTimeoutTaskFinished()
        {
            Guid taskId = Guid.NewGuid();

            this.TaskExecutor.CancelTimeout = TimeSpan.FromSeconds(3);

            this.TaskExecutor.StartTask(taskId, TaskPriority.Normal);

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            this.TaskExecutor.CancelTask(taskId);

            Assert.AreEqual(0, this.TaskExecutor.ActiveTasksCount);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void CancelTaskAfterFinish()
        {
            Guid taskId = Guid.NewGuid();

            Helpers.WaitForEvent<TaskCompletedEventArgs>(
                TimeSpan.FromSeconds(1),
                handler => this.TaskExecutor.TaskCompleted += handler,
                () => this.TaskExecutor.StartTask(taskId, TaskPriority.Normal));

            this.TaskExecutor.CancelTask(taskId);
        }

        [TestMethod]
        public void CancelNotAvailableTask()
        {
            this.TaskExecutor.CancelTask(Guid.NewGuid());
        }

        private void TaskPriorityTest(TaskPriority taskPriority, ProcessPriorityClass processPriority)
        {
            this.TaskExecutor.StartTask(Guid.NewGuid(), taskPriority);

            int processId = this.TaskExecutor.ActiveTaskProcesses.Single().Value;

            Process process = Process.GetProcessById(processId);

            Assert.AreEqual(processPriority, process.PriorityClass);
        }
    }
}