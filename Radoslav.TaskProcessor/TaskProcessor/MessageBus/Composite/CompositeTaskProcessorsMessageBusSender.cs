using System;
using System.Collections.ObjectModel;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.MessageBus
{
    /// <summary>
    /// Composite implementation of <see cref="ITaskProcessorMessageBusSender"/>.
    /// </summary>
    public sealed class CompositeTaskProcessorMessageBusSender : Collection<ITaskProcessorMessageBusSender>, ITaskProcessorMessageBusSender
    {
        #region ITaskProcessorMessageBusSender Members

        /// <inheritdoc />
        public void NotifyStateChanged(Guid taskProcessorId, TaskProcessorState state)
        {
            foreach (ITaskProcessorMessageBusSender mb in this)
            {
                mb.NotifyStateChanged(taskProcessorId, state);
            }
        }

        /// <inheritdoc />
        public void NotifyMasterModeChangeRequest(Guid taskProcessorId, bool isMaster)
        {
            foreach (ITaskProcessorMessageBusSender mb in this)
            {
                mb.NotifyMasterModeChangeRequest(taskProcessorId, isMaster);
            }
        }

        /// <inheritdoc />
        public void NotifyMasterModeChanged(Guid taskProcessorId, bool isMaster, MasterModeChangeReason reason)
        {
            foreach (ITaskProcessorMessageBusSender mb in this)
            {
                mb.NotifyMasterModeChanged(taskProcessorId, isMaster, reason);
            }
        }

        /// <inheritdoc />
        public void NotifyStopRequested(Guid taskProcessorId)
        {
            foreach (ITaskProcessorMessageBusSender mb in this)
            {
                mb.NotifyStopRequested(taskProcessorId);
            }
        }

        /// <inheritdoc />
        public void NotifyPerformanceMonitoring(TimeSpan refreshInterval)
        {
            foreach (ITaskProcessorMessageBusSender mb in this)
            {
                mb.NotifyPerformanceMonitoring(refreshInterval);
            }
        }

        /// <inheritdoc />
        public void NotifyPerformanceReport(TaskProcessorPerformanceReport performanceInfo)
        {
            foreach (ITaskProcessorMessageBusSender mb in this)
            {
                mb.NotifyPerformanceReport(performanceInfo);
            }
        }

        /// <inheritdoc />
        public void NotifyConfigurationChanged(Guid taskProcessorId)
        {
            foreach (ITaskProcessorMessageBusSender mb in this)
            {
                mb.NotifyConfigurationChanged(taskProcessorId);
            }
        }

        #endregion ITaskProcessorMessageBusSender Members

        /// <inheritdoc />
        protected override void InsertItem(int index, ITaskProcessorMessageBusSender item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            base.InsertItem(index, item);
        }

        /// <inheritdoc />
        protected override void SetItem(int index, ITaskProcessorMessageBusSender item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            base.SetItem(index, item);
        }
    }
}