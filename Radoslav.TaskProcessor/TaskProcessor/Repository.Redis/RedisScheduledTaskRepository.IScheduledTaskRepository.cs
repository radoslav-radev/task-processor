using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Radoslav.Redis;
using Radoslav.TaskProcessor.TaskScheduler;

namespace Radoslav.TaskProcessor.Repository.Redis
{
    /// <summary>
    /// An implementation of the <see cref="IScheduledTaskRepository"/> that uses Redis for storage.
    /// </summary>
    public sealed partial class RedisScheduledTaskRepository : IScheduledTaskRepository
    {
        #region IScheduledTasksRepository Members

        /// <inheritdoc />
        public event EventHandler<ScheduledTaskEventArgs> Added;

        /// <inheritdoc />
        public event EventHandler<ScheduledTaskEventArgs> Updated;

        /// <inheritdoc />
        public event EventHandler<ScheduledTaskEventArgs> Deleted;

        /// <inheritdoc />
        public bool RaiseEvents
        {
            get
            {
                return (this.subscription != null) && this.subscription.ActiveChannels.Any();
            }

            set
            {
                if (value)
                {
                    if (this.subscription == null)
                    {
                        this.subscription = this.provider.CreateSubscription();

                        this.subscription.MessageReceived += this.OnMessageReceived;
                    }

                    if (!this.subscription.ActiveChannels.Any())
                    {
                        this.subscription.SubscribeToChannels(this.subscribeTimeout, RedisScheduledTaskRepository.Channel);
                    }
                }
                else
                {
                    if ((this.subscription != null) && this.subscription.ActiveChannels.Any())
                    {
                        this.subscription.UnsubscribeFromChannels(this.subscribeTimeout, RedisScheduledTaskRepository.Channel);
                    }
                }
            }
        }

        /// <inheritdoc />
        public IEnumerable<IScheduledTask> GetAll()
        {
            Trace.WriteLine("ENTER: Getting all scheduled tasks ...");

            var values = this.provider.GetHashAsBinary(RedisScheduledTaskRepository.ScheduledTasksHashKey);

            foreach (var pair in values.Where(p => p.Key.EndsWith("$Content")))
            {
                string scheduledTaskIdAsString = pair.Key.Substring(0, pair.Key.Length - "$Content".Length);

                IScheduledTask result;

                byte[] recurrenceDefinitionContent = null;

                if (!values.TryGetValue(scheduledTaskIdAsString + "$RecurrenceDefinition", out recurrenceDefinitionContent))
                {
                    continue;
                }

                if (this.serializer.CanDetermineEntityTypeFromContent)
                {
                    result = (IScheduledTask)this.serializer.Deserialize(pair.Value);

                    result.Schedule = (IScheduleDefinition)this.serializer.Deserialize(recurrenceDefinitionContent);
                }
                else
                {
                    byte[] scheduledTaskTypeAsBinary;
                    string scheduledTaskTypeAsString = null;
                    byte[] recurrenceDefinitionTaskTypeAsBinary;
                    string recurrenceDefinitionTaskTypeAsString = null;

                    if (values.TryGetValue(scheduledTaskIdAsString + "$Type", out scheduledTaskTypeAsBinary))
                    {
                        scheduledTaskTypeAsString = Encoding.UTF8.GetString(scheduledTaskTypeAsBinary);
                    }

                    if (values.TryGetValue(scheduledTaskIdAsString + "$RecurrenceDefinition$Type", out recurrenceDefinitionTaskTypeAsBinary))
                    {
                        recurrenceDefinitionTaskTypeAsString = Encoding.UTF8.GetString(recurrenceDefinitionTaskTypeAsBinary);
                    }

                    result = this.Deserialize(scheduledTaskIdAsString, pair.Value, scheduledTaskTypeAsString, recurrenceDefinitionContent, recurrenceDefinitionTaskTypeAsString);
                }

                yield return result;
            }

            Trace.WriteLine("EXIT: All scheduled tasks returned.");
        }

