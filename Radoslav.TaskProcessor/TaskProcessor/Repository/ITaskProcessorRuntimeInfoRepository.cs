using System;
using System.Collections.Generic;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Repository
{
    /// <summary>
    /// Basic functionality of <see cref="ITaskProcessorRuntimeInfo"/> repository.
    /// </summary>
    public interface ITaskProcessorRuntimeInfoRepository
    {
        /// <summary>
        /// Gets or sets the time after which the task processor runtime information in the storage expire.
        /// </summary>
        /// <value>The time after which the task processor runtime information in the storage expire.</value>
        /// <exception cref="ArgumentOutOfRangeException">Value is less or equal to <see cref="TimeSpan.Zero"/>.</exception>
        TimeSpan Expiration { get; set; }

        /// <summary>
        /// Gets all task processors runtime information from the storage.
        /// </summary>
        /// <returns>All task processors runtime information in storage.</returns>
        IEnumerable<ITaskProcessorRuntimeInfo> GetAll();

        /// <summary>
        /// Gets task processor runtime information from the storage by its task processor ID.
        /// </summary>
        /// <param name="taskProcessorId">The ID of the task processor.</param>
        /// <returns>A task processor runtime information with the specified ID, or null if not found.</returns>
        ITaskProcessorRuntimeInfo GetById(Guid taskProcessorId);

        /// <summary>
        /// Creates a new task processor runtime information instance.
        /// </summary>
        /// <param name="taskProcessorId">The ID of the task processor for which is the information.</param>
        /// <param name="machineName">The name of the computer where the task processor is running.</param>
        /// <returns>A new task processor runtime information instance configured with the provided parameters.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="machineName" /> is null or empty string.</exception>
        ITaskProcessorRuntimeInfo Create(Guid taskProcessorId, string machineName);

        /// <summary>
        /// Add task processor runtime information to the storage.
        /// </summary>
        /// <param name="taskProcessorInfo">The task processor runtime information to add.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="taskProcessorInfo"/> is null.</exception>
        void Add(ITaskProcessorRuntimeInfo taskProcessorInfo);

        /// <summary>
        /// Updates the runtime information for a task processor.
        /// </summary>
        /// <param name="taskProcessorInfo">The runtime information for the task processor to update.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="taskProcessorInfo"/> is null.</exception>
        void Update(ITaskProcessorRuntimeInfo taskProcessorInfo);

        /// <summary>
        /// Remove task processor runtime information from storage.
        /// </summary>
        /// <param name="taskProcessorId">The ID of the task processor whose runtime information to remove.</param>
        void Delete(Guid taskProcessorId);

        /// <summary>
        /// Indicates to storage that a task processor is still alive.
        /// </summary>
        /// <remarks>If this method has not been called by a task processor for <see cref="Expiration"/> time,
        /// its runtime information will be removed automatically from repository.</remarks>
        /// <param name="taskProcessorId">The ID of the task processor that is heart-beating.</param>
        /// <returns>True if heartbeat was successful; false if the task processor runtime information for
        /// the specified task processor is not found or has already expired.</returns>
        bool Heartbeat(Guid taskProcessorId);

        /// <summary>
        /// Gets the current master task processor ID from storage.
        /// </summary>
        /// <returns>The current master task processor ID.</returns>
        Guid? GetMasterId();

        /// <summary>
        /// Sets the current master task processor ID in storage even if there is current value.
        /// </summary>
        /// <param name="taskProcessorId">The ID of the new master task processor.</param>
        void SetMaster(Guid taskProcessorId);

        /// <summary>
        /// Sets the current master task processor ID in storage, but only if there is no current value.
        /// </summary>
        /// <param name="taskProcessorId">The ID of the new master task processor.</param>
        /// <returns>True if there is no current master and value was set in storage; false if there is already a current master and the value was not set.</returns>
        bool SetMasterIfNotExists(Guid taskProcessorId);

        /// <summary>
        /// Clears the master task processor ID in storage.
        /// </summary>
        void ClearMaster();

        /// <summary>
        /// Indicates that the master task processor is still alive.
        /// </summary>
        /// <returns>True if heart-beating was successful; false if the master record in storage has already expired.</returns>
        bool MasterHeartbeat();
    }
}