using System;
using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakeTaskJobConfiguration : ITaskJobConfiguration
    {
        private int? maxWorkers;

        #region ITaskJobConfiguration Members

        public Type TaskType { get; internal set; }

        #endregion ITaskJobConfiguration Members

        #region IMaxWorkersConfiguration

        public int? MaxWorkers
        {
            get
            {
                return this.maxWorkers;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Value must not be negative.");
                }

                this.maxWorkers = value;
            }
        }

        #endregion IMaxWorkersConfiguration
    }
}