using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.Repository;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public abstract class TaskJobSettingsRepositoryTestsBase : RepositoryTestsBase<ITaskJobSettingsRepository>
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetNullTaskType()
        {
            this.Repository.Get(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetInvalidTaskType()
        {
            this.Repository.Get(typeof(object));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetNullTaskType()
        {
            this.Repository.Set(null, new FakeTaskJobSettings());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetNullTaskJobSettings()
        {
            this.Repository.Set(typeof(FakeTask), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetInvalidTaskType()
        {
            this.Repository.Set(typeof(object), new FakeTaskJobSettings());
        }

        [TestMethod]
        public void GetSet()
        {
            FakeTaskJobSettings settings1 = new FakeTaskJobSettings()
            {
                Username = "User",
                Password = "P@ssw0rd"
            };

            this.Repository.Set(typeof(FakeTask), settings1);

            FakeTaskJobSettings settings2 = (FakeTaskJobSettings)this.Repository.Get(typeof(FakeTask));

            Assert.AreEqual(settings1.Username, settings2.Username);
            Assert.AreEqual(settings1.Password, settings2.Password);
        }

        [TestMethod]
        public void Clear()
        {
            this.Repository.Set(typeof(FakeTask), new FakeTaskJobSettings());

            Assert.IsNotNull(this.Repository.Get(typeof(FakeTask)));

            this.Repository.Clear(typeof(FakeTask));

            Assert.IsNull(this.Repository.Get(typeof(FakeTask)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ClearNullTaskType()
        {
            this.Repository.Clear(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClearInvalidTaskType()
        {
            this.Repository.Clear(typeof(object));
        }
    }
}