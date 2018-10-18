using System;
using System.Collections;
using System.Collections.Generic;
using Radoslav.ServiceLocator;

namespace Radoslav
{
    public sealed class FakeServiceLocator : MockObject, IRadoslavServiceLocator
    {
        private readonly List<object> createdObjects = new List<object>();

        public IEnumerable<object> CreatedObjects
        {
            get { return this.createdObjects; }
        }

        #region IRadoslavServiceLocator Members

        public bool CanResolve(Type contractType)
        {
            this.RecordMethodCall(contractType);

            return this.GetPredefinedResultOrDefault<bool>(contractType);
        }

        public object ResolveSingle(Type contractType)
        {
            this.RecordMethodCall(contractType);

            return this.GetPredefinedResultOrDefault<object>(contractType);
        }

        public object ResolveSingle(Type contractType, string key)
        {
            this.RecordMethodCall(contractType, key);

            throw new NotImplementedException();
        }

        public IEnumerable ResolveMultiple(Type contractType)
        {
            this.RecordMethodCall(contractType);

            return this.GetPredefinedResultOrDefault<IEnumerable>(contractType);
        }

        #endregion IRadoslavServiceLocator Members
    }
}