using System;
using Radoslav.TaskProcessor.Configuration;

namespace Radoslav.TaskProcessor.UnitTests
{
    internal sealed class FakeSerializationConfiguration : MockObject, ITaskProcessorSerializationConfiguration
    {
        #region ITaskProcessorSerializationConfiguration Members

        public Type GetSerializerType(Type entityType)
        {
            return this.GetPredefinedResultOrDefault<Type>(entityType);
        }

        #endregion ITaskProcessorSerializationConfiguration Members
    }
}