        /// <inheritdoc />
        public IScheduledTask GetById(Guid scheduledTaskId)
        {
            Trace.WriteLine("ENTER: Getting scheduled task with ID '{0}' ...".FormatInvariant(scheduledTaskId));

            string taskIdAsString = RedisConverter.ToString(scheduledTaskId);

            IScheduledTask result;

            byte[] scheduledTaskContent = null;
            byte[] recurrenceDefinitionContent = null;

            using (IRedisPipeline pipeline = this.provider.CreatePipeline())
            {
                pipeline.GetHashBinaryValue(RedisScheduledTaskRepository.ScheduledTasksHashKey, taskIdAsString + "$Content", value => scheduledTaskContent = value);
                pipeline.GetHashBinaryValue(RedisScheduledTaskRepository.ScheduledTasksHashKey, taskIdAsString + "$RecurrenceDefinition", value => recurrenceDefinitionContent = value);

                if (this.serializer.CanDetermineEntityTypeFromContent)
                {
                    pipeline.Flush();

                    if ((scheduledTaskContent == null) || (scheduledTaskContent.Length == 0) || (recurrenceDefinitionContent == null) || (recurrenceDefinitionContent.Length == 0))
                    {
                        return null;
                    }

                    result = (IScheduledTask)this.serializer.Deserialize(scheduledTaskContent);

                    result.Schedule = (IScheduleDefinition)this.serializer.Deserialize(recurrenceDefinitionContent);
                }
                else
                {
                    string scheduledTaskType = null;
                    string recurrenceDefinitionType = null;

                    pipeline.GetHashTextValue(RedisScheduledTaskRepository.ScheduledTasksHashKey, taskIdAsString + "$Type", value => scheduledTaskType = value);
                    pipeline.GetHashTextValue(RedisScheduledTaskRepository.ScheduledTasksHashKey, taskIdAsString + "$RecurrenceDefinition$Type", value => recurrenceDefinitionType = value);

                    pipeline.Flush();

                    result = this.Deserialize(scheduledTaskId.ToString(), scheduledTaskContent, scheduledTaskType, recurrenceDefinitionContent, recurrenceDefinitionType);
                }
            }

            Trace.WriteLine("EXIT: Scheduled task '{0}' with ID '{1}' returned.".FormatInvariant(result, scheduledTaskId));

            return result;
        }

        /// <inheritdoc />
        public void Add(IScheduledTask scheduledTask)
        {
            if (scheduledTask == null)
            {
                throw new ArgumentNullException(nameof(scheduledTask));
            }

            if (scheduledTask.Schedule == null)
            {
                throw new ArgumentException("'{0}' recurrence definition is null.".FormatInvariant(scheduledTask), nameof(scheduledTask));
            }

            Trace.WriteLine("ENTER: Adding scheduled task '{0}' with ID '{1}' ...".FormatInvariant(scheduledTask, scheduledTask.Id));

            this.AddOrUpdate(scheduledTask);

            this.provider.PublishMessage(RedisScheduledTaskRepository.Channel, RedisConverter.ToString("Add", scheduledTask.Id));

            Trace.WriteLine("EXIT: Task '{0}' with ID '{1}' added.".FormatInvariant(scheduledTask, scheduledTask.Id));
        }

        /// <inheritdoc />
        public void Update(IScheduledTask scheduledTask)
        {
            if (scheduledTask == null)
            {
                throw new ArgumentNullException(nameof(scheduledTask));
            }

            if (scheduledTask.Schedule == null)
            {
                throw new ArgumentException("'{0}' recurrence definition is null.".FormatInvariant(scheduledTask), nameof(scheduledTask));
            }

            Trace.WriteLine("ENTER: Updating scheduled task '{0}' with ID '{1}' ...".FormatInvariant(scheduledTask, scheduledTask.Id));

            this.AddOrUpdate(scheduledTask);

            this.provider.PublishMessage(RedisScheduledTaskRepository.Channel, RedisConverter.ToString("Update", scheduledTask.Id));

            Trace.WriteLine("EXIT: Task '{0}' with ID '{1}' updated.".FormatInvariant(scheduledTask, scheduledTask.Id));
        }

        /// <inheritdoc />
        public void Delete(Guid scheduledTaskId)
        {
            Trace.WriteLine("ENTER: Deleting scheduled task '{0}' ...".FormatInvariant(scheduledTaskId));

            string taskIdAsString = RedisConverter.ToString(scheduledTaskId);

            this.provider.RemoveFromHash(
                RedisScheduledTaskRepository.ScheduledTasksHashKey,
                taskIdAsString + "$Content",
                taskIdAsString + "$Type",
                taskIdAsString + "$RecurrenceDefinition",
                taskIdAsString + "$RecurrenceDefinition$Type");

            this.provider.PublishMessage(RedisScheduledTaskRepository.Channel, RedisConverter.ToString("Delete", scheduledTaskId));

            Trace.WriteLine("EXIT: Scheduled task '{0}' deleted.".FormatInvariant(scheduledTaskId));
        }

        #endregion IScheduledTasksRepository Members
    }
}