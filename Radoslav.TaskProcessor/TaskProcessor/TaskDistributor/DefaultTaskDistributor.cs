using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Radoslav.TaskProcessor.Configuration;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.Repository;

namespace Radoslav.TaskProcessor.TaskDistributor
{
    /// <summary>
    /// Default implementation of <see cref="ITaskDistributor"/>.
    /// </summary>
    public sealed class DefaultTaskDistributor : ITaskDistributor
    {
        private readonly ITaskProcessorRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTaskDistributor"/> class.
        /// </summary>
        /// <param name="repository">The repository to be used when the task distributor has to retrieve some data from storage.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="repository"/> is null.</exception>
        public DefaultTaskDistributor(ITaskProcessorRepository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            this.repository = repository;
        }

        /// <summary>
        /// Gets the repository to be used when the task distributor has to retrieve some data from storage.
        /// </summary>
        /// <value>The repository to be used when the task distributor has to retrieve some data from storage.</value>
        public ITaskProcessorRepository Repository
        {
            get { return this.repository; }
        }

        #region ITaskDistributor Members

        /// <inheritdoc />
        public IEnumerable<ITaskProcessorRuntimeInfo> ChooseProcessorForTask(ITaskRuntimeInfo pendingTaskInfo)
        {
            if (pendingTaskInfo == null)
            {
                throw new ArgumentNullException("pendingTaskInfo");
            }

            ITaskRuntimeInfo[] activeTasksInfo = this.repository.TaskRuntimeInfo.GetActive().ToArray();

            return this.repository.TaskProcessorRuntimeInfo.GetAll()
                .Where(processorInfo => DefaultTaskDistributor.CanExecuteTask(pendingTaskInfo, processorInfo, activeTasksInfo))
                .OrderBy(t => activeTasksInfo.Count(tt => tt.TaskProcessorId == t.TaskProcessorId));
        }

        /// <inheritdoc />
        public IEnumerable<ITaskRuntimeInfo> ChooseNextTasksForProcessor(Guid taskProcessorId)
        {
            Trace.WriteLine("ENTER: Choosing next tasks to assign to processor '{0}' ...".FormatInvariant(taskProcessorId));

            ITaskProcessorRuntimeInfo taskProcessor = this.repository.TaskProcessorRuntimeInfo.GetById(taskProcessorId);

            if (taskProcessor == null)
            {
                Trace.WriteLine("EXIT: Processor '{0}' not found in configuration.".FormatInvariant(taskProcessorId));

                yield break;
            }

            var pendingAndActiveTasks = this.repository.TaskRuntimeInfo.GetPendingAndActive();

            IEnumerable<ITaskRuntimeInfo> pendingTasksInfo = pendingAndActiveTasks[TaskStatus.Pending];
            IEnumerable<ITaskRuntimeInfo> activeTasksInfo = pendingAndActiveTasks[TaskStatus.InProgress];

            int? remainingTasksCount = null;

            if (taskProcessor.Configuration.Tasks.MaxWorkers.HasValue)
            {
                remainingTasksCount = taskProcessor.Configuration.Tasks.MaxWorkers.Value - activeTasksInfo.Count(t => t.TaskProcessorId == taskProcessor.TaskProcessorId);

                if (remainingTasksCount <= 0)
                {
                    Trace.WriteLine("EXIT: No tasks to assign because processor max workers threshold {0} is reached.".FormatInvariant(taskProcessor.Configuration.Tasks.MaxWorkers.Value));

                    yield break;
                }
            }

            Dictionary<Type, int> remainingTasksCountByTaskType = new Dictionary<Type, int>();

            Func<ITaskRuntimeInfo, bool> filterPredicate = pendingTaskInfo =>
            {
                Lazy<int> activeTasksCountByTaskType = new Lazy<int>(() => activeTasksInfo.Count(t => t.TaskType == pendingTaskInfo.TaskType));

                Lazy<int?> remainingTasksByTaskTypeValue = new Lazy<int?>(() => null);

                ITaskJobConfiguration processorTaskJobConfig = taskProcessor.Configuration.Tasks[pendingTaskInfo.TaskType];

                Lazy<int> activeProcessorTasksCountByTaskType = new Lazy<int>(() => activeTasksInfo.Count(t => (t.TaskType == pendingTaskInfo.TaskType) && (t.TaskProcessorId == taskProcessor.TaskProcessorId)));

                Lazy<int?> remainingProcessorTasksCountByTaskType = new Lazy<int?>(() => null);

                if ((processorTaskJobConfig != null) && processorTaskJobConfig.MaxWorkers.HasValue)
                {
                    if (activeProcessorTasksCountByTaskType.Value >= processorTaskJobConfig.MaxWorkers.Value)
                    {
                        return false;
                    }

                    remainingProcessorTasksCountByTaskType = new Lazy<int?>(() => processorTaskJobConfig.MaxWorkers.Value - activeProcessorTasksCountByTaskType.Value);
                }

                if (!remainingTasksCountByTaskType.ContainsKey(pendingTaskInfo.TaskType))
                {
                    int? value = DefaultTaskDistributor.GetMinMaxWorkers(
                        remainingTasksByTaskTypeValue.Value,
                        remainingProcessorTasksCountByTaskType.Value);

                    if (value.HasValue)
                    {
                        remainingTasksCountByTaskType.Add(pendingTaskInfo.TaskType, value.Value);
                    }
                }

                return true;
            };

            foreach (ITaskRuntimeInfo result in pendingTasksInfo
                .OrderByDescending(t => (int)t.Priority)
                .Where(filterPredicate))
            {
                int count;

                if (remainingTasksCountByTaskType.TryGetValue(result.TaskType, out count))
                {
                    if (count == 0)
                    {
                        continue;
                    }

                    remainingTasksCountByTaskType[result.TaskType] = count - 1;
                }

                yield return result;

                if (remainingTasksCount == 1)
                {
                    yield break;
                }

                remainingTasksCount++;
            }
        }

