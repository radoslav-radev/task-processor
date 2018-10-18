using System;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.Repository;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal static class TaskProcessorUnitTestExtensions
    {
        internal static ITaskRuntimeInfo Create(this ITaskRuntimeInfoRepository repository)
        {
            return repository.Create(Guid.NewGuid(), typeof(FakeTask), DateTime.UtcNow, TaskPriority.Normal, null);
        }

        internal static TTaskRuntimeInfo Create<TTaskRuntimeInfo>(this ITaskRuntimeInfoRepository repository)
            where TTaskRuntimeInfo : ITaskRuntimeInfo
        {
            return (TTaskRuntimeInfo)repository.Create(Guid.NewGuid(), typeof(FakeTask), DateTime.UtcNow, TaskPriority.Normal, null);
        }

        internal static ITaskRuntimeInfo Add(this ITaskRuntimeInfoRepository repository)
        {
            ITaskRuntimeInfo result = repository.Create();

            repository.Add(result);

            return result;
        }

        internal static TTaskRuntimeInfo Add<TTaskRuntimeInfo>(this ITaskRuntimeInfoRepository repository)
            where TTaskRuntimeInfo : ITaskRuntimeInfo
        {
            TTaskRuntimeInfo result = repository.Create<TTaskRuntimeInfo>();

            repository.Add(result);

            return result;
        }

        internal static ITaskRuntimeInfo Add(this ITaskRuntimeInfoRepository repository, TaskPriority priority)
        {
            ITaskRuntimeInfo result = repository.Create(Guid.NewGuid(), typeof(FakeTask), DateTime.UtcNow, priority, null);

            repository.Add(result);

            return result;
        }

        internal static ITaskRuntimeInfo Add(this ITaskRuntimeInfoRepository repository, string pollingQueue)
        {
            ITaskRuntimeInfo result = repository.Create(Guid.NewGuid(), typeof(FakeTask), DateTime.UtcNow, TaskPriority.Normal, pollingQueue);

            repository.Add(result);

            return result;
        }
    }
}