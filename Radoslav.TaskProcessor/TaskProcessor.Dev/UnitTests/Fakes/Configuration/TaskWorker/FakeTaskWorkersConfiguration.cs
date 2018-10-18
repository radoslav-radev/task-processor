using System;
using System.Collections.Generic;
using Radoslav.TaskProcessor.Configuration;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakeTaskWorkersConfiguration : MockObject, ITaskWorkersConfiguration
    {
        private readonly Dictionary<Type, FakeTaskWorkerConfiguration> configs = new Dictionary<Type, FakeTaskWorkerConfiguration>();

        #region ITaskWorkerConfiguration Members

        public ITaskWorkerConfiguration this[Type taskType]
        {
            get
            {
                FakeTaskWorkerConfiguration result;

                this.configs.TryGetValue(taskType, out result);

                return result;
            }
        }

        #endregion ITaskWorkerConfiguration Members

        internal void Add<TTask>(FakeTaskWorkerConfiguration config)
            where TTask : ITask
        {
            this.configs.Add(typeof(TTask), config);
        }

        internal FakeTaskWorkerConfiguration Get<TTask>()
            where TTask : ITask
        {
            return (FakeTaskWorkerConfiguration)this[typeof(TTask)];
        }
    }
}