        #endregion ITaskDistributor Members

        private static bool CanExecuteTask(ITaskRuntimeInfo pendingTaskInfo, ITaskProcessorRuntimeInfo processorInfo, IEnumerable<ITaskRuntimeInfo> activeTaskInfo)
        {
            Trace.WriteLine("ENTER: Checking if task processor '{0}' can execute task '{1}' ...".FormatInvariant(processorInfo.TaskProcessorId, pendingTaskInfo.TaskId));

            if (processorInfo.Configuration.Tasks.MaxWorkers.HasValue &&
                activeTaskInfo.AtLeast(processorInfo.Configuration.Tasks.MaxWorkers.Value, t => t.TaskProcessorId == processorInfo.TaskProcessorId))
            {
                Trace.WriteLine("EXIT: Task processor '{0}' cannot execute task '{1}' because max workers threshold {2} is reached.".FormatInvariant(processorInfo.TaskProcessorId, pendingTaskInfo.TaskId, processorInfo.Configuration.Tasks.MaxWorkers.Value));

                return false;
            }

            ITaskJobConfiguration taskJobConfig = processorInfo.Configuration.Tasks[pendingTaskInfo.TaskType];

            if ((taskJobConfig != null) && taskJobConfig.MaxWorkers.HasValue &&
                activeTaskInfo.AtLeast(taskJobConfig.MaxWorkers.Value, tt => (tt.TaskType == taskJobConfig.TaskType) && (tt.TaskProcessorId == processorInfo.TaskProcessorId)))
            {
                Trace.WriteLine("Task processor '{0}' cannot execute task '{1}' because max workers threshold {2} for task type '{3}' is reached.".FormatInvariant(processorInfo.TaskProcessorId, pendingTaskInfo.TaskId, taskJobConfig.MaxWorkers.Value, pendingTaskInfo.TaskType.Name));

                return false;
            }

            Trace.WriteLine("EXIT: Task processor '{0}' can execute task '{1}'.".FormatInvariant(processorInfo.TaskProcessorId, pendingTaskInfo.TaskId));

            return true;
        }

        private static int? GetMinMaxWorkers(int? value1, int? value2)
        {
            if (value1.HasValue)
            {
                if (value2.HasValue)
                {
                    return Math.Min(value1.Value, value2.Value);
                }
                else
                {
                    return value1;
                }
            }
            else
            {
                if (value2.HasValue)
                {
                    return value2;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}