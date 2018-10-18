using System;
using Radoslav.TaskProcessor.Facade;

namespace Radoslav.TaskProcessor.Model
{
    /// <summary>
    /// Class for task processor model extensions.
    /// </summary>
    public static class TaskProcessorModelExtensions
    {
        /// <summary>
        /// Gets task job settings for task type.
        /// </summary>
        /// <typeparam name="TTask">The type of the task.</typeparam>
        /// <param name="facade">The <see cref="ITaskProcessorFacade"/> instance to extend.</param>
        /// <returns>Task job settings for the specified task type, or null if not found.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="facade"/> is null.</exception>
        public static ITaskJobSettings GetTaskJobSettings<TTask>(this ITaskProcessorFacade facade)
            where TTask : ITask
        {
            if (facade == null)
            {
                throw new ArgumentNullException(nameof(facade));
            }

            return facade.GetTaskJobSettings(typeof(TTask));
        }

        /// <summary>
        /// Gets task job settings for task type.
        /// </summary>
        /// <typeparam name="TTask">The type of the task.</typeparam>
        /// <typeparam name="TTaskJobSettings">The type of the expected task job settings.</typeparam>
        /// <param name="facade">The <see cref="ITaskProcessorFacade"/> instance to extend.</param>
        /// <returns>Task job settings for the specified task type, or null if not found.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="facade"/> is null.</exception>
        /// <exception cref="InvalidCastException">The task job settings returned from storage are not <typeparamref name="TTaskJobSettings"/>.</exception>
        public static TTaskJobSettings GetTaskJobSettings<TTask, TTaskJobSettings>(this ITaskProcessorFacade facade)
            where TTask : ITask
            where TTaskJobSettings : ITaskJobSettings
        {
            if (facade == null)
            {
                throw new ArgumentNullException(nameof(facade));
            }

            return (TTaskJobSettings)facade.GetTaskJobSettings(typeof(TTask));
        }

        /// <summary>
        /// Sets task job settings for task type.
        /// </summary>
        /// <param name="facade">The <see cref="ITaskProcessorFacade"/> instance to extend.</param>
        /// <param name="settings">The settings to set for the specified task type.</param>
        /// <typeparam name="TTask">The type of the task.</typeparam>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="facade"/> or <paramref name="settings" /> is null.</exception>
        public static void SetTaskJobSettings<TTask>(this ITaskProcessorFacade facade, ITaskJobSettings settings)
            where TTask : ITask
        {
            if (facade == null)
            {
                throw new ArgumentNullException(nameof(facade));
            }

            facade.SetTaskJobSettings(typeof(TTask), settings);
        }

        /// <summary>
        /// Clears the task job settings for task type.
        /// </summary>
        /// <typeparam name="TTask">The type of the task.</typeparam>
        /// <param name="facade">The <see cref="ITaskProcessorFacade"/> instance to extend.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="facade"/> is null.</exception>
        public static void ClearTaskJobSettings<TTask>(this ITaskProcessorFacade facade)
            where TTask : ITask
        {
            if (facade == null)
            {
                throw new ArgumentNullException(nameof(facade));
            }

            facade.ClearTaskJobSettings(typeof(TTask));
        }
    }
}