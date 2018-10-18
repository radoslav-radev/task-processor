using System;
using System.Collections.Generic;
using System.Diagnostics;
using Radoslav.Redis;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.Repository.Redis
{
    /// <summary>
    /// An implementation of <see cref="ITaskProcessorRuntimeInfoRepository"/> that uses Redis for storage.
    /// </summary>
    public sealed partial class RedisTaskProcessorRuntimeInfoRepository : ITaskProcessorRuntimeInfoRepository
    {
        private const string MasterTaskProcessorIdKey = "MasterTaskProcessor";
        private const string EntitySetKey = "TaskProcessors";

        private readonly IRedisProvider provider;

        private TimeSpan expiration = RedisTaskProcessorRepository.DefaultExpirationTimeout;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTaskProcessorRuntimeInfoRepository"/> class.
        /// </summary>
        /// <param name="provider">The Redis provider to use.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="provider"/> is null.</exception>
        public RedisTaskProcessorRuntimeInfoRepository(IRedisProvider provider)
        {
            this.provider = provider;
        }

        #endregion Constructors

        /// <summary>
        /// Gets the Redis provider used by the repository.
        /// </summary>
        /// <value>The Redis provider used by the repository.</value>
        public IRedisProvider Provider
        {
            get { return this.provider; }
        }

        #region ITaskProcessorRuntimeInfoRepository Members

        /// <inheritdoc />
        public TimeSpan Expiration
        {
            get
            {
                return this.expiration;
            }

            set
            {
                Trace.WriteLine("ENTER: Setting {0} expiration to {1} ...".FormatInvariant(this, value));

                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Value must be positive.");
                }

                this.expiration = value;

                Trace.WriteLine("EXIT: {0} expiration set to {1}.".FormatInvariant(this, value));
            }
        }

        /// <inheritdoc />
        public IEnumerable<ITaskProcessorRuntimeInfo> GetAll()
        {
            IEnumerable<string> entityIds = this.provider.GetSet(RedisTaskProcessorRuntimeInfoRepository.EntitySetKey);

            List<ITaskProcessorRuntimeInfo> result = new List<ITaskProcessorRuntimeInfo>();

            using (IRedisPipeline pipeline = this.provider.CreatePipeline())
            {
                foreach (string entityId in entityIds)
                {
                    string entityKey = RedisTaskProcessorRuntimeInfoRepository.GetEntityKey(entityId);

                    pipeline.GetHash(entityKey, values =>
                    {
                        if (values.Count > 0)
                        {
                            result.Add(RedisTaskProcessorRuntimeInfoRepository.Convert(values));
                        }
                    });
                }

                pipeline.Flush();
            }

            return result;
        }

        /// <inheritdoc />
        public ITaskProcessorRuntimeInfo GetById(Guid taskProcessorId)
        {
            string entityKey = RedisTaskProcessorRuntimeInfoRepository.GetEntityKey(taskProcessorId);

            IReadOnlyDictionary<string, string> values = this.provider.GetHashAsText(entityKey);

            if (values.Count == 0)
            {
                return null;
            }

            return RedisTaskProcessorRuntimeInfoRepository.Convert(values);
        }

        /// <inheritdoc />
        public ITaskProcessorRuntimeInfo Create(Guid taskProcessorId, string machineName)
        {
            if (string.IsNullOrEmpty(machineName))
            {
                throw new ArgumentNullException("machineName");
            }

            return new RedisTaskProcessorRuntimeInfo(taskProcessorId, machineName);
        }

        /// <inheritdoc />
        public void Add(ITaskProcessorRuntimeInfo taskProcessorInfo)
        {
            Trace.WriteLine("ENTER: Adding {0} ...".FormatInvariant(taskProcessorInfo));

            if (taskProcessorInfo == null)
            {
                throw new ArgumentNullException("taskProcessorInfo");
            }

            string entityKey = RedisTaskProcessorRuntimeInfoRepository.GetEntityKey(taskProcessorInfo.TaskProcessorId);

            IReadOnlyDictionary<string, string> values = RedisTaskProcessorRuntimeInfoRepository.Serialize(taskProcessorInfo);

            using (IRedisTransaction transaction = this.provider.CreateTransaction())
            {
                transaction.SetHashValues(entityKey, values);

                transaction.AddToSet(RedisTaskProcessorRuntimeInfoRepository.EntitySetKey, RedisConverter.ToString(taskProcessorInfo.TaskProcessorId));

                if (this.expiration < TimeSpan.MaxValue)
                {
                    transaction.ExpireKeyIn(entityKey, this.expiration);
                }

                transaction.Commit();
            }

            Trace.WriteLine("EXIT: {0} added.".FormatInvariant(taskProcessorInfo));
        }

        /// <inheritdoc />
        public void Update(ITaskProcessorRuntimeInfo taskProcessorInfo)
        {
            if (taskProcessorInfo == null)
            {
                throw new ArgumentNullException("taskProcessorInfo");
            }

            Trace.WriteLine("ENTER: Updating {0} ...".FormatInvariant(taskProcessorInfo));

            string entityKey = RedisTaskProcessorRuntimeInfoRepository.GetEntityKey(taskProcessorInfo.TaskProcessorId);

            IReadOnlyDictionary<string, string> values = RedisTaskProcessorRuntimeInfoRepository.Serialize(taskProcessorInfo);

            using (IRedisTransaction transaction = this.provider.CreateTransaction())
            {
                transaction.RemoveKey(entityKey);

                transaction.SetHashValues(entityKey, values);

                if (this.expiration < TimeSpan.MaxValue)
                {
                    transaction.ExpireKeyIn(entityKey, this.expiration);
                }

                transaction.Commit();
            }

            Trace.WriteLine("EXIT: {0} updated.".FormatInvariant(taskProcessorInfo));
        }

        /// <inheritdoc />
        public bool Heartbeat(Guid taskProcessorId)
        {
            Trace.WriteLine("ENTER: Heart-beating {0} ...".FormatInvariant(taskProcessorId));

            string entityKey = RedisTaskProcessorRuntimeInfoRepository.GetEntityKey(taskProcessorId);

            if (this.provider.ExpireKeyIn(entityKey, this.Expiration))
            {
                Trace.WriteLine("EXIT: {0} heartbeat successful.".FormatInvariant(taskProcessorId));

                return true;
            }
            else
            {
                Trace.WriteLine("EXIT: {0} heartbeat failed.".FormatInvariant(taskProcessorId));

                return false;
            }
        }

        /// <inheritdoc />
        public Guid? GetMasterId()
        {
            Trace.WriteLine("ENTER: Getting master task processor ID ...");

            string value = this.provider.GetTextValue(RedisTaskProcessorRuntimeInfoRepository.MasterTaskProcessorIdKey);

            Guid result;

            if (Guid.TryParse(value, out result))
            {
                Trace.WriteLine("EXIT: Return master task processor ID '{0}'.".FormatInvariant(value));

                return result;
            }
            else
            {
                Trace.WriteLine("EXIT: Master task processor ID value '{0}' cannot be parsed to {1}.".FormatInvariant(value, typeof(Guid)));

                return null;
            }
        }

        /// <inheritdoc />
        public void SetMaster(Guid taskProcessorId)
        {
            Trace.WriteLine("ENTER: Setting master task processor ID to '{0}' ...".FormatInvariant(taskProcessorId));

            using (IRedisTransaction transaction = this.provider.CreateTransaction())
            {
                transaction.SetValue(RedisTaskProcessorRuntimeInfoRepository.MasterTaskProcessorIdKey, RedisConverter.ToString(taskProcessorId));

                transaction.ExpireKeyIn(RedisTaskProcessorRuntimeInfoRepository.MasterTaskProcessorIdKey, this.Expiration);

                transaction.Commit();
            }

            Trace.WriteLine("EXIT: Master task processor ID set to '{0}'.".FormatInvariant(taskProcessorId));
        }

        /// <inheritdoc />
        public bool SetMasterIfNotExists(Guid taskProcessorId)
        {
            Trace.WriteLine("ENTER: Setting master task processor ID to '{0}' if not exists ...".FormatInvariant(taskProcessorId));

            if (this.provider.SetValueIfNotExists(RedisTaskProcessorRuntimeInfoRepository.MasterTaskProcessorIdKey, RedisConverter.ToString(taskProcessorId)))
            {
                this.provider.ExpireKeyIn(RedisTaskProcessorRuntimeInfoRepository.MasterTaskProcessorIdKey, this.Expiration);

                Trace.WriteLine("EXIT: Master task processor ID set to '{0}'.".FormatInvariant(taskProcessorId));

                return true;
            }
            else
            {
                Trace.WriteLine("EXIT: Master task processor ID already exists and was not set to '{0}'.".FormatInvariant(taskProcessorId));

                return false;
            }
        }

        /// <inheritdoc />
        public void ClearMaster()
        {
            Trace.WriteLine("ENTER: Clearing master task processor ID ...");

            this.provider.RemoveKey(RedisTaskProcessorRuntimeInfoRepository.MasterTaskProcessorIdKey);

            Trace.WriteLine("EXIT: Master task processor ID cleared.");
        }

        /// <inheritdoc />
        public bool MasterHeartbeat()
        {
            Trace.WriteLine("ENTER: Master heart-beating ...");

            if (this.provider.ExpireKeyIn(RedisTaskProcessorRuntimeInfoRepository.MasterTaskProcessorIdKey, this.Expiration))
            {
                Trace.WriteLine("EXIT: Master heart-beat completed successfully.");

                return true;
            }
            else
            {
                Trace.WriteLine("EXIT: Master heart-beat failed; key is expired.");

                return false;
            }
        }

        /// <inheritdoc />
        public void Delete(Guid taskProcessorId)
        {
            string entityKey = RedisTaskProcessorRuntimeInfoRepository.GetEntityKey(taskProcessorId);

            using (IRedisTransaction transaction = this.provider.CreateTransaction())
            {
                transaction.RemoveFromSet(RedisTaskProcessorRuntimeInfoRepository.EntitySetKey, RedisConverter.ToString(taskProcessorId));

                transaction.RemoveKey(entityKey);

                transaction.Commit();
            }
        }

        #endregion ITaskProcessorRuntimeInfoRepository Members

        #region Internal Methods

        private static string GetEntityKey(Guid entityId)
        {
            return RedisTaskProcessorRuntimeInfoRepository.GetEntityKey(RedisConverter.ToString(entityId));
        }

        private static string GetEntityKey(string entityId)
        {
            return "TaskProcessor$" + entityId;
        }

        private static IReadOnlyDictionary<string, string> Serialize(ITaskProcessorRuntimeInfo taskProcessorInfo)
        {
            Dictionary<string, string> result = new Dictionary<string, string>()
            {
                { "Id", RedisConverter.ToString(taskProcessorInfo.TaskProcessorId) },
                { "MachineName", RedisConverter.ToString(taskProcessorInfo.MachineName) }
            };

            RedisTaskProcessorConfigurationRepository.Serialize(taskProcessorInfo.Configuration, result, "Configuration");

            return result;
        }

        private static ITaskProcessorRuntimeInfo Convert(IReadOnlyDictionary<string, string> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            RedisTaskProcessorRuntimeInfo result = new RedisTaskProcessorRuntimeInfo(
                RedisConverter.ParseGuid(values["Id"]),
                values["MachineName"]);

            RedisTaskProcessorConfigurationRepository.Deserialize(values, result.Configuration, "Configuration");

            return result;
        }

        #endregion Internal Methods
    }
}