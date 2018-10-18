using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Radoslav.Redis;
using Radoslav.Serialization;
using Radoslav.TaskProcessor.TaskScheduler;

namespace Radoslav.TaskProcessor.Repository.Redis
{
    public sealed partial class RedisScheduledTaskRepository : IDisposable
    {
        private const string ScheduledTasksHashKey = "ScheduledTasks";
        private const string Channel = "Radoslav$TaskProcessor$ScheduledTasks";

        private readonly IRedisProvider provider;
        private readonly IEntityBinarySerializer serializer;

        private IRedisMessageSubscription subscription;
        private TimeSpan subscribeTimeout = TimeSpan.FromSeconds(5);

        #region Constructor & Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisScheduledTaskRepository"/> class.
        /// </summary>
        /// <param name="provider">The Redis provider to use.</param>
        /// <param name="serializer">The serializer to use.</param>
        public RedisScheduledTaskRepository(IRedisProvider provider, IEntityBinarySerializer serializer)
        {
            Trace.WriteLine("ENTER: Constructing '{0}' ...".FormatInvariant(this.GetType().Name));

            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            this.provider = provider;
            this.serializer = serializer;

            Trace.WriteLine("EXIT: '{0}' constructed.".FormatInvariant(this.GetType().Name));
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="RedisScheduledTaskRepository"/> class.
        /// </summary>
        ~RedisScheduledTaskRepository()
        {
            this.Dispose(false);
        }

        #endregion Constructor & Destructor

        #region Properties

        /// <summary>
        /// Gets the Redis provider used by the repository.
        /// </summary>
        /// <value>The Redis provider used by the repository.</value>
        public IRedisProvider Provider
        {
            get { return this.provider; }
        }

        /// <summary>
        /// Gets the Redis serializer used by the repository.
        /// </summary>
        /// <value>The Redis serializer used by the repository.</value>
        public IEntityBinarySerializer Serializer
        {
            get { return this.serializer; }
        }

        /// <summary>
        /// Gets or sets the subscribe and unsubscribe timeout.
        /// </summary>
        /// <value>The subscribe and unsubscribe timeout.</value>
        /// <exception cref="ArgumentOutOfRangeException">Value is less then <see cref="TimeSpan.Zero"/>.</exception>
        public TimeSpan SubscribeTimeout
        {
            get
            {
                return this.subscribeTimeout;
            }

            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Value must be positive.");
                }

                this.subscribeTimeout = value;
            }
        }

        #endregion Properties

        #region IDisposable Members

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.subscription != null)
            {
                this.subscription.MessageReceived -= this.OnMessageReceived;

                this.subscription.Dispose();
            }
        }

        #endregion IDisposable Members

        private void AddOrUpdate(IScheduledTask scheduledTask)
        {
            string taskIdAsString = RedisConverter.ToString(scheduledTask.Id);

            if (this.serializer.CanDetermineEntityTypeFromContent)
            {
                this.provider.SetHashValues(
                    RedisScheduledTaskRepository.ScheduledTasksHashKey,
                    new Dictionary<string, byte[]>()
                    {
                        { taskIdAsString + "$Content", this.serializer.Serialize(scheduledTask) },
                        { taskIdAsString + "$RecurrenceDefinition", this.serializer.Serialize(scheduledTask.Schedule) }
                    });
            }
            else
            {
                using (IRedisTransaction transaction = this.provider.CreateTransaction())
                {
                    transaction.SetHashValue(RedisScheduledTaskRepository.ScheduledTasksHashKey, taskIdAsString + "$Content", this.serializer.Serialize(scheduledTask));
                    transaction.SetHashValue(RedisScheduledTaskRepository.ScheduledTasksHashKey, taskIdAsString + "$Type", RedisConverter.ToString(scheduledTask.GetType(), false));
                    transaction.SetHashValue(RedisScheduledTaskRepository.ScheduledTasksHashKey, taskIdAsString + "$RecurrenceDefinition", this.serializer.Serialize(scheduledTask.Schedule));
                    transaction.SetHashValue(RedisScheduledTaskRepository.ScheduledTasksHashKey, taskIdAsString + "$RecurrenceDefinition$Type", RedisConverter.ToString(scheduledTask.Schedule.GetType(), false));

                    transaction.Commit();
                }
            }
        }

        private IScheduledTask Deserialize(string scheduledTaskId, byte[] scheduledTaskContent, string scheduledTaskTypeAsString, byte[] recurrenceDefinitionContent, string recurrenceDefinitionTypeAsString)
        {
            if ((scheduledTaskContent == null) || (scheduledTaskContent.Length == 0) || (recurrenceDefinitionContent == null) || (recurrenceDefinitionContent.Length == 0))
            {
                return null;
            }

            if (string.IsNullOrEmpty(scheduledTaskTypeAsString))
            {
#if DEBUG
                throw new TypeNotFoundInRedisException("Scheduled task '{0}' type was not found in Redis.".FormatInvariant(scheduledTaskId));
#else
                Trace.TraceWarning("EXIT: Scheduled task '{0}' type was not found in Redis.".FormatInvariant(scheduledTaskId));

                return null;
#endif
            }

            Type scheduledTaskType;
            Type recurrenceDefinitionType;

#if DEBUG
            scheduledTaskType = Type.GetType(scheduledTaskTypeAsString, true);
            recurrenceDefinitionType = Type.GetType(recurrenceDefinitionTypeAsString, true);
#else
            scheduledTaskType = Type.GetType(scheduledTaskTypeAsString, false);
            recurrenceDefinitionType = Type.GetType(recurrenceDefinitionTypeAsString, false);

            if (scheduledTaskType == null)
            {
                Trace.TraceWarning("EXIT: Scheduled task type '{0}' cannot be resolved.", scheduledTaskTypeAsString);

                return null;
            }

            if (recurrenceDefinitionType == null)
            {
                Trace.TraceWarning("EXIT: Recurrence definition type for the scheduled task '{0}' of type '{1}' cannot be resolved.", scheduledTaskTypeAsString);

                return null;
            }
#endif

            IScheduledTask result = (IScheduledTask)this.serializer.Deserialize(scheduledTaskContent, scheduledTaskType);

            result.Schedule = (IScheduleDefinition)this.serializer.Deserialize(recurrenceDefinitionContent, recurrenceDefinitionType);

            return result;
        }

        private void OnMessageReceived(object sender, RedisMessageEventArgs e)
        {
            switch (e.Channel)
            {
                case RedisScheduledTaskRepository.Channel:
                    string[] data = RedisConverter.ParseCollection<string>(e.Message).ToArray();

                    if (data.Length != 2)
                    {
                        Trace.TraceWarning("Message '{0}' received on channel '{1}' cannot be parsed.", e.Message, e.Channel);

                        return;
                    }

                    Guid scheduledTaskId;

                    try
                    {
                        scheduledTaskId = RedisConverter.ParseGuid(data[1]);
                    }
                    catch (ArgumentException ex)
                    {
                        Trace.TraceWarning(ex.Message);

                        return;
                    }

                    EventHandler<ScheduledTaskEventArgs> handler = null;

                    switch (data[0])
                    {
                        case "Add":
                            handler = this.Added;
                            break;

                        case "Update":
                            handler = this.Updated;
                            break;

                        case "Delete":
                            handler = this.Deleted;
                            break;

                        default:
                            Trace.TraceWarning("Unknown command in message '{0}' received on channel '{1}'.", e.Message, e.Channel);
                            return;
                    }

                    if (handler != null)
                    {
                        handler(this, new ScheduledTaskEventArgs(scheduledTaskId));
                    }

                    break;
            }
        }
    }
}