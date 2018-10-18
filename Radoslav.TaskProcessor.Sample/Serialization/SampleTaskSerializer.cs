using System;
using System.Linq;
using System.Text;
using Radoslav.Serialization;

namespace Radoslav.TaskProcessor.Sample
{
    public sealed class SampleTaskSerializer : IEntityBinarySerializer
    {
        #region ITaskSerializer Members

        public bool CanDetermineEntityTypeFromContent
        {
            get
            {
                // Because we handle only SampleTask and we are always sure the content is for SampleTask.
                // If we support more than one task type in this serializer, we should return false.
                return true;
            }
        }

        public byte[] Serialize(object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            SampleTask sampleTask = entity as SampleTask;

            if (sampleTask == null)
            {
                throw new NotSupportedException(string.Format("Type '{0}' is not supported.", entity.GetType().Name));
            }

            if (sampleTask.Details == null)
            {
                return new byte[0];
            }

            return Encoding.UTF8.GetBytes(string.Join(",", sampleTask.Details.Select(detail => detail.DurationInSeconds)));
        }

        public object Deserialize(byte[] content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            SampleTask result = new SampleTask();

            result.Details = Encoding.UTF8.GetString(content)
                .Split(',')
                .Select(value => new SampleTaskDetail() { DurationInSeconds = int.Parse(value) })
                .ToArray();

            return result;
        }

        public object Deserialize(byte[] content, Type taskType)
        {
            return this.Deserialize(content);
        }

        #endregion ITaskSerializer Members
    }
}