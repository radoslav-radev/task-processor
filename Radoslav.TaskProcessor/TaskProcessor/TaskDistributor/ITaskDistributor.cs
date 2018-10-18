using System;
using System.Collections.Generic;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.TaskDistributor
{
    /// <summary>
    /// Basic functionality of a task distributor.
    /// </summary>
    /// <remarks>
    /// This class is responsible for choosing which task processor should execute a given task, and which tasks should be executed by a given task processor.
    /// The selection algorithm should consider tasks tenant and priority.
    /// </remarks>
    public interface ITaskDistributor
    {
        /// <summary>
        /// Chooses the best processors to execute a pending task.
        /// </summary>
        /// <param name="pendingTaskInfo">The runtime information for the pending task.</param>
        /// <returns>The best task processors to execute the task in a corresponding order. The first is best suited, the last - least.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="pendingTaskInfo"/>  is null.</exception>
        IEnumerable<ITaskProcessorRuntimeInfo> ChooseProcessorForTask(ITaskRuntimeInfo pendingTaskInfo);

        /// <summary>
        /// Choose which tasks should be executed next by a task processor, in a corresponding order.
        /// </summary>
        /// <param name="taskProcessorId">The ID of the task processor for which to choose tasks.</param>
        /// <returns>The tasks that should be executed by the task processor, in the corresponding order.
        /// The first task is most urgent to execute, the last - least.</returns>
        IEnumerable<ITaskRuntimeInfo> ChooseNextTasksForProcessor(Guid taskProcessorId);
    }
}