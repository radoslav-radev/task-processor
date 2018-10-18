using System;
using System.Collections.Generic;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.Repository;

namespace Radoslav.TaskProcessor.UnitTests
{
    public sealed class FakeTaskJobSettingsRepository : MockObject, ITaskJobSettingsRepository
    {
        private readonly Dictionary<Type, ITaskJobSettings> jobSettings = new Dictionary<Type, ITaskJobSettings>();

        #region ITaskJobSettingsRepository Members

        public ITaskJobSettings Get(Type taskType)
        {
            this.RecordMethodCall(taskType);

            ITaskJobSettings result;

            this.jobSettings.TryGetValue(taskType, out result);

            return result;
        }

        public void Set(Type taskType, ITaskJobSettings settings)
        {
            this.RecordMethodCall(taskType, settings);

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            this.jobSettings.Add(taskType, settings);
        }

        public void Clear(Type taskType)
        {
            this.RecordMethodCall(taskType);

            this.jobSettings.Clear();
        }

        #endregion ITaskJobSettingsRepository Members
    }
}