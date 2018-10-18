using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.Configuration;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.Repository;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public abstract class TaskProcessorRuntimeInfoRepositoryUnitTests : RepositoryTestsBase<ITaskProcessorRuntimeInfoRepository>
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateTaskProcessorRuntimeInfoNullName()
        {
            this.Repository.Create(Guid.NewGuid(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateTaskProcessorRuntimeInfoEmptyName()
        {
            this.Repository.Create(Guid.NewGuid(), string.Empty);
        }

        [TestMethod]
        public void CreateTaskProcessorRuntimeInfo()
        {
            Guid processorId = Guid.NewGuid();

            ITaskProcessorRuntimeInfo processorInfo = this.Repository.Create(processorId, "Test");

            Assert.AreEqual(processorId, processorInfo.TaskProcessorId);
            Assert.AreEqual("Test", processorInfo.MachineName);
            Assert.IsNotNull(processorInfo.Configuration);
            Assert.IsNotNull(processorInfo.Configuration.Tasks);
            Assert.IsNull(processorInfo.Configuration.Tasks.MaxWorkers);
            Assert.IsFalse(processorInfo.Configuration.Tasks.Any());
        }

        [TestMethod]
        public void GetTaskProcessorRuntimeInfoById1()
        {
            ITaskProcessorRuntimeInfo processorInfo1 = this.Repository.Create(Guid.NewGuid(), "Test");

            processorInfo1.Configuration.Tasks.MaxWorkers = 100;

            processorInfo1.Configuration.Tasks.Add(typeof(IFakeTask)).MaxWorkers = 10;

            this.Repository.Add(processorInfo1);

            ITaskProcessorRuntimeInfo processorInfo2 = this.Repository.GetById(processorInfo1.TaskProcessorId);

            Assert.IsNotNull(processorInfo2);

            Assert.AreEqual(processorInfo1.TaskProcessorId, processorInfo2.TaskProcessorId);
            Assert.AreEqual(processorInfo1.MachineName, processorInfo2.MachineName);

            Assert.AreEqual(100, processorInfo2.Configuration.Tasks.MaxWorkers);
            Assert.AreEqual(1, processorInfo2.Configuration.Tasks.Count());
            Assert.AreEqual(10, processorInfo2.Configuration.Tasks[typeof(IFakeTask)].MaxWorkers);
        }

        [TestMethod]
        public void GetTaskProcessorRuntimeInfoById2()
        {
            ITaskProcessorRuntimeInfo processorInfo1 = this.Repository.Create(Guid.NewGuid(), "Test");

            processorInfo1.Configuration.Tasks.Add(typeof(IFakeTask));

            this.Repository.Add(processorInfo1);

            ITaskProcessorRuntimeInfo processorInfo2 = this.Repository.GetById(processorInfo1.TaskProcessorId);

            Assert.IsNotNull(processorInfo2);

            Assert.AreEqual(processorInfo1.TaskProcessorId, processorInfo2.TaskProcessorId);
            Assert.AreEqual(processorInfo1.MachineName, processorInfo2.MachineName);

            Assert.IsNull(processorInfo2.Configuration.Tasks.MaxWorkers);
            Assert.AreEqual(1, processorInfo2.Configuration.Tasks.Count());
            Assert.IsNull(processorInfo2.Configuration.Tasks[typeof(IFakeTask)].MaxWorkers);
        }

        [TestMethod]
        public void GetAllTaskProcessorRuntimeInfo()
        {
            ITaskProcessorRuntimeInfo processorInfo1 = this.Repository.Create(Guid.NewGuid(), "First");

            this.Repository.Add(processorInfo1);

            ITaskProcessorRuntimeInfo processorInfo2 = this.Repository.Create(Guid.NewGuid(), "Second");

            this.Repository.Add(processorInfo2);

            ITaskProcessorRuntimeInfo[] processorInfos = this.Repository.GetAll().ToArray();

            ITaskProcessorRuntimeInfo processorInfo11 = processorInfos.FirstOrDefault(p => p.TaskProcessorId == processorInfo1.TaskProcessorId);

            Assert.IsNotNull(processorInfo11);

            Assert.AreEqual(processorInfo1.TaskProcessorId, processorInfo11.TaskProcessorId);
            Assert.AreEqual(processorInfo1.MachineName, processorInfo11.MachineName);

            ITaskProcessorRuntimeInfo processorInfo22 = processorInfos.FirstOrDefault(p => p.TaskProcessorId == processorInfo2.TaskProcessorId);

            Assert.IsNotNull(processorInfo11);

            Assert.AreEqual(processorInfo2.TaskProcessorId, processorInfo22.TaskProcessorId);
            Assert.AreEqual(processorInfo2.MachineName, processorInfo22.MachineName);
        }

        [TestMethod]
        public void UpdateTaskProcessorRuntimeInfo()
        {
            ITaskProcessorRuntimeInfo processorInfo = this.Repository.Create(Guid.NewGuid(), "Test");

            processorInfo.Configuration.Tasks.Add(typeof(IFakeTask));

            this.Repository.Add(processorInfo);

            processorInfo.Configuration.Tasks.MaxWorkers = 100;

            processorInfo.Configuration.Tasks.Remove(typeof(IFakeTask));

            processorInfo.Configuration.Tasks.Add(typeof(IFakeTask2)).MaxWorkers = 10;

            processorInfo.Configuration.PollingJobs.Add(typeof(FakePollingJob));

            processorInfo.Configuration.PollingJobs[typeof(FakePollingJob)].PollInterval = TimeSpan.FromMinutes(1);
            processorInfo.Configuration.PollingJobs[typeof(FakePollingJob)].IsMaster = true;

            this.Repository.Update(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            Assert.AreEqual(100, processorInfo.Configuration.Tasks.MaxWorkers);
            Assert.AreEqual(1, processorInfo.Configuration.Tasks.Count());
            Assert.AreEqual(10, processorInfo.Configuration.Tasks[typeof(IFakeTask2)].MaxWorkers);

            Assert.AreEqual(TimeSpan.FromMinutes(1), processorInfo.Configuration.PollingJobs[typeof(FakePollingJob)].PollInterval);

            Assert.IsTrue(processorInfo.Configuration.PollingJobs[typeof(FakePollingJob)].IsMaster);
            Assert.IsFalse(processorInfo.Configuration.PollingJobs[typeof(FakePollingJob)].IsActive);
        }

        [TestMethod]
        public void DeleteTaskProcessorRuntimeInfo()
        {
            Guid taskProcessorId = Guid.NewGuid();

            this.Repository.Add(
                this.Repository.Create(taskProcessorId, "Test"));

            this.Repository.Delete(taskProcessorId);

            this.AssertTaskProcessorInfoIsNotAvailable(taskProcessorId);
        }

        [TestMethod]
        public void TaskProcessorRuntimeInfoInitialExpiration()
        {
            Assert.IsTrue(this.Repository.Expiration > TimeSpan.Zero);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TaskProcessorRuntimeInfoZeroExpiration()
        {
            this.Repository.Expiration = TimeSpan.Zero;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TaskProcessorRuntimeInfoNegativeExpiration()
        {
            this.Repository.Expiration = TimeSpan.FromMinutes(-1);
        }

        [TestMethod]
        public void ExpirationAdd()
        {
            this.Repository.Expiration = TimeSpan.FromSeconds(0.5);

            ITaskProcessorRuntimeInfo processorInfo = this.Repository.Create(Guid.NewGuid(), "Test");

            this.Repository.Add(processorInfo);

            Thread.Sleep(TimeSpan.FromSeconds(2));

            this.AssertTaskProcessorInfoIsNotAvailable(processorInfo.TaskProcessorId);
        }

        [TestMethod]
        public void ExpirationUpdate()
        {
            this.Repository.Expiration = TimeSpan.FromSeconds(0.5);

            ITaskProcessorRuntimeInfo processorInfo = this.Repository.Create(Guid.NewGuid(), "Test");

            this.Repository.Add(processorInfo);
            this.Repository.Update(processorInfo);

            Thread.Sleep(TimeSpan.FromSeconds(2));

            this.AssertTaskProcessorInfoIsNotAvailable(processorInfo.TaskProcessorId);
        }

        [TestMethod]
        public void TaskProcessorRuntimeInfoHeartbeat()
        {
            this.Repository.Expiration = TimeSpan.FromSeconds(1);

            Guid taskProcessorId = Guid.NewGuid();

            this.Repository.Add(
                this.Repository.Create(taskProcessorId, "Test"));

            Thread.Sleep(TimeSpan.FromSeconds(0.75));

            Assert.IsTrue(this.Repository.Heartbeat(taskProcessorId));

            Thread.Sleep(TimeSpan.FromSeconds(0.75));

            Assert.IsTrue(this.Repository.GetAll().Any(p => p.TaskProcessorId == taskProcessorId));

            Assert.IsNotNull(this.Repository.GetById(taskProcessorId));
        }

        [TestMethod]
        public void GetMasterTaskProcessor()
        {
            this.Repository.ClearMaster();

            Guid taskProcessorId = Guid.NewGuid();

            Assert.IsTrue(this.Repository.SetMasterIfNotExists(taskProcessorId));

            Assert.AreEqual(taskProcessorId, this.Repository.GetMasterId());
        }

        [TestMethod]
        public void SetMasterTaskProcessor()
        {
            this.Repository.ClearMaster();

            Guid taskProcessorId = Guid.NewGuid();

            this.Repository.SetMaster(taskProcessorId);

            Assert.AreEqual(taskProcessorId, this.Repository.GetMasterId());

            taskProcessorId = Guid.NewGuid();

            this.Repository.SetMaster(taskProcessorId);

            Assert.AreEqual(taskProcessorId, this.Repository.GetMasterId());
        }

        [TestMethod]
        public void SetMasterTaskProcessorIfNotExists()
        {
            this.Repository.ClearMaster();

            Guid taskProcessorId = Guid.NewGuid();

            Assert.IsTrue(this.Repository.SetMasterIfNotExists(taskProcessorId));

            Assert.IsFalse(this.Repository.SetMasterIfNotExists(Guid.NewGuid()));

            Assert.AreEqual(taskProcessorId, this.Repository.GetMasterId());
        }

        [TestMethod]
        public void ClearMasterTaskProcessor()
        {
            this.Repository.ClearMaster();

            Guid taskProcessorId = Guid.NewGuid();

            Assert.IsTrue(this.Repository.SetMasterIfNotExists(taskProcessorId));

            this.Repository.ClearMaster();

            Assert.IsNull(this.Repository.GetMasterId());
        }

        [TestMethod]
        public void MasterTaskProcessorExpiration()
        {
            this.Repository.ClearMaster();

            this.Repository.Expiration = TimeSpan.FromSeconds(0.5);

            this.Repository.SetMaster(Guid.NewGuid());

            Thread.Sleep(TimeSpan.FromSeconds(1));

            Assert.IsNull(this.Repository.GetMasterId());
        }

        [TestMethod]
        public void MasterTaskProcessorExpirationIfNotExists()
        {
            this.Repository.ClearMaster();

            this.Repository.Expiration = TimeSpan.FromSeconds(0.5);

            Assert.IsTrue(this.Repository.SetMasterIfNotExists(Guid.NewGuid()));

            Thread.Sleep(TimeSpan.FromSeconds(1));

            Assert.IsNull(this.Repository.GetMasterId());
        }

        [TestMethod]
        public void MasterTaskProcessorHeartbeat()
        {
            this.Repository.ClearMaster();

            Guid taskProcessorId = Guid.NewGuid();

            this.Repository.Expiration = TimeSpan.FromSeconds(1);

            Assert.IsTrue(this.Repository.SetMasterIfNotExists(taskProcessorId));

            Thread.Sleep(TimeSpan.FromSeconds(0.75));

            this.Repository.MasterHeartbeat();

            Thread.Sleep(TimeSpan.FromSeconds(0.75));

            Assert.AreEqual(taskProcessorId, this.Repository.GetMasterId());
        }

        #region Configuration

        #region Task Jobs

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullTaskJob()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.Tasks.Add(default(Type));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullTaskJobCopy()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.Tasks.AddCopy(default(ITaskJobConfiguration));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddDuplicateTaskJob1()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.Tasks.Add(typeof(FakeTask));
            processorInfo.Configuration.Tasks.Add(typeof(FakeTask));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddDuplicateTaskJob2()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            processorInfo.Configuration.Tasks.Add(typeof(FakeTask));

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.Tasks.AddCopy(new FakeTaskJobConfiguration()
            {
                TaskType = typeof(FakeTask)
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetTaskJobNull()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            processorInfo.Configuration.Tasks.Add(typeof(IFakeTask));

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            if (processorInfo.Configuration.Tasks[null] == null)
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveTaskJobNull()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.Tasks.Remove(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NegativeTaskJobsMaxWorkers()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.Tasks.MaxWorkers = -1;
        }

        [TestMethod]
        public void ZeroTaskJobsMaxWorkers()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.Tasks.MaxWorkers = 0;

            this.Repository.Update(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            Assert.AreEqual(0, processorInfo.Configuration.Tasks.MaxWorkers);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NegativeTaskJobMaxWorkers()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),

                Configuration = new FakeTaskProcessorConfiguration()
            };

            processorInfo.Configuration.Tasks.Add(typeof(IFakeTask));

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.Tasks[typeof(IFakeTask)].MaxWorkers = -1;
        }

        [TestMethod]
        public void ZeroTaskJobMaxWorkers()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),

                Configuration = new FakeTaskProcessorConfiguration()
            };

            processorInfo.Configuration.Tasks.Add(typeof(IFakeTask));

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.Tasks[typeof(IFakeTask)].MaxWorkers = 0;

            this.Repository.Update(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            Assert.AreEqual(0, processorInfo.Configuration.Tasks[typeof(IFakeTask)].MaxWorkers);
        }

        [TestMethod]
        public void RemoveTaskJobSystemConfiguration()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),

                Configuration = new FakeTaskProcessorConfiguration()
            };

            processorInfo.Configuration.Tasks.Add(typeof(IFakeTask));

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            Assert.IsNotNull(processorInfo.Configuration.Tasks[typeof(IFakeTask)]);

            processorInfo.Configuration.Tasks.Remove(typeof(IFakeTask));

            Assert.IsNull(processorInfo.Configuration.Tasks[typeof(IFakeTask)]);

            this.Repository.Update(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            Assert.IsNull(processorInfo.Configuration.Tasks[typeof(IFakeTask)]);
        }

        #endregion Task Jobs

        #region Polling Jobs

        [TestMethod]
        public void PollingJobsConfiguration()
        {
            FakeTaskProcessorRuntimeInfo processorInfo1 = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),

                Configuration = new FakeTaskProcessorConfiguration()
            };

            processorInfo1.Configuration.PollingJobs.Add(new FakePollingJobConfiguration()
            {
                ImplementationType = typeof(FakePollingJob),
                PollInterval = TimeSpan.FromMinutes(1),
                IsMaster = true
            });

            processorInfo1.Configuration.PollingJobs.Add(new FakePollingJobConfiguration()
            {
                ImplementationType = typeof(FakePollingJob2),
                PollInterval = TimeSpan.FromMinutes(2),
                IsActive = true
            });

            processorInfo1.Configuration.PollingJobs.Add(new FakePollingJobConfiguration()
            {
                ImplementationType = typeof(FakePollingJob3),
                PollInterval = TimeSpan.FromMinutes(3),
                IsConcurrent = true
            });

            this.Repository.Add(processorInfo1);

            ITaskProcessorRuntimeInfo processorInfo2 = this.Repository.GetById(processorInfo1.TaskProcessorId);

            Assert.AreEqual(3, processorInfo2.Configuration.PollingJobs.Count());

            IPollingJobConfiguration queue1 = processorInfo2.Configuration.PollingJobs.First(q => q.ImplementationType == typeof(FakePollingJob));

            Assert.AreEqual(TimeSpan.FromMinutes(1), queue1.PollInterval);
            Assert.IsTrue(queue1.IsMaster);
            Assert.IsFalse(queue1.IsActive);
            Assert.IsFalse(queue1.IsConcurrent);

            IPollingJobConfiguration queue2 = processorInfo2.Configuration.PollingJobs.First(q => q.ImplementationType == typeof(FakePollingJob2));

            Assert.AreEqual(TimeSpan.FromMinutes(2), queue2.PollInterval);
            Assert.IsFalse(queue2.IsMaster);
            Assert.IsTrue(queue2.IsActive);
            Assert.IsFalse(queue2.IsConcurrent);

            IPollingJobConfiguration queue3 = processorInfo2.Configuration.PollingJobs.First(q => q.ImplementationType == typeof(FakePollingJob3));

            Assert.AreEqual(TimeSpan.FromMinutes(3), queue3.PollInterval);
            Assert.IsFalse(queue3.IsMaster);
            Assert.IsFalse(queue3.IsActive);
            Assert.IsTrue(queue3.IsConcurrent);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NegativePollingJobInterval()
        {
            this.InvalidPollingJobInterval(TimeSpan.FromSeconds(-1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ZeroPollingJobInterval()
        {
            this.InvalidPollingJobInterval(TimeSpan.Zero);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetPollingJobNullImplementationType()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),

                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            if (processorInfo.Configuration.PollingJobs[null] != null)
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPollingJobImplementationTypeNotDescendant1()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),

                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            if (processorInfo.Configuration.PollingJobs[typeof(IPollingJob)] != null)
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPollingJobImplementationTypeNotDescendant2()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),

                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            if (processorInfo.Configuration.PollingJobs[typeof(ICloneable)] != null)
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddPollingJobNullImplementationType()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),

                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.PollingJobs.Add(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddPollingJobImplementationTypeNotDescendant1()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),

                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.PollingJobs.Add(typeof(IPollingJob));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddPollingJobImplementationTypeNotDescendant2()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.PollingJobs.Add(typeof(ICloneable));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddPollingJobDuplicate()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.PollingJobs.Add(typeof(FakePollingJob));
            processorInfo.Configuration.PollingJobs.Add(typeof(FakePollingJob));
        }

        [TestMethod]
        public void AddPollingJob()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            IPollingJobConfiguration result = processorInfo.Configuration.PollingJobs.Add(typeof(FakePollingJob));

            Assert.AreEqual(typeof(FakePollingJob), result.ImplementationType);

            result = processorInfo.Configuration.PollingJobs[typeof(FakePollingJob)];

            Assert.AreEqual(typeof(FakePollingJob), result.ImplementationType);
        }

        [TestMethod]
        public void AddPollingJobCopy()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.PollingJobs.AddCopy(new FakePollingJobConfiguration()
            {
                ImplementationType = typeof(FakePollingJob),
                PollInterval = TimeSpan.FromMinutes(1),
                IsMaster = true,
                IsActive = true
            });

            IPollingJobConfiguration result = processorInfo.Configuration.PollingJobs[typeof(FakePollingJob)];

            Assert.AreEqual(typeof(FakePollingJob), result.ImplementationType);
            Assert.AreEqual(TimeSpan.FromMinutes(1), result.PollInterval);
            Assert.IsTrue(result.IsMaster);
            Assert.IsTrue(result.IsActive);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddPollingJobCopyNullImplementationType()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.PollingJobs.AddCopy(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddPollingJobCopyDuplicate()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.PollingJobs.Add(typeof(FakePollingJob));

            processorInfo.Configuration.PollingJobs.AddCopy(new FakePollingJobConfiguration()
            {
                ImplementationType = typeof(FakePollingJob),
                PollInterval = TimeSpan.FromMinutes(1)
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemovePollingJobConfigurationNullImplementationType()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.PollingJobs.Remove(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RemovePollingJobConfigurationNotDescendant1()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.PollingJobs.Remove(typeof(IPollingJob));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RemovePollingJobConfigurationNotDescendant2()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.PollingJobs.Remove(typeof(ICloneable));
        }

        [TestMethod]
        public void RemovePollingJobConfiguration()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            processorInfo.Configuration.PollingJobs.Add(typeof(FakePollingJob)).PollInterval = TimeSpan.FromMinutes(1);

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            Assert.IsNotNull(processorInfo.Configuration.PollingJobs[typeof(FakePollingJob)]);

            processorInfo.Configuration.PollingJobs.Remove(typeof(FakePollingJob));

            Assert.IsNull(processorInfo.Configuration.PollingJobs[typeof(FakePollingJob)]);
        }

        #endregion Polling Jobs

        #region Polling Queues

        [TestMethod]
        public void PollingQueuesConfiguration()
        {
            FakeTaskProcessorRuntimeInfo processorInfo1 = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            processorInfo1.Configuration.PollingQueues.Add(new FakePollingQueueConfiguration()
            {
                Key = "A",
                PollInterval = TimeSpan.FromMinutes(1),
                IsMaster = true
            });

            processorInfo1.Configuration.PollingQueues.Add(new FakePollingQueueConfiguration()
            {
                Key = "B",
                PollInterval = TimeSpan.FromMinutes(2),
                IsActive = true
            });

            processorInfo1.Configuration.PollingQueues.Add(new FakePollingQueueConfiguration()
            {
                Key = "C",
                PollInterval = TimeSpan.FromMinutes(3),
                IsConcurrent = true
            });

            this.Repository.Add(processorInfo1);

            ITaskProcessorRuntimeInfo processorInfo2 = this.Repository.GetById(processorInfo1.TaskProcessorId);

            Assert.AreEqual(3, processorInfo2.Configuration.PollingQueues.Count());

            ITaskProcessorPollingQueueConfiguration queue = processorInfo2.Configuration.PollingQueues.First(q => q.Key == "A");

            Assert.AreEqual(TimeSpan.FromMinutes(1), queue.PollInterval);
            Assert.IsTrue(queue.IsMaster);
            Assert.IsFalse(queue.IsActive);
            Assert.IsFalse(queue.IsConcurrent);

            queue = processorInfo2.Configuration.PollingQueues.First(q => q.Key == "B");

            Assert.AreEqual(TimeSpan.FromMinutes(2), queue.PollInterval);
            Assert.IsFalse(queue.IsMaster);
            Assert.IsTrue(queue.IsActive);
            Assert.IsFalse(queue.IsConcurrent);

            queue = processorInfo2.Configuration.PollingQueues.First(q => q.Key == "C");

            Assert.AreEqual(TimeSpan.FromMinutes(3), queue.PollInterval);
            Assert.IsFalse(queue.IsMaster);
            Assert.IsFalse(queue.IsActive);
            Assert.IsTrue(queue.IsConcurrent);
        }

        [TestMethod]
        public void AddPollingQueue()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.PollingQueues.Add("A");

            processorInfo.Configuration.PollingQueues["A"].PollInterval = TimeSpan.FromSeconds(1);
            processorInfo.Configuration.PollingQueues["A"].IsActive = true;

            processorInfo.Configuration.PollingQueues.Add("B");

            processorInfo.Configuration.PollingQueues["B"].PollInterval = TimeSpan.FromSeconds(2);
            processorInfo.Configuration.PollingQueues["B"].IsMaster = true;
            processorInfo.Configuration.PollingQueues["B"].MaxWorkers = 0;

            processorInfo.Configuration.PollingQueues.Add("C");

            processorInfo.Configuration.PollingQueues["C"].PollInterval = TimeSpan.FromSeconds(3);
            processorInfo.Configuration.PollingQueues["C"].IsConcurrent = true;
            processorInfo.Configuration.PollingQueues["C"].MaxWorkers = 12;

            this.Repository.Update(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            Assert.AreEqual("A", processorInfo.Configuration.PollingQueues["A"].Key);
            Assert.AreEqual(TimeSpan.FromSeconds(1), processorInfo.Configuration.PollingQueues["A"].PollInterval);
            Assert.IsTrue(processorInfo.Configuration.PollingQueues["A"].IsActive);
            Assert.IsFalse(processorInfo.Configuration.PollingQueues["A"].IsMaster);
            Assert.IsFalse(processorInfo.Configuration.PollingQueues["A"].IsConcurrent);
            Assert.AreEqual(0, processorInfo.Configuration.PollingQueues["A"].MaxWorkers);

            Assert.AreEqual("B", processorInfo.Configuration.PollingQueues["B"].Key);
            Assert.AreEqual(TimeSpan.FromSeconds(2), processorInfo.Configuration.PollingQueues["B"].PollInterval);
            Assert.IsFalse(processorInfo.Configuration.PollingQueues["B"].IsActive);
            Assert.IsTrue(processorInfo.Configuration.PollingQueues["B"].IsMaster);
            Assert.IsFalse(processorInfo.Configuration.PollingQueues["B"].IsConcurrent);
            Assert.AreEqual(0, processorInfo.Configuration.PollingQueues["B"].MaxWorkers);

            Assert.AreEqual("C", processorInfo.Configuration.PollingQueues["C"].Key);
            Assert.AreEqual(TimeSpan.FromSeconds(3), processorInfo.Configuration.PollingQueues["C"].PollInterval);
            Assert.IsFalse(processorInfo.Configuration.PollingQueues["C"].IsActive);
            Assert.IsFalse(processorInfo.Configuration.PollingQueues["C"].IsMaster);
            Assert.IsTrue(processorInfo.Configuration.PollingQueues["C"].IsConcurrent);
            Assert.AreEqual(12, processorInfo.Configuration.PollingQueues["C"].MaxWorkers);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddPollingQueueNullKey()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.PollingQueues.Add(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddPollingQueueEmptyKey()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.PollingQueues.Add(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddPollingQueueDuplicate()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.PollingQueues.Add("A");
            processorInfo.Configuration.PollingQueues.Add("A");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddPollingQueueCopyNull()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.PollingQueues.AddCopy(null);
        }

        [TestMethod]
        public void AddPollingQueueCopy()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.PollingQueues.AddCopy(new FakePollingQueueConfiguration()
            {
                Key = "A",
                PollInterval = TimeSpan.FromSeconds(1),
                IsActive = true
            });

            processorInfo.Configuration.PollingQueues.AddCopy(new FakePollingQueueConfiguration()
            {
                Key = "B",
                PollInterval = TimeSpan.FromSeconds(2),
                IsMaster = true,
                MaxWorkers = 0
            });

            processorInfo.Configuration.PollingQueues.AddCopy(new FakePollingQueueConfiguration()
            {
                Key = "C",
                PollInterval = TimeSpan.FromSeconds(3),
                IsConcurrent = true,
                MaxWorkers = 12
            });

            this.Repository.Update(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            Assert.AreEqual("A", processorInfo.Configuration.PollingQueues["A"].Key);
            Assert.AreEqual(TimeSpan.FromSeconds(1), processorInfo.Configuration.PollingQueues["A"].PollInterval);
            Assert.IsTrue(processorInfo.Configuration.PollingQueues["A"].IsActive);
            Assert.IsFalse(processorInfo.Configuration.PollingQueues["A"].IsMaster);
            Assert.IsFalse(processorInfo.Configuration.PollingQueues["A"].IsConcurrent);
            Assert.AreEqual(0, processorInfo.Configuration.PollingQueues["A"].MaxWorkers);

            Assert.AreEqual("B", processorInfo.Configuration.PollingQueues["B"].Key);
            Assert.AreEqual(TimeSpan.FromSeconds(2), processorInfo.Configuration.PollingQueues["B"].PollInterval);
            Assert.IsFalse(processorInfo.Configuration.PollingQueues["B"].IsActive);
            Assert.IsTrue(processorInfo.Configuration.PollingQueues["B"].IsMaster);
            Assert.IsFalse(processorInfo.Configuration.PollingQueues["B"].IsConcurrent);
            Assert.AreEqual(0, processorInfo.Configuration.PollingQueues["B"].MaxWorkers);

            Assert.AreEqual("C", processorInfo.Configuration.PollingQueues["C"].Key);
            Assert.AreEqual(TimeSpan.FromSeconds(3), processorInfo.Configuration.PollingQueues["C"].PollInterval);
            Assert.IsFalse(processorInfo.Configuration.PollingQueues["C"].IsActive);
            Assert.IsFalse(processorInfo.Configuration.PollingQueues["C"].IsMaster);
            Assert.IsTrue(processorInfo.Configuration.PollingQueues["C"].IsConcurrent);
            Assert.AreEqual(12, processorInfo.Configuration.PollingQueues["C"].MaxWorkers);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddPollingQueueCopyDuplicate()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.PollingQueues.Add("A");

            processorInfo.Configuration.PollingQueues.AddCopy(new FakePollingQueueConfiguration()
            {
                Key = "A",
                PollInterval = TimeSpan.FromSeconds(1)
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NegativePollingQueueMaxWorkers()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            processorInfo.Configuration.PollingQueues.Add("A").MaxWorkers = -1;
        }

        [TestMethod]
        public void RemovePollingQueue()
        {
            ITaskProcessorRuntimeInfo processorInfo = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            processorInfo.Configuration.PollingQueues.Add("A").PollInterval = TimeSpan.FromMinutes(1);

            this.Repository.Add(processorInfo);

            processorInfo = this.Repository.GetById(processorInfo.TaskProcessorId);

            Assert.IsNotNull(processorInfo.Configuration.PollingQueues["A"]);

            processorInfo.Configuration.PollingQueues.Remove("A");

            Assert.IsNull(processorInfo.Configuration.PollingQueues["A"]);
        }

        #endregion Polling Queues

        #endregion Configuration

        private void AssertTaskProcessorInfoIsNotAvailable(Guid taskProcessorId)
        {
            Assert.IsNull(this.Repository.GetById(taskProcessorId));

            Assert.IsFalse(this.Repository.GetAll().Any(p => p.TaskProcessorId == taskProcessorId));

            Assert.IsFalse(this.Repository.Heartbeat(taskProcessorId));
        }

        private void InvalidPollingJobInterval(TimeSpan interval)
        {
            FakeTaskProcessorRuntimeInfo processorInfo1 = new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = Guid.NewGuid(),
                Configuration = new FakeTaskProcessorConfiguration()
            };

            processorInfo1.Configuration.PollingJobs.Add(new FakePollingJobConfiguration()
            {
                ImplementationType = typeof(FakePollingJob),
                PollInterval = TimeSpan.FromMinutes(1)
            });

            this.Repository.Add(processorInfo1);

            ITaskProcessorRuntimeInfo processorInfo2 = this.Repository.GetById(processorInfo1.TaskProcessorId);

            IPollingJobConfiguration queue1 = processorInfo2.Configuration.PollingJobs.First(q => q.ImplementationType == typeof(FakePollingJob));

            queue1.PollInterval = interval;
        }
    }
}