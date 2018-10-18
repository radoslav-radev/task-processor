using System;
using System.Collections.Generic;

namespace Radoslav
{
    public sealed class CallbackEqualityComparerFactory : IEqualityComparerFactory
    {
        private readonly Dictionary<Type, object> callbacks = new Dictionary<Type, object>();

        public void RegisterCallback<T>(Func<T, T, bool> callback)
        {
            this.RegisterCallback<T>(callback, false);
        }

        public void RegisterCallback<T>(Func<T, T, bool> callback, bool callCallbackForNullValues)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            if (this.callbacks.ContainsKey(typeof(T)))
            {
                throw new ArgumentException("Equality comparer callback for type {0} is already registered.".FormatInvariant(typeof(T)), "callback");
            }

            this.callbacks.Add(typeof(T), new CallbackEqualityComparer<T>(callback, callCallbackForNullValues));
        }

        #region IEqualityComparerFactory Members

        public bool IsSupported(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return this.callbacks.ContainsKey(type);
        }

        public IEqualityComparer<T> GetEqualityComparer<T>()
        {
            object result;

            if (this.callbacks.TryGetValue(typeof(T), out result))
            {
                return (IEqualityComparer<T>)result;
            }
            else
            {
                return new DefaultEqualityComparer<T>();
            }
        }

        #endregion IEqualityComparerFactory Members
    }
}