using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Radoslav.Redis;
using Radoslav.Serialization;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Repository.Redis
{
    /// <summary>
    /// An implementation of <see cref="ITaskRuntimeInfoRepository"/> that uses Redis for storage.
    /// </summary>
    public sealed partial class RedisTaskRuntimeInfoRepository : ITaskRuntimeInfoRepository
    {
        private const string PendingTasksList = "PendingTasks";
        private const string ActiveTasksList = "ActiveTasks";
        private const string FailedTasksList = "FailedTasks";
        private const string ArchiveTasksHash = "ArchiveTasks";

        private readonly IRedisProvider provider;
        private readonly IEntityBinarySerializer serializer;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTaskRuntimeInfoRepository"/> class.
        /// </summary>
        /// <param name="provider">The Redis provider to use.</param>
        /// <param name="serializer">The serializer to use when archiving task runtime information.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="provider"/> or <paramref name="serializer"/> is null.</exception>
        public RedisTaskRuntimeInfoRepository(IRedisProvider provider, IEntityBinarySerializer serializer)
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

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets the Redis provider used by the repository.
        /// </summary>
        /// <value>The Redis provider used by the repository.</value>
        public IRedisProvider Provider
        {
            get { return this.provider; }
        }

        #endregion Properties

        #region ITaskRuntimeInfoRepository Members

        /// <inheritdoc />
        public IEnumerable<ITaskRuntimeInfo> GetPending(bool includePollingQueueTasks)
        {
            Trace.WriteLine("ENTER: Getting runtime information for pending tasks ({0} polling queue tasks).".FormatInvariant(includePollingQueueTasks ? "include" : "without"));

            List<ITaskRuntimeInfo> result = this.GetAll(RedisTaskRuntimeInfoRepository.PendingTasksList);

            if (includePollingQueueTasks)
            {
                var listKeys = this.provider.SearchKeys(RedisTaskRuntimeInfoRepository.PendingTasksList + "$*");

                List<string> entityIds = new List<string>();

                using (IRedisPipeline pipeline = this.provider.CreatePipeline())
                {
                    foreach (string listKey in listKeys)
                    {
                        pipeline.GetList(listKey, values => entityIds.AddRange(values));
                    }

                    pipeline.Flush();
                }

                result.AddRange(this.GetAll(entityIds).Where(t => t.Status == TaskStatus.Pending));
            }

            Trace.WriteLine("EXIT: Return runtime information for {0} pending tasks ({1} polling queue tasks).".FormatInvariant(result.Count, includePollingQueueTasks ? "include" : "without"));

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<ITaskRuntimeInfo> GetActive()
        {
            Trace.WriteLine("ENTER: Getting runtime information for active tasks ...");

            ICollection<ITaskRuntimeInfo> result = this.GetAll(RedisTaskRuntimeInfoRepository.ActiveTasksList);

            Trace.WriteLine("EXIT: Return runtime information for {0} active tasks.".FormatInvariant(result.Count));

            return result;
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<TaskStatus, IEnumerable<ITaskRuntimeInfo>> GetPendingAndActive()
        {
            Trace.WriteLine("ENTER: Getting runtime information for pending and active tasks ...");

            Dictionary<TaskStatus, IEnumerable<ITaskRuntimeInfo>> result = this.GetAllByType(
                RedisTaskRuntimeInfoRepository.PendingTasksList,
                RedisTaskRuntimeInfoRepository.ActiveTasksList);

            if (!result.ContainsKey(TaskStatus.Pending))
            {
                result.Add(TaskStatus.Pending, Enumerable.Empty<ITaskRuntimeInfo>());
            }

            if (!result.ContainsKey(TaskStatus.InProgress))
            {
                result.Add(TaskStatus.InProgress, Enumerable.Empty<ITaskRuntimeInfo>());
            }

            Trace.WriteLine("EXIT: Return runtime information for {0} pending and {1} active tasks.".FormatInvariant(result[TaskStatus.Pending].Count(), result[TaskStatus.InProgress].Count()));

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<ITaskRuntimeInfo> GetFailed()
        {
            Trace.WriteLine("ENTER: Getting runtime information for failed tasks ...");

            IEnumerable<string> entityIds = this.provider.GetListAsText(RedisTaskRuntimeInfoRepository.FailedTasksList);

            List<ITaskRuntimeInfo> result = new List<ITaskRuntimeInfo>();

            using (IRedisPipeline pipeline = this.provider.CreatePipeline())
            {
                foreach (string taskId in entityIds)
                {
                    pipeline.GetHashBinaryValue(RedisTaskRuntimeInfoRepository.ArchiveTasksHash, taskId, content =>
                        result.Add((ITaskRuntimeInfo)this.serializer.Deserialize(content, typeof(RedisTaskRuntimeInfo))));
                }

                pipeline.Flush();
            }

            Trace.WriteLine("EXIT: Return runtime information for {0} failed tasks.".FormatInvariant(result.Count));

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<ITaskRuntimeInfo> GetArchive()
        {
            Trace.WriteLine("ENTER: Getting runtime information for archive tasks ...");

            return this.provider.GetHashValuesAsBinary(RedisTaskRuntimeInfoRepository.ArchiveTasksHash)
                .Select(content => this.serializer.Deserialize(content, typeof(RedisTaskRuntimeInfo)))
                .Cast<ITaskRuntimeInfo>();
        }

        /// <inheritdoc />
        public IEnumerable<ITaskRuntimeInfo> ReservePollingQueueTasks(string pollingQueueKey, int maxResults)
        {
            Trace.WriteLine("ENTER: Reserving {0} polling queue '{1}' tasks for execution ...".FormatInvariant(maxResults, pollingQueueKey));

            if (string.IsNullOrEmpty(pollingQueueKey))
            {
                throw new ArgumentNullException(nameof(pollingQueueKey));
            }

            if (maxResults < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxResults), maxResults, "Value must not be negative.");
            }

            string pollingQueueListKey = RedisTaskRuntimeInfoRepository.GetPollingQueueRedisKey(pollingQueueKey, TaskStatus.Pending);

            List<string> entityIds = new List<string>();

            using (IRedisPipeline pipeline = this.provider.CreatePipeline())
            {
                for (int i = 0; i < maxResults; i++)
                {
                    pipeline.PopFirstListElementAsText(pollingQueueListKey, value =>
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            entityIds.Add(value);
                        }
                    });
                }

                pipeline.Flush();
            }

            ICollection<ITaskRuntimeInfo> result = this.GetAll(entityIds);

            Trace.WriteLine("EXIT: {0} polling queue '{1}' tasks reserved for execution.".FormatInvariant(result.Count, pollingQueueKey));

            return result;
        }

        /// <inheritdoc />
        public ITaskRuntimeInfo GetById(Guid taskId)
        {
            Trace.WriteLine("ENTER: Getting runtime information for task '{0}' ...".FormatInvariant(taskId));

            ITaskRuntimeInfo result = this.GetById(taskId, false);

            if (result == null)
            {
                Trace.WriteLine("ENTER: Runtime information for task '{0}' not found among pending and active tasks, so search in archive ...".FormatInvariant(taskId));

                result = this.GetById(taskId, true);
            }

            if (result == null)
            {
                Trace.WriteLine("EXIT: Runtime information for task '{0}' not found.".FormatInvariant(taskId));
            }
            else
            {
                Trace.WriteLine("EXIT: Return runtime information for task '{0}'.".FormatInvariant(taskId));
            }

            return result;
        }

        /// <inheritdoc />
        public Type GetTaskType(Guid taskId)
        {
            Trace.WriteLine("ENTER: Getting task '{0}' type ...".FormatInvariant(taskId));

            string entityKey = RedisTaskRuntimeInfoRepository.GetEntityKey(taskId);

            string taskTypeAsString = this.provider.GetHashTextValue(entityKey, "TaskType");

            Type result = null;

            if (!string.IsNullOrEmpty(taskTypeAsString))
            {
#if DEBUG
                result = Type.GetType(taskTypeAsString, true);
#else
                result = Type.GetType(taskTypeAsString, false);

                if (result == null)
                {
                    Trace.TraceWarning("EXIT: Task '{0}' type '{1}' cannot be resolved.", taskId, taskTypeAsString);

                    return null;
                }
#endif
            }
            else
            {
                RedisTaskRuntimeInfo taskInfo = this.GetById(taskId, true);

                if (taskInfo != null)
                {
                    result = taskInfo.TaskType;
                }
            }

            if (result == null)
            {
                Trace.WriteLine("EXIT: Task '{0}' type was not found neither in hash nor in archive.".FormatInvariant(taskId));
            }
            else
            {
                Trace.WriteLine("EXIT: Return task '{0}' type '{1}'.".FormatInvariant(taskId, result));
            }

            return result;
        }

        /// <inheritdoc />
        public ITaskRuntimeInfo Create(Guid taskId, Type taskType, DateTime submittedUtc, TaskPriority priority, string pollingQueue)
        {
            if (taskType == null)
            {
                throw new ArgumentNullException(nameof(taskType));
            }

            if (!typeof(ITask).IsAssignableFrom(taskType))
            {
                throw new ArgumentException("Task type '{0}' does not implement '{1}'.".FormatInvariant(taskType, typeof(ITask)), nameof(taskType));
            }

            return new RedisTaskRuntimeInfo(taskId, taskType, submittedUtc, TaskStatus.Pending)
            {
                Priority = priority,
                PollingQueue = pollingQueue
            };
        }

        /// <inheritdoc />
        public void Add(ITaskRuntimeInfo taskInfo)
        {
            if (taskInfo == null)
            {
                throw new ArgumentNullException("taskInfo");
            }

            Trace.WriteLine("ENTER: Adding runtime information for task '{0}' of type '{1}' with priority '{2}' in polling queue '{3}' ...".FormatInvariant(taskInfo.TaskId, taskInfo.TaskType, taskInfo.Priority, taskInfo.PollingQueue));

            taskInfo.ValidateForAdd();

            string entityKey = RedisTaskRuntimeInfoRepository.GetEntityKey(taskInfo.TaskId);

            string addToListKey;

            Dictionary<string, string> values = new Dictionary<string, string>()
            {
                { "Id", RedisConverter.ToString(taskInfo.TaskId) },
                { "TaskType", RedisConverter.ToString(taskInfo.TaskType, true) },
                { "SubmittedUtc", RedisConverter.ToString(taskInfo.SubmittedUtc) },
                { "Status", RedisConverter.ToString(taskInfo.Status) }
            };

            if (string.IsNullOrEmpty(taskInfo.PollingQueue))
            {
                values.Add("Priority", RedisConverter.ToString(taskInfo.Priority));

                addToListKey = RedisTaskRuntimeInfoRepository.PendingTasksList;
            }
            else
            {
                values.Add("PollingQueue", taskInfo.PollingQueue);

                addToListKey = RedisTaskRuntimeInfoRepository.GetPollingQueueRedisKey(taskInfo.PollingQueue, TaskStatus.Pending);
            }

            using (IRedisTransaction transaction = this.Provider.CreateTransaction())
            {
                transaction.SetHashValues(entityKey, values);

                transaction.AddToList(addToListKey, RedisConverter.ToString(taskInfo.TaskId));

                transaction.Commit();
            }

            Trace.WriteLine("EXIT: Runtime information for task '{0}' of type '{1}' with priority '{2}' in polling queue '{3}' added.".FormatInvariant(taskInfo.TaskId, taskInfo.TaskType, taskInfo.Priority, taskInfo.PollingQueue));
        }

        /// <inheritdoc />
        public void Assign(Guid taskId, Guid? taskProcessorId)
        {
            Trace.WriteLine("ENTER: Recording task '{0}' assigned to processor '{1}' ...".FormatInvariant(taskId, taskProcessorId));

            string entityKey = RedisTaskRuntimeInfoRepository.GetEntityKey(taskId);

            this.Provider.SetHashValue(entityKey, "TaskProcessorId", RedisConverter.ToString(taskProcessorId));

            Trace.WriteLine("EXIT: Task '{0}' assigned to processor '{1}' recorded.".FormatInvariant(taskId, taskProcessorId));
        }

        /// <inheritdoc />
        public void Start(Guid taskId, Guid taskProcessorId, DateTime timestampUtc)
        {
            Trace.WriteLine("ENTER: Recording task '{0}' started by processor '{1}' ...".FormatInvariant(taskId, taskProcessorId));

            string entityKey = RedisTaskRuntimeInfoRepository.GetEntityKey(taskId);

            Dictionary<string, string> values = new Dictionary<string, string>()
            {
                { "Status", RedisConverter.ToString(TaskStatus.InProgress) },
                { "TaskProcessorId", RedisConverter.ToString(taskProcessorId) },
                { "StartedUtc", RedisConverter.ToString(timestampUtc) }
            };

            using (IRedisTransaction transaction = this.Provider.CreateTransaction())
            {
                transaction.SetHashValues(entityKey, values);
                transaction.RemoveFromList(RedisTaskRuntimeInfoRepository.PendingTasksList, RedisConverter.ToString(taskId));
                transaction.AddToList(RedisTaskRuntimeInfoRepository.ActiveTasksList, RedisConverter.ToString(taskId));

                transaction.Commit();
            }

            Trace.WriteLine("EXIT: Task '{0}' assigned to processor '{1}' recorded.".FormatInvariant(taskId, taskProcessorId));
        }

        /// <inheritdoc />
        public void Progress(Guid taskId, double percentage)
        {
            Trace.WriteLine("ENTER: Record task '{0}' progress to {1}% ...".FormatInvariant(taskId, percentage));

            if ((percentage < 0) || (percentage > 100))
            {
                throw new ArgumentOutOfRangeException("percentage");
            }

            string entityKey = RedisTaskRuntimeInfoRepository.GetEntityKey(taskId);

            this.Provider.SetHashValue(entityKey, "Percentage", RedisConverter.ToString(percentage));

            Trace.WriteLine("ENTER: Task '{0}' progress to {1}% recorded.".FormatInvariant(taskId, percentage));
        }

        /// <inheritdoc />
        public void RequestCancel(Guid taskId, DateTime timestampUtc)
        {
            Trace.WriteLine("ENTER: Record task '{0}' cancel request ...".FormatInvariant(taskId));

            string taskIdAsString = RedisConverter.ToString(taskId);

            string entityKey = RedisTaskRuntimeInfoRepository.GetEntityKey(taskIdAsString);

            RedisTaskRuntimeInfo taskInfo = this.GetById(taskId, false);

            taskInfo.Status = TaskStatus.Canceled;
            taskInfo.CanceledUtc = timestampUtc;

            byte[] content = this.serializer.Serialize(taskInfo);

            using (IRedisTransaction transaction = this.Provider.CreateTransaction())
            {
                transaction.RemoveKey(entityKey);
                transaction.RemoveFromList(RedisTaskRuntimeInfoRepository.PendingTasksList, taskIdAsString);
                transaction.RemoveFromList(RedisTaskRuntimeInfoRepository.ActiveTasksList, taskIdAsString);

                transaction.SetHashValue(RedisTaskRuntimeInfoRepository.ArchiveTasksHash, taskIdAsString, content);

                transaction.Commit();
            }

            Trace.WriteLine("EXIT: Task '{0}' cancel request recorded.".FormatInvariant(taskId));
        }

        /// <inheritdoc />
        public void CompleteCancel(Guid taskId, DateTime timestampUtc)
        {
            Trace.WriteLine("ENTER: Record task '{0}' cancel completed ...".FormatInvariant(taskId));

            string taskIdAsString = RedisConverter.ToString(taskId);

            RedisTaskRuntimeInfo taskInfo = this.GetById(taskId, true);

            taskInfo.Status = TaskStatus.Canceled;
            taskInfo.CompletedUtc = timestampUtc;

            byte[] content = this.serializer.Serialize(taskInfo);

            this.provider.SetHashValue(RedisTaskRuntimeInfoRepository.ArchiveTasksHash, taskIdAsString, content);

            Trace.WriteLine("EXIT: Task '{0}' cancel completed recorded.".FormatInvariant(taskId));
        }

        /// <inheritdoc />
        public void Fail(Guid taskId, DateTime timestampUtc, Exception error)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            Trace.WriteLine("ENTER: Record task '{0}' failed with error '{1}' ...".FormatInvariant(taskId, error.Message));

            string taskIdAsString = RedisConverter.ToString(taskId);

            string entityKey = RedisTaskRuntimeInfoRepository.GetEntityKey(taskIdAsString);

            RedisTaskRuntimeInfo taskInfo = this.GetById(taskId, false);

            taskInfo.Status = TaskStatus.Failed;
            taskInfo.CompletedUtc = timestampUtc;
            taskInfo.Error = error.ToString();

            byte[] content = this.serializer.Serialize(taskInfo);

            using (IRedisTransaction transaction = this.Provider.CreateTransaction())
            {
                transaction.RemoveKey(entityKey);
                transaction.RemoveFromList(RedisTaskRuntimeInfoRepository.ActiveTasksList, taskIdAsString);
                transaction.AddToList(RedisTaskRuntimeInfoRepository.FailedTasksList, taskIdAsString);
                transaction.SetHashValue(RedisTaskRuntimeInfoRepository.ArchiveTasksHash, taskIdAsString, content);

                transaction.Commit();
            }

            Trace.WriteLine("EXIT: Task '{0}' failed with error '{1}' recorded.".FormatInvariant(taskId, error.Message));
        }

        /// <inheritdoc />
        public void Complete(Guid taskId, DateTime timestampUtc)
        {
            Trace.WriteLine("ENTER: Record task '{0}' completed ...".FormatInvariant(taskId));

            string taskIdAsString = RedisConverter.ToString(taskId);

            string entityKey = RedisTaskRuntimeInfoRepository.GetEntityKey(taskIdAsString);

            RedisTaskRuntimeInfo taskInfo = this.GetById(taskId, false);

            taskInfo.Percentage = 100;
            taskInfo.Status = TaskStatus.Success;
            taskInfo.CompletedUtc = timestampUtc;

            byte[] content = this.serializer.Serialize(taskInfo);

            using (IRedisTransaction transaction = this.Provider.CreateTransaction())
            {
                transaction.RemoveKey(entityKey);
                transaction.RemoveFromList(RedisTaskRuntimeInfoRepository.ActiveTasksList, taskIdAsString);
                transaction.SetHashValue(RedisTaskRuntimeInfoRepository.ArchiveTasksHash, taskIdAsString, content);

                transaction.Commit();
            }

            Trace.WriteLine("EXIT: Task '{0}' completed recorded.".FormatInvariant(taskId));
        }

        /// <inheritdoc />
        public bool CheckIsPendingOrActive(Guid taskId)
        {
            Trace.WriteLine("ENTER: Checking if task '{0}' is pending or active ...".FormatInvariant(taskId));

            string entityKey = RedisTaskRuntimeInfoRepository.GetEntityKey(taskId);

            bool result = this.provider.ContainsKey(entityKey);

            Trace.WriteLine("EXIT: Check whether task '{0}' is pending or active completed. Result is {1}.".FormatInvariant(taskId, result));

            return result;
        }

        #endregion ITaskRuntimeInfoRepository Members

        #region Internal Methods

        private static string GetEntityKey(Guid entityId)
        {
            return RedisTaskRuntimeInfoRepository.GetEntityKey(RedisConverter.ToString(entityId));
        }

        private static string GetEntityKey(string entityId)
        {
            return "TaskRuntimeInfo$" + entityId;
        }

        private static string GetPollingQueueRedisKey(string pollingQueueKey, TaskStatus status)
        {
            switch (status)
            {
                case TaskStatus.Pending:
                    return string.Concat(RedisTaskRuntimeInfoRepository.PendingTasksList, "$", pollingQueueKey);

                default:
                    throw new NotSupportedException<TaskStatus>(status);
            }
        }

        private static RedisTaskRuntimeInfo Convert(IReadOnlyDictionary<string, string> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            RedisTaskRuntimeInfo result = new RedisTaskRuntimeInfo(
                RedisConverter.ParseGuid(values["Id"]),
                RedisConverter.ParseType(values["TaskType"]),
                RedisConverter.ParseDateTime(values["SubmittedUtc"]),
                RedisConverter.ParseEnum<TaskStatus>(values["Status"]));

            string value;

            if (values.TryGetValue("PollingQueue", out value))
            {
                result.PollingQueue = value;
            }

            if (values.TryGetValue("Priority", out value))
            {
                result.Priority = RedisConverter.ParseEnum<TaskPriority>(value);
            }

            if (values.TryGetValue("TaskProcessorId", out value))
            {
                result.TaskProcessorId = RedisConverter.ParseGuidOrNull(value);
            }

            if (values.TryGetValue("StartedUtc", out value))
            {
                result.StartedUtc = RedisConverter.ParseDateTimeOrNull(value);
            }

            if (values.TryGetValue("Percentage", out value))
            {
                result.Percentage = RedisConverter.ParseDouble(value);
            }

            if (values.TryGetValue("CanceledUtc", out value))
            {
                result.CanceledUtc = RedisConverter.ParseDateTimeOrNull(value);
            }

            if (values.TryGetValue("CompletedUtc", out value))
            {
                result.CompletedUtc = RedisConverter.ParseDateTimeOrNull(value);
            }

            if (values.TryGetValue("Error", out value))
            {
                result.Error = value;
            }

            return result;
        }

        private List<ITaskRuntimeInfo> GetAll(IEnumerable<string> entityIds)
        {
            List<ITaskRuntimeInfo> result = new List<ITaskRuntimeInfo>();

            using (IRedisPipeline pipeline = this.provider.CreatePipeline())
            {
                foreach (string entityId in entityIds)
                {
                    string entityKey = RedisTaskRuntimeInfoRepository.GetEntityKey(entityId);

                    pipeline.GetHash(entityKey, values =>
                    {
                        if (values.Count > 0)
                        {
                            result.Add(RedisTaskRuntimeInfoRepository.Convert(values));
                        }
                    });
                }

                pipeline.Flush();
            }

            return result;
        }

        private List<ITaskRuntimeInfo> GetAll(string listKey)
        {
            IEnumerable<string> entityIds = this.provider.GetListAsText(listKey);

            return this.GetAll(entityIds);
        }

        private Dictionary<TaskStatus, IEnumerable<ITaskRuntimeInfo>> GetAllByType(params string[] listKeys)
        {
            List<string> entityIds = new List<string>();

            using (IRedisPipeline pipeline = this.provider.CreatePipeline())
            {
                foreach (string listKey in listKeys)
                {
                    pipeline.GetList(listKey, values => entityIds.AddRange(values));
                }

                pipeline.Flush();
            }

            Dictionary<TaskStatus, IEnumerable<ITaskRuntimeInfo>> result = new Dictionary<TaskStatus, IEnumerable<ITaskRuntimeInfo>>();

            using (IRedisPipeline pipeline = this.provider.CreatePipeline())
            {
                foreach (string entityId in entityIds)
                {
                    string entityKey = RedisTaskRuntimeInfoRepository.GetEntityKey(entityId);

                    pipeline.GetHash(entityKey, values =>
                    {
                        if (values.Count > 0)
                        {
                            ITaskRuntimeInfo taskInfo = RedisTaskRuntimeInfoRepository.Convert(values);

                            IEnumerable<ITaskRuntimeInfo> collection;

                            if (!result.TryGetValue(taskInfo.Status, out collection))
                            {
                                collection = new List<ITaskRuntimeInfo>();

                                result.Add(taskInfo.Status, collection);
                            }

                            ((ICollection<ITaskRuntimeInfo>)collection).Add(taskInfo);
                        }
                    });
                }

                pipeline.Flush();
            }

            return result;
        }

        private RedisTaskRuntimeInfo GetById(Guid taskId, bool fromArchive)
        {
            string taskIdAsString = RedisConverter.ToString(taskId);

            string entityKey = RedisTaskRuntimeInfoRepository.GetEntityKey(taskIdAsString);

            if (fromArchive)
            {
                byte[] content = this.provider.GetHashBinaryValue(RedisTaskRuntimeInfoRepository.ArchiveTasksHash, taskIdAsString);

                if (content == null)
                {
                    return null;
                }

                return (RedisTaskRuntimeInfo)this.serializer.Deserialize(content, typeof(RedisTaskRuntimeInfo));
            }
            else
            {
                IReadOnlyDictionary<string, string> values = this.provider.GetHashAsText(entityKey);

                if (values.Count == 0)
                {
                    return null;
                }

                return RedisTaskRuntimeInfoRepository.Convert(values);
            }
        }

        #endregion Internal Methods
    }
}