using System;
using System.ComponentModel;
using System.Configuration;
using Radoslav.Configuration.Validators;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Configuration.AppConfig
{
    /// <summary>
    /// A client configuration of a task in the App.config file.
    /// </summary>
    internal sealed class ClientTaskConfiguration : ConfigurationElement
    {
        /// <summary>
        /// Gets the type of the task.
        /// </summary>
        /// <value>The type of the task.</value>
        [ConfigurationProperty("type", IsRequired = true, IsKey = true)]
        [TypeConverter(typeof(TypeNameConverter))]
        [AssignableFromTypeValidator(typeof(ITask))]
        internal Type TaskType
        {
            get
            {
                return (Type)this["type"];
            }
        }
    }
}