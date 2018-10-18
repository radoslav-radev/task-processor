using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Radoslav.Retryable.DelayStrategy;

namespace Radoslav.UnitTests
{
    public sealed class FakeServiceLocatorObject : IFakeServiceLocatorObject
    {
        private readonly Collection<IDelayStrategy> strategies = new Collection<IDelayStrategy>();
        private readonly List<Type> supportedTypes = new List<Type>();

        public ICollection<IDelayStrategy> DelayStrategies
        {
            get { return this.strategies; }
        }

        public ICollection<Type> SupportedTypes
        {
            get { return this.supportedTypes; }
        }
    }
}