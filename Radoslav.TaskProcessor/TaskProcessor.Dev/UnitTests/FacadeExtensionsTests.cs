using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.Facade;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class FacadeExtensionsTests
    {
        public TestContext TestContext { get; set; }

        private FakeTaskProcessorFacade Facade
        {
            get
            {
                return (FakeTaskProcessorFacade)this.TestContext.Properties["Facade"];
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.TestContext.Properties.Add("Facade", new FakeTaskProcessorFacade());
        }

        [TestMethod]
        public void Get1()
        {
            FakeTaskJobSettings settings1 = new FakeTaskJobSettings();

            this.Facade.PredefineResult(settings1, r => r.GetTaskJobSettings(typeof(FakeTask)));

            ITaskJobSettings settings2 = this.Facade.GetTaskJobSettings<FakeTask>();

            Assert.AreEqual(settings1, settings2);

            this.Facade.AssertMethodCallOnceWithArguments(r => r.GetTaskJobSettings(typeof(FakeTask)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetNullRepository1()
        {
            default(ITaskProcessorFacade).GetTaskJobSettings<FakeTask>();
        }

        [TestMethod]
        public void Get2()
        {
            FakeTaskJobSettings settings1 = new FakeTaskJobSettings();

            this.Facade.PredefineResult(settings1, r => r.GetTaskJobSettings(typeof(FakeTask)));

            FakeTaskJobSettings settings2 = this.Facade.GetTaskJobSettings<FakeTask, FakeTaskJobSettings>();

            Assert.AreEqual(settings1, settings2);

            this.Facade.AssertMethodCallOnceWithArguments(r => r.GetTaskJobSettings(typeof(FakeTask)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetNullRepository2()
        {
            default(ITaskProcessorFacade).GetTaskJobSettings<FakeTask, FakeTaskJobSettings>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void GetInvalidCast()
        {
            this.Facade.PredefineResult(new FakeTaskJobSettings(), r => r.GetTaskJobSettings(typeof(FakeTask)));

            this.Facade.GetTaskJobSettings<FakeTask, FakeTaskJobSettings2>();
        }

        [TestMethod]
        public void Set()
        {
            FakeTaskJobSettings settings = new FakeTaskJobSettings();

            this.Facade.SetTaskJobSettings<FakeTask>(settings);

            this.Facade.AssertMethodCallOnceWithArguments(r => r.SetTaskJobSettings(typeof(FakeTask), settings));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetNullRepository()
        {
            default(ITaskProcessorFacade).SetTaskJobSettings<FakeTask>(new FakeTaskJobSettings());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetNullSettings()
        {
            this.Facade.SetTaskJobSettings<FakeTask>(null);
        }

        [TestMethod]
        public void Clear()
        {
            this.Facade.ClearTaskJobSettings<FakeTask>();

            this.Facade.AssertMethodCallOnceWithArguments(r => r.ClearTaskJobSettings(typeof(FakeTask)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ClearNullRepository()
        {
            default(ITaskProcessorFacade).ClearTaskJobSettings<FakeTask>();
        }
    }
}