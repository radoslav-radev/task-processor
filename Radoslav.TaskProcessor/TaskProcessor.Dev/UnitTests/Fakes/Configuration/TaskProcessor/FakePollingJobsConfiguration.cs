using System;
using System.Collections.Generic;
using System.Linq;
using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakePollingJobsConfiguration : IPollingJobsConfiguration
    {
        private readonly List<FakePollingJobConfiguration> jobs = new List<FakePollingJobConfiguration>();

        #region IPollingJobsConfiguration Members

        public IPollingJobConfiguration this[Type implementationType]
        {
            get
            {
                return this.jobs.FirstOrDefault(q => q.ImplementationType == implementationType);
            }
        }

        public IPollingJobConfiguration Add(Type implementationType)
        {
            FakePollingJobConfiguration result = new FakePollingJobConfiguration()
            {
                ImplementationType = implementationType
            };

            this.jobs.Add(result);

            return result;
        }

        public void AddCopy(IPollingJobConfiguration source)
        {
            this.jobs.Add(new FakePollingJobConfiguration(source));
        }

        public void Remove(Type implementationType)
        {
            this.jobs.Remove(q => q.ImplementationType == implementationType);
        }

        #endregion IPollingJobsConfiguration Members

        #region IEnumerable<IPollingJobConfiguration> Members

        public IEnumerator<IPollingJobConfiguration> GetEnumerator()
        {
            return this.jobs.GetEnumerator();
        }

        #endregion IEnumerable<IPollingJobConfiguration> Members

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.jobs.GetEnumerator();
        }

        #endregion IEnumerable Members

        internal void Add(FakePollingJobConfiguration jobConfig)
        {
            this.jobs.Add(jobConfig);
        }
    }
}