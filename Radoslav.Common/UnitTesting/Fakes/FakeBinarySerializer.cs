using System;
using Radoslav.Serialization;

namespace Radoslav
{
    public sealed class FakeBinarySerializer : MockObject, IEntityBinarySerializer
    {
        private readonly EntityBinarySerializer serializer = new EntityBinarySerializer();

        #region IEntityBinarySerializer Members

        public bool CanDetermineEntityTypeFromContent { get; set; }

        public byte[] Serialize(object entity)
        {
            this.RecordMethodCall(entity);

            return this.serializer.Serialize(entity);
        }

        public object Deserialize(byte[] content)
        {
            this.RecordMethodCall(content);

            return this.serializer.Deserialize(content);
        }

        public object Deserialize(byte[] content, Type entityType)
        {
            this.RecordMethodCall(content, entityType);

            return this.serializer.Deserialize(content);
        }

        #endregion IEntityBinarySerializer Members
    }
}