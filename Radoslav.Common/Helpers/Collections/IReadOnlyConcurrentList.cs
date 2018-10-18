using System;
using System.Collections.Generic;

namespace Radoslav.Collections
{
    /// <summary>
    /// Defines basic functionality of a read-only concurrent list.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the list.</typeparam>
    public interface IReadOnlyConcurrentList<T> : IReadOnlyList<T>
    {
        /// <summary>
        /// Executes a callback method with a read lock on the collection.
        /// </summary>
        /// <param name="callback">The callback to be executed with read lock.</param>
        void ExecuteWithReadLock(Action callback);
    }
}