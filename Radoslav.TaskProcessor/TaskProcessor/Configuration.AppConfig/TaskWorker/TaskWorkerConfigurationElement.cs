using System;
using System.ComponentModel;
using System.Configuration;
using System.Xml;
using Radoslav.Configuration.Validators;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.TaskWorker;

namespace Radoslav.TaskProcessor.Configuration.AppConfig
{
    /// <summary>
    /// An implementation of <see cref="ITaskWorkerConfiguration"/> that reads from App.config file.
    /// </summary>
    internal sealed class TaskWorkerConfigurationElement : ConfigurationElement, ITaskWorkerConfiguration
    {
        #region ITaskWorkerConfigurationElement Members

        /// <inheritdoc />
        [ConfigurationProperty("task", IsRequired = true, IsKey = true)]
        [TypeConverter(typeof(TypeNameConverter))]
        [AssignableFromTypeValidator(typeof(ITask))]
        public Type TaskType
        {
            get
            {
                return (Type)this["task"];
            }
        }

        /// <inheritdoc />
        [ConfigurationProperty("worker", IsRequired = true)]
        [TypeConverter(typeof(TypeNameConverter))]
        public Type WorkerType
        {
            get
            {
                return (Type)this["worker"];
            }
        }

        /// <inheritdoc />
        [ConfigurationProperty("hasTaskJobSettings")]
        public bool HasTaskJobSettings
        {
            get
            {
                return (bool)this["hasTaskJobSettings"];
            }
        }

        #endregion ITaskWorkerConfigurationElement Members

        /// <inheritdoc />
        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            base.DeserializeElement(reader, serializeCollectionKey);

            CompositeValidator validator = new CompositeValidator(
                new ParameterlessConstructorValidator(),
                new PublicConstructorValidator(),
                new AssignableFromTypeValidator(typeof(ITaskWorker)));

            validator.Validate(this.WorkerType);
        }
    }
}