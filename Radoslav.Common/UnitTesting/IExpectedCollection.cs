using System.Collections;

namespace Radoslav
{
    public interface IExpectedCollection : IEnumerable
    {
        bool IsOrdered { get; }
    }
}