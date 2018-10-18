using System;
using System.ComponentModel;
using System.Configuration;
using Radoslav.Configuration.Validators;

namespace Radoslav.TaskProcessor.Configuration.AppConfig
{
    /// <summary>
    /// An implementation of <see cref="ITaskJobConfiguration"/> that reads from App.config file.
    /// </summary>
    internal sealed class TaskJobConfigurationElement : ConfigurationElement, ITaskJobConfiguration
    {
        #region ITaskJobConfiguration Members

        /// <inheritdoc />
        [ConfigurationProperty("type", IsRequired = true, IsKey = true)]
        [TypeConverter(typeof(TypeNameConverter))]
        public Type TaskType
        {
            get
            {
                return (Type)this["type"];
            }
        }

        /// <inheritdoc />
        [ConfigurationProperty("max")]
        [NullableIntegerValidator(MinValue = 0)]
        public int? MaxWorkers
        {
            get
            {
                return (int?)this["max"];
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion ITaskJobConfiguration Members
    }
}