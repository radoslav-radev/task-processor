using System;
using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor.Model
{
    /// <summary>
    /// Basic functionality of a task processor runtime information.
    /// </summary>
    /// <remarks>These objects are used by the task processors during runtime and are stored in Redis or other cache.</remarks>
    public interface ITaskProcessorRuntimeInfo
    {
        /// <summary>
        /// Gets the task processor unique ID.
        /// </summary>
        /// <value>The task processor unique ID.</value>
        Guid TaskProcessorId { get; }

        /// <summary>
        /// Gets the name of the computer where the task processor is running.
        /// </summary>
        /// <value>The name of the computer where the task processor is running.</value>
        string MachineName { get; }

        /// <summary>
        /// Gets the task processor configuration.
        /// </summary>
        /// <value>The task processor configuration.</value>
        ITaskProcessorConfiguration Configuration { get; }
    }
}