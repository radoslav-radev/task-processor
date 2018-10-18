using System;

namespace Radoslav.TaskProcessor.Configuration
{
    /// <summary>
    /// Basic functionality of a polling job configuration.
    /// </summary>
    public interface IPollingJobConfiguration : IPollingConfiguration
    {
        /// <summary>
        /// Gets the type of the polling job implementing <see cref="Radoslav.TaskProcessor.Model.IPollingJob"/>.
        /// </summary>
        /// <value>The type implementing the polling job.</value>
        Type ImplementationType { get; }
    }
}