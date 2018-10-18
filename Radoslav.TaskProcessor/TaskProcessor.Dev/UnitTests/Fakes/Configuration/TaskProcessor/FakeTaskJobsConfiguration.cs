using System;
using System.Collections;
using System.Collections.Generic;
using Radoslav.TaskProcessor.Configuration;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakeTaskJobsConfiguration : ITaskJobsConfiguration
    {
        private readonly Dictionary<Type, FakeTaskJobConfiguration> taskJobs = new Dictionary<Type, FakeTaskJobConfiguration>();

        private int? maxWorkers;

        #region ITaskJobsConfiguration Members

        public int? MaxWorkers
        {
            get
            {
                return this.maxWorkers;
            }

            set
            {
                if (value.HasValue && value.Value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Value must not be negative.");
                }

                this.maxWorkers = value;
            }
        }

        public ITaskJobConfiguration this[Type taskType]
        {
            get
            {
                if (!typeof(ITask).IsAssignableFrom(taskType))
                {
                    throw new ArgumentException("Type '{0}' does not implement '{1}'.".FormatInvariant(taskType, typeof(ITask)), "taskType");
                }

                FakeTaskJobConfiguration result;

                this.taskJobs.TryGetValue(taskType, out result);

                return result;
            }
        }

        public ITaskJobConfiguration Add(Type taskType)
        {
            if (taskType == null)
            {
                throw new ArgumentNullException("taskType");
            }

            if (!typeof(ITask).IsAssignableFrom(taskType))
            {
                throw new ArgumentException("Type '{0}' does not implement '{1}'.".FormatInvariant(taskType, typeof(ITask)), "taskType");
            }

            FakeTaskJobConfiguration result = new FakeTaskJobConfiguration()
            {
                TaskType = taskType
            };

            this.taskJobs.Add(taskType, result);

            return result;
        }

        public void AddCopy(ITaskJobConfiguration taskJob)
        {
            if (taskJob == null)
            {
                throw new ArgumentNullException("taskJob");
            }

            if (this.taskJobs.ContainsKey(taskJob.TaskType))
            {
                throw new ArgumentException("Task job configuration for type '{0}' already exists.".FormatInvariant(taskJob.TaskType), "taskJob");
            }

            this.taskJobs.Add(taskJob.TaskType, (FakeTaskJobConfiguration)taskJob);
        }

        public void Remove(Type taskType)
        {
            if (taskType == null)
            {
                throw new ArgumentNullException("taskType");
            }

            this.taskJobs.Remove(taskType);
        }

        #endregion ITaskJobsConfiguration Members

        #region IEnumerable<ITaskJobConfiguration> Members

        public IEnumerator<ITaskJobConfiguration> GetEnumerator()
        {
            return this.taskJobs.Values.GetEnumerator();
        }

        #endregion IEnumerable<ITaskJobConfiguration> Members

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.taskJobs.Values.GetEnumerator();
        }

        #endregion IEnumerable Members
    }
}