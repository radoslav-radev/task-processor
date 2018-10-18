using System;
using System.Collections.Generic;

namespace Radoslav
{
    public interface IEqualityComparerFactory
    {
        bool IsSupported(Type type);

        IEqualityComparer<T> GetEqualityComparer<T>();
    }
}