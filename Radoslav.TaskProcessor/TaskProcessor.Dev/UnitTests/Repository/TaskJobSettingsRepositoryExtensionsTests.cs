using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.Repository;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class TaskJobSettingsRepositoryExtensionsTestsBase : RepositoryTestsBase<FakeTaskJobSettingsRepository>
    {
        [TestMethod]
        public void Get1()
        {
            FakeTaskJobSettings settings1 = new FakeTaskJobSettings();

            this.Repository.Set(typeof(FakeTask), settings1);

            ITaskJobSettings settings2 = this.Repository.Get<FakeTask>();

            Assert.AreEqual(settings1, settings2);

            this.Repository.AssertMethodCallOnceWithArguments(r => r.Get(typeof(FakeTask)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetNullRepository1()
        {
            default(ITaskJobSettingsRepository).Get<FakeTask>();
        }

        [TestMethod]
        public void Get2()
        {
            FakeTaskJobSettings settings1 = new FakeTaskJobSettings();

            this.Repository.Set(typeof(FakeTask), settings1);

            FakeTaskJobSettings settings2 = this.Repository.Get<FakeTask, FakeTaskJobSettings>();

            Assert.AreEqual(settings1, settings2);

            this.Repository.AssertMethodCallOnceWithArguments(r => r.Get(typeof(FakeTask)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetNullRepository2()
        {
            default(ITaskJobSettingsRepository).Get<FakeTask, FakeTaskJobSettings>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void GetInvalidCast()
        {
            this.Repository.Set(typeof(FakeTask), new FakeTaskJobSettings());

            this.Repository.Get<FakeTask, FakeTaskJobSettings2>();
        }

        [TestMethod]
        public void Set()
        {
            FakeTaskJobSettings settings = new FakeTaskJobSettings();

            this.Repository.Set<FakeTask>(settings);

            this.Repository.AssertMethodCallOnceWithArguments(r => r.Set(typeof(FakeTask), settings));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetNullRepository()
        {
            default(ITaskJobSettingsRepository).Set<FakeTask>(new FakeTaskJobSettings());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetNullSettings()
        {
            this.Repository.Set<FakeTask>(null);
        }

        [TestMethod]
        public void Clear()
        {
            this.Repository.Clear<FakeTask>();

            this.Repository.AssertMethodCallOnceWithArguments(r => r.Clear(typeof(FakeTask)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ClearNullRepository()
        {
            default(ITaskJobSettingsRepository).Clear<FakeTask>();
        }

        protected override FakeTaskJobSettingsRepository CreateRepository()
        {
            return new FakeTaskJobSettingsRepository();
        }
    }
}