using System;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Repository
{
    /// <summary>
    /// Class for repository extensions.
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Checks if a task runtime information can be added to repository.
        /// </summary>
        /// <param name="taskInfo">The task runtime information to add.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="taskInfo"/> is null.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="taskInfo"/>
        /// <see cref="ITaskRuntimeInfo.Status"/> is not Pending, or
        /// <see cref="ITaskRuntimeInfo.TaskProcessorId"/> is not null, or
        /// <see cref="ITaskRuntimeInfo.StartedUtc"/> is not null, or
        /// <see cref="ITaskRuntimeInfo.CanceledUtc"/> is not null, or
        /// <see cref="ITaskRuntimeInfo.CompletedUtc"/> is not null, or
        /// <see cref="ITaskRuntimeInfo.Percentage"/> is not 0, or
        /// <see cref="ITaskRuntimeInfo.Error"/> is not null or emprty.</exception>
        public static void ValidateForAdd(this ITaskRuntimeInfo taskInfo)
        {
            if (taskInfo == null)
            {
                throw new ArgumentNullException("taskInfo");
            }

            if (taskInfo.Status != TaskStatus.Pending)
            {
                throw new ArgumentOutOfRangeException("taskInfo", taskInfo.Status, "Task status must be {0}.".FormatInvariant(TaskStatus.Pending));
            }

            if (taskInfo.TaskProcessorId.HasValue)
            {
                throw new ArgumentException("TaskProcessorId must be null.", "taskInfo");
            }

            if (taskInfo.Percentage != 0)
            {
                throw new ArgumentException("Percentage must be 0.", "taskInfo");
            }

            if (taskInfo.StartedUtc.HasValue)
            {
                throw new ArgumentException("StartedUtc must be null.", "taskInfo");
            }

            if (taskInfo.CanceledUtc.HasValue)
            {
                throw new ArgumentException("CanceledUtc must be null.", "taskInfo");
            }

            if (taskInfo.CompletedUtc.HasValue)
            {
                throw new ArgumentException("CompletedUtc must be null.", "taskInfo");
            }

            if (!string.IsNullOrEmpty(taskInfo.Error))
            {
                throw new ArgumentException("Error must be null", "taskInfo");
            }
        }

        /// <summary>
        /// Validates if a <see cref="Type"/> is a valid polling job implementation type that can be used in task processor configuration.
        /// </summary>
        /// <param name="implementationType">The type to validate.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="implementationType"/> is null.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="implementationType"/> does not implement <see cref="IPollingJob"/>.</exception>
        public static void ValidatePollingJobImplementationType(Type implementationType)
        {
            if (implementationType == null)
            {
                throw new ArgumentNullException("implementationType");
            }

            if ((implementationType == typeof(IPollingJob)) || !typeof(IPollingJob).IsAssignableFrom(implementationType))
            {
                throw new ArgumentException("Type '{0}' is not a descendant of '{1}'.".FormatInvariant(implementationType, typeof(IPollingJob)), "implementationType");
            }
        }

        /// <summary>
        /// Gets task job settings for task type.
        /// </summary>
        /// <typeparam name="TTask">The type of the task.</typeparam>
        /// <param name="repository">The repository to extend.</param>
        /// <returns>Task job settings for the specified task type, or null if not found.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="repository"/> is null.</exception>
        public static ITaskJobSettings Get<TTask>(this ITaskJobSettingsRepository repository)
            where TTask : ITask
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            return repository.Get(typeof(TTask));
        }

        /// <summary>
        /// Gets task job settings for task type.
        /// </summary>
        /// <typeparam name="TTask">The type of the task.</typeparam>
        /// <typeparam name="TTaskJobSettings">The type of the expected task job settings.</typeparam>
        /// <param name="repository">The repository to extend.</param>
        /// <returns>Task job settings for the specified task type, or null if not found.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="repository"/> is null.</exception>
        /// <exception cref="InvalidCastException">The task job settings returned from storage are not <typeparamref name="TTaskJobSettings"/>.</exception>
        public static TTaskJobSettings Get<TTask, TTaskJobSettings>(this ITaskJobSettingsRepository repository)
            where TTask : ITask
            where TTaskJobSettings : ITaskJobSettings
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            return (TTaskJobSettings)repository.Get(typeof(TTask));
        }

        /// <summary>
        /// Sets task job settings for task type.
        /// </summary>
        /// <typeparam name="TTask">The type of the task.</typeparam>
        /// <param name="repository">The repository to extend.</param>
        /// <param name="settings">The settings to set for the specified task type.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="repository"/> or <paramref name="settings" /> is null.</exception>
        public static void Set<TTask>(this ITaskJobSettingsRepository repository, ITaskJobSettings settings)
            where TTask : ITask
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            repository.Set(typeof(TTask), settings);
        }

        /// <summary>
        /// Clears the task job settings for task type.
        /// </summary>
        /// <typeparam name="TTask">The type of the task.</typeparam>
        /// <param name="repository">The repository to extend.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="repository"/> is null.</exception>
        public static void Clear<TTask>(this ITaskJobSettingsRepository repository)
            where TTask : ITask
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            repository.Clear(typeof(TTask));
        }
    }
}