using System;
using Radoslav.ServiceLocator;
using Radoslav.TaskProcessor.Configuration;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.TaskWorker
{
    /// <summary>
    /// An implementation of the <see cref="ITaskWorkerFactory"/> that uses configuration to create task workers.
    /// </summary>
    public class TaskWorkerFactory : ITaskWorkerFactory
    {
        private readonly ITaskWorkersConfiguration configuration;
        private readonly IRadoslavServiceLocator locator;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskWorkerFactory"/> class.
        /// </summary>
        /// <param name="configurationProvider">The configuration factory to provide task worker configuration.</param>
        /// <param name="locator">The service locator to resolve task worker instances.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="configurationProvider"/> or <paramref name="locator"/> is null.</exception>
        public TaskWorkerFactory(ITaskProcessorConfigurationProvider configurationProvider, IRadoslavServiceLocator locator)
        {
            if (configurationProvider == null)
            {
                throw new ArgumentNullException(nameof(configurationProvider));
            }

            if (locator == null)
            {
                throw new ArgumentNullException(nameof(locator));
            }

            this.locator = locator;

            this.configuration = configurationProvider.GetTaskWorkerConfiguration();
        }

        #endregion Constructor

        #region ITaskWorkerFactory Members

        /// <inheritdoc />
        public virtual ITaskWorker CreateTaskWorker(ITask task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            ITaskWorkerConfiguration config = this.configuration[task.GetType()];

            if (config == null)
            {
                throw new NotSupportedException<Type>(task.GetType());
            }

            if (this.locator.CanResolve(config.WorkerType))
            {
                return this.locator.ResolveSingle<ITaskWorker>(config.WorkerType);
            }
            else
            {
                return config.WorkerType.CreateInstance<ITaskWorker>();
            }
        }

        #endregion ITaskWorkerFactory Members
    }
}