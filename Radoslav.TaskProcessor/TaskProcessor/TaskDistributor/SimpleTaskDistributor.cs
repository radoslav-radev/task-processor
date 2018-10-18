using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Radoslav.TaskProcessor.Model;
using Radoslav.TaskProcessor.Repository;

namespace Radoslav.TaskProcessor.TaskDistributor
{
    /// <summary>
    /// Very simple (minimal) implementation of <see cref="ITaskDistributor"/>.
    /// </summary>
    public sealed class SimpleTaskDistributor : ITaskDistributor
    {
        private readonly ITaskProcessorRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTaskDistributor"/> class.
        /// </summary>
        /// <param name="repository">The repository to be used when the task distributor has to retrieve some data from storage.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="repository"/> is null.</exception>
        public SimpleTaskDistributor(ITaskProcessorRepository repository)
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

            return this.repository.TaskProcessorRuntimeInfo
                .GetAll()
                .OrderBy(t => activeTasksInfo.Count(tt => tt.TaskProcessorId == t.TaskProcessorId));
        }

        /// <inheritdoc />
        public IEnumerable<ITaskRuntimeInfo> ChooseNextTasksForProcessor(Guid taskProcessorId)
        {
            Trace.WriteLine("ENTER: Choosing next tasks to assign to processor '{0}' ...".FormatInvariant(taskProcessorId));

            return this.repository.TaskRuntimeInfo.GetPending(false)
                .OrderByDescending(task => (int)task.Priority);
        }

        #endregion ITaskDistributor Members
    }
}