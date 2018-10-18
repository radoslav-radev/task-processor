using System;
using System.Collections;

namespace Radoslav.ServiceLocator
{
    /// <summary>
    /// Basic functionality of a Radoslav service locator.
    /// </summary>
    public interface IRadoslavServiceLocator
    {
        /// <summary>
        /// Checks if a the service locator can resolve a specified contract type.
        /// </summary>
        /// <param name="contractType">The contract type to check.</param>
        /// <returns>True if the service locator can resolve the contract type; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="contractType"/> is null.</exception>
        bool CanResolve(Type contractType);

        /// <summary>
        /// Returns an implementation of the specified contract.
        /// </summary>
        /// <param name="contractType">The contract type to resolve.</param>
        /// <returns>An implementation of the specified contract.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="contractType"/> is null.</exception>
        /// <exception cref="ArgumentException">The specified contract type is unknown for the service locator,
        /// or there are multiple implementations defined for the contract type.</exception>
        object ResolveSingle(Type contractType);

        /// <summary>
        /// Returns all implementations of a specified contract.
        /// </summary>
        /// <param name="contractType">The contract type to resolve.</param>
        /// <returns>All implementations of the specified contract, or empty collection if
        /// there are no implementation of the specified contract.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="contractType"/> is null.</exception>
        IEnumerable ResolveMultiple(Type contractType);
    }
}