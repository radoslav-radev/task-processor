using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Diagnostics;

namespace Radoslav.Common.UnitTests
{
    [TestClass]
    public abstract class ChildProcessKillerTestsBase
    {
        public TestContext TestContext { get; set; }

        private IChildProcessKiller ChildProcessKiller
        {
            get
            {
                return (IChildProcessKiller)this.TestContext.Properties["ChildProcessKiller"];
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.TestContext.Properties.Add("ChildProcessKiller", this.CreateChildProcessKiller());
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (this.ChildProcessKiller is IDisposable)
            {
                ((IDisposable)this.ChildProcessKiller).Dispose();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddProcessNull()
        {
            this.ChildProcessKiller.AddProcess(null);
        }

        [TestMethod]
        public void AddProcess()
        {
            Process parent = new Process();

            parent.StartInfo.FileName = "Radoslav.Common.UnitTests.ConsoleApp.exe";

            parent.StartInfo.Arguments = "ChildProcessKiller \"{0}\" \"{1}\" {2}".FormatInvariant(
                this.ChildProcessKiller.GetType().AssemblyQualifiedName,
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "system32", "calc"),
                1);

            Assert.IsTrue(parent.Start());

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            parent.Kill();

            Thread.Sleep(TimeSpan.FromSeconds(0.5));

            Assert.IsFalse(Process.GetProcesses().Any(p => Helpers.GetParentProcessId(p.Id) == parent.Id));
        }

        protected abstract IChildProcessKiller CreateChildProcessKiller();
    }
}