using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.Repository;
using Radoslav.TaskProcessor.TaskScheduler;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public abstract class ScheduledTaskRepositoryTests : RepositoryTestsBase<IScheduledTaskRepository>
    {
        private TimeSpan Timeout
        {
            get
            {
                return TimeSpan.FromSeconds(2);
            }
        }

        [TestMethod]
        public void Add()
        {
            FakeSerializableScheduledTask scheduledTask1 = new FakeSerializableScheduledTask()
            {
                Id = Guid.NewGuid(),
                Value = DateTime.Now.Millisecond,
                Schedule = new OneTimeScheduleDefinition(DateTime.UtcNow)
            };

            this.Repository.Add(scheduledTask1);

            FakeSerializableScheduledTask scheduledTask2 = (FakeSerializableScheduledTask)this.Repository.GetById(scheduledTask1.Id);

            Assert.AreEqual(scheduledTask1.Value, scheduledTask2.Value);

            this.AssertEquals(((OneTimeScheduleDefinition)scheduledTask1.Schedule).ScheduledDateTimeUtc, ((OneTimeScheduleDefinition)scheduledTask2.Schedule).ScheduledDateTimeUtc);

            FakeSerializableScheduledTask scheduledTask3 = (FakeSerializableScheduledTask)this.Repository.GetAll().First(t => t.Id == scheduledTask1.Id);

            Assert.AreEqual(scheduledTask1.Value, scheduledTask3.Value);

            this.AssertEquals(((OneTimeScheduleDefinition)scheduledTask1.Schedule).ScheduledDateTimeUtc, ((OneTimeScheduleDefinition)scheduledTask3.Schedule).ScheduledDateTimeUtc);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddScheduledTaskNull()
        {
            this.Repository.Add(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddRecurrenceDefinitionNull()
        {
            this.Repository.Add(new FakeSerializableScheduledTask());
        }

        [TestMethod]
        public void Update()
        {
            FakeSerializableScheduledTask scheduledTask1 = new FakeSerializableScheduledTask()
            {
                Id = Guid.NewGuid(),
                Value = DateTime.Now.Millisecond,
                Schedule = new OneTimeScheduleDefinition(DateTime.UtcNow.AddHours(-1))
            };

            this.Repository.Add(scheduledTask1);

            scheduledTask1.Value = DateTime.Now.Ticks;

            ((OneTimeScheduleDefinition)scheduledTask1.Schedule).ScheduledDateTimeUtc = DateTime.UtcNow;

            this.Repository.Update(scheduledTask1);

            FakeSerializableScheduledTask scheduledTask2 = (FakeSerializableScheduledTask)this.Repository.GetById(scheduledTask1.Id);

            Assert.AreEqual(scheduledTask1.Value, scheduledTask2.Value);

            this.AssertEquals(((OneTimeScheduleDefinition)scheduledTask1.Schedule).ScheduledDateTimeUtc, ((OneTimeScheduleDefinition)scheduledTask2.Schedule).ScheduledDateTimeUtc);

            FakeSerializableScheduledTask scheduledTask3 = (FakeSerializableScheduledTask)this.Repository.GetAll().First(t => t.Id == scheduledTask1.Id);

            Assert.AreEqual(scheduledTask1.Value, scheduledTask3.Value);

            this.AssertEquals(((OneTimeScheduleDefinition)scheduledTask1.Schedule).ScheduledDateTimeUtc, ((OneTimeScheduleDefinition)scheduledTask3.Schedule).ScheduledDateTimeUtc);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateScheduledTaskNull()
        {
            this.Repository.Update(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateRecurrenceDefinitionNull()
        {
            this.Repository.Update(new FakeSerializableScheduledTask());
        }

        [TestMethod]
        public void Delete()
        {
            FakeSerializableScheduledTask scheduledTask = new FakeSerializableScheduledTask()
            {
                Id = Guid.NewGuid(),
                Schedule = new OneTimeScheduleDefinition(DateTime.UtcNow)
            };

            this.Repository.Add(scheduledTask);

            this.Repository.Delete(scheduledTask.Id);

            Assert.IsNull(this.Repository.GetById(scheduledTask.Id));

            Assert.IsFalse(this.Repository.GetAll().Any(t => t.Id == scheduledTask.Id));
        }

        [TestMethod]
        public void NotifyAdd()
        {
            this.Repository.RaiseEvents = true;

            FakeSerializableScheduledTask scheduledTask = new FakeSerializableScheduledTask()
            {
                Id = Guid.NewGuid(),
                Schedule = new OneTimeScheduleDefinition(DateTime.UtcNow)
            };

            ScheduledTaskEventArgs args = Helpers.WaitForEvent<ScheduledTaskEventArgs>(this.Timeout,
                handler => this.Repository.Added += handler,
                () => this.Repository.Add(scheduledTask));

            Assert.AreEqual(scheduledTask.Id, args.ScheduledTaskId);
        }

        [TestMethod]
        public void NotifyUpdate()
        {
            FakeSerializableScheduledTask scheduledTask = new FakeSerializableScheduledTask()
            {
                Id = Guid.NewGuid(),
                Schedule = new OneTimeScheduleDefinition(DateTime.UtcNow)
            };

            this.Repository.Add(scheduledTask);

            this.Repository.RaiseEvents = true;

            ScheduledTaskEventArgs args = Helpers.WaitForEvent<ScheduledTaskEventArgs>(this.Timeout,
                handler => this.Repository.Updated += handler,
                () => this.Repository.Update(scheduledTask));

            Assert.AreEqual(scheduledTask.Id, args.ScheduledTaskId);
        }

        [TestMethod]
        public void NotifyDelete()
        {
            FakeSerializableScheduledTask scheduledTask = new FakeSerializableScheduledTask()
            {
                Id = Guid.NewGuid(),
                Schedule = new OneTimeScheduleDefinition(DateTime.UtcNow)
            };

            this.Repository.Add(scheduledTask);

            this.Repository.RaiseEvents = true;

            ScheduledTaskEventArgs args = Helpers.WaitForEvent<ScheduledTaskEventArgs>(this.Timeout,
                handler => this.Repository.Deleted += handler,
                () => this.Repository.Delete(scheduledTask.Id));

            Assert.AreEqual(scheduledTask.Id, args.ScheduledTaskId);
        }

        protected virtual void AssertEquals(DateTime value1, DateTime value2)
        {
            Assert.AreEqual(value1, value2);
        }
    }
}