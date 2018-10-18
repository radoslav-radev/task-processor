using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Radoslav.TaskProcessor.MessageBus
{
    internal partial class CompositeTaskMessageBusSender : Collection<ITaskMessageBusSender>, ITaskMessageBusSender
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeTaskMessageBusSender"/> class.
        /// </summary>
        internal CompositeTaskMessageBusSender()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeTaskMessageBusSender"/> class.
        /// </summary>
        /// <param name="items">Collection of <see cref="ITaskMessageBusSender"/> elements with each to compose the <see cref="CompositeTaskMessageBusSender"/> instance.</param>
        internal CompositeTaskMessageBusSender(IList<ITaskMessageBusSender> items)
            : base(items)
        {
        }

        #endregion Constructors

        #region ITaskMessageBusSender Members

        /// <inheritdoc />
        public void NotifyTaskSubmitted(Guid taskId, DateTime timestampUtc, bool isPollingQueueTask)
        {
            foreach (ITaskMessageBusSender mb in this)
            {
                mb.NotifyTaskSubmitted(taskId, timestampUtc, isPollingQueueTask);
            }
        }

        /// <inheritdoc />
        public void NotifyTaskAssigned(Guid taskId, Guid taskProcessorId)
        {
            foreach (ITaskMessageBusSender mb in this)
            {
                mb.NotifyTaskAssigned(taskId, taskProcessorId);
            }
        }

        /// <inheritdoc />
        public void NotifyTaskStarted(Guid taskId, Guid taskProcessorId, DateTime timestampUtc)
        {
            foreach (ITaskMessageBusSender mb in this)
            {
                mb.NotifyTaskStarted(taskId, taskProcessorId, timestampUtc);
            }
        }

        /// <inheritdoc />
        public void NotifyTaskProgress(Guid taskId, double percentage)
        {
            foreach (ITaskMessageBusSender mb in this)
            {
                mb.NotifyTaskProgress(taskId, percentage);
            }
        }

        /// <inheritdoc />
        public void NotifyTaskCancelRequest(Guid taskId, DateTime timestampUtc)
        {
            foreach (ITaskMessageBusSender mb in this)
            {
                mb.NotifyTaskCancelRequest(taskId, timestampUtc);
            }
        }

        /// <inheritdoc />
        public void NotifyTaskCancelCompleted(Guid taskId, DateTime timestampUtc, Guid taskProcessorId, bool isTaskProcessorStopping)
        {
            foreach (ITaskMessageBusSender mb in this)
            {
                mb.NotifyTaskCancelCompleted(taskId, timestampUtc, taskProcessorId, isTaskProcessorStopping);
            }
        }

        /// <inheritdoc />
        public void NotifyTaskFailed(Guid taskId, DateTime timestampUtc, Guid taskProcessorId, bool isTaskProcessorStopping, string error)
        {
            foreach (ITaskMessageBusSender mb in this)
            {
                mb.NotifyTaskFailed(taskId, timestampUtc, taskProcessorId, isTaskProcessorStopping, error);
            }
        }

        /// <inheritdoc />
        public void NotifyTaskCompleted(Guid taskId, DateTime timestampUtc, Guid taskProcessorId, bool isTaskProcessorStopping, TimeSpan totalCpuTime)
        {
            foreach (ITaskMessageBusSender mb in this)
            {
                mb.NotifyTaskCompleted(taskId, timestampUtc, taskProcessorId, isTaskProcessorStopping, totalCpuTime);
            }
        }

        /// <inheritdoc />
        public bool IsSupported(Type taskType)
        {
            return this.Any(mb => mb.IsSupported(taskType));
        }

        #endregion ITaskMessageBusSender Members

        /// <inheritdoc />
        protected override void InsertItem(int index, ITaskMessageBusSender item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            base.InsertItem(index, item);
        }

        /// <inheritdoc />
        protected override void SetItem(int index, ITaskMessageBusSender item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            base.SetItem(index, item);
        }
    }
}