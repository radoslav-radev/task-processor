using System;
using System.Collections.Generic;
using System.Linq;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.Repository;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakeTaskProcessorRuntimeInfoRepository : MockObject, ITaskProcessorRuntimeInfoRepository
    {
        private readonly Dictionary<Guid, FakeTaskProcessorRuntimeInfo> taskProcessors = new Dictionary<Guid, FakeTaskProcessorRuntimeInfo>();

        private Guid? masterTaskProcessorId;
        private DateTime masterExpiresAt;

        internal FakeTaskProcessorRuntimeInfoRepository()
        {
            this.Expiration = TimeSpan.FromMinutes(1);
        }

        #region ITaskProcessorRuntimeInfoRepository Members

        public TimeSpan Expiration { get; set; }

        public IEnumerable<ITaskProcessorRuntimeInfo> GetAll()
        {
            this.RecordMethodCall();

            return this.taskProcessors.Values.Where(p => p.ExpireAt > DateTime.Now);
        }

        public ITaskProcessorRuntimeInfo GetById(Guid taskProcessorId)
        {
            this.RecordMethodCall(taskProcessorId);

            FakeTaskProcessorRuntimeInfo result;

            this.taskProcessors.TryGetValue(taskProcessorId, out result);

            if ((result == null) || (result.ExpireAt < DateTime.Now))
            {
                return null;
            }

            return result;
        }

        public ITaskProcessorRuntimeInfo Create(Guid taskProcessorId, string machineName)
        {
            this.RecordMethodCall(taskProcessorId, machineName);

            return new FakeTaskProcessorRuntimeInfo()
            {
                TaskProcessorId = taskProcessorId,
                MachineName = machineName,
                Configuration = new FakeTaskProcessorConfiguration()
            };
        }

        public void Add(ITaskProcessorRuntimeInfo taskProcessorInfo)
        {
            if (taskProcessorInfo == null)
            {
                throw new ArgumentNullException("taskProcessorInfo");
            }

            this.RecordMethodCall(taskProcessorInfo);

            FakeTaskProcessorRuntimeInfo fakeProcessor = (FakeTaskProcessorRuntimeInfo)taskProcessorInfo;

            fakeProcessor.ExpireAt = DateTime.Now + this.Expiration;

            this.taskProcessors.Add(taskProcessorInfo.TaskProcessorId, fakeProcessor);
        }

        public void Update(ITaskProcessorRuntimeInfo taskProcessorInfo)
        {
            if (taskProcessorInfo == null)
            {
                throw new ArgumentNullException("taskProcessorInfo");
            }

            this.RecordMethodCall(taskProcessorInfo);

            this.taskProcessors[taskProcessorInfo.TaskProcessorId] = (FakeTaskProcessorRuntimeInfo)taskProcessorInfo;
        }

        public bool Heartbeat(Guid taskProcessorId)
        {
            this.RecordMethodCall(taskProcessorId);

            this.ExecutePredefinedMethod(taskProcessorId);

            if (this.HasPredefinedResult<bool>(taskProcessorId))
            {
                return this.GetPredefinedResult<bool>(taskProcessorId);
            }

            FakeTaskProcessorRuntimeInfo processor;

            if (!this.taskProcessors.TryGetValue(taskProcessorId, out processor) || (processor.ExpireAt < DateTime.Now))
            {
                return false;
            }

            processor.ExpireAt = DateTime.Now + this.Expiration;

            return true;
        }

        public void Delete(Guid taskProcessorId)
        {
            this.RecordMethodCall(taskProcessorId);

            this.taskProcessors.Remove(taskProcessorId);
        }

        public Guid? GetMasterId()
        {
            this.RecordMethodCall();

            if (this.masterExpiresAt < DateTime.Now)
            {
                return null;
            }

            return this.masterTaskProcessorId;
        }

        public void SetMaster(Guid taskProcessorId)
        {
            this.RecordMethodCall(taskProcessorId);

            this.masterTaskProcessorId = taskProcessorId;

            this.masterExpiresAt = DateTime.Now + this.Expiration;
        }

        public bool SetMasterIfNotExists(Guid taskProcessorId)
        {
            this.RecordMethodCall(taskProcessorId);

            if (this.masterTaskProcessorId.HasValue && (DateTime.Now < this.masterExpiresAt))
            {
                return false;
            }

            this.masterTaskProcessorId = taskProcessorId;

            this.masterExpiresAt = DateTime.Now + this.Expiration;

            return true;
        }

        public void ClearMaster()
        {
            this.RecordMethodCall();

            this.masterTaskProcessorId = null;
        }

        public bool MasterHeartbeat()
        {
            this.RecordMethodCall();

            this.ExecutePredefinedMethod();

            if (this.HasPredefinedResult<bool>())
            {
                return this.GetPredefinedResult<bool>();
            }

            if (this.masterExpiresAt < DateTime.Now)
            {
                return false;
            }

            this.masterExpiresAt = DateTime.Now + this.Expiration;

            return true;
        }

        #endregion ITaskProcessorRuntimeInfoRepository Members
    }
}