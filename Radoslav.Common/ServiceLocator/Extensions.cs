using System;
using System.Collections.Generic;
using System.Linq;

namespace Radoslav.ServiceLocator
{
    /// <summary>
    /// Radoslav service locator extensions methods.
    /// </summary>
    public static class ServiceLocatorExtensions
    {
        /// <summary>
        /// Checks if a the service locator can resolve a specified contract type.
        /// </summary>
        /// <typeparam name="TContract">The contract type to check.</typeparam>
        /// <param name="locator">The service locator.</param>
        /// <returns>True if the service locator can resolve the contract type; otherwise false.</returns>
        /// <exception cref="NullReferenceException">Parameter <paramref name="locator"/> is null.</exception>
        public static bool CanResolve<TContract>(this IRadoslavServiceLocator locator)
        {
            if (locator == null)
            {
                throw new NullReferenceException();
            }

            return locator.CanResolve(typeof(TContract));
        }

        /// <summary>
        /// Returns an implementation of the specified contract.
        /// </summary>
        /// <typeparam name="TContract">The contract type to resolve.</typeparam>
        /// <param name="locator">The service locator.</param>
        /// <returns>An implementation of the specified contract.</returns>
        /// <exception cref="NullReferenceException">Parameter <paramref name="locator"/> is null.</exception>
        /// <exception cref="ArgumentException">The specified contract type is unknown for
        /// the service locator, or there are multiple implementations defined for contractType.</exception>
        public static TContract ResolveSingle<TContract>(this IRadoslavServiceLocator locator)
        {
            if (locator == null)
            {
                throw new NullReferenceException();
            }

            return (TContract)locator.ResolveSingle(typeof(TContract));
        }

        /// <summary>
        /// Returns an implementation of the specified contract.
        /// </summary>
        /// <typeparam name="TContract">The type to which to convert the resolved implementation.</typeparam>
        /// <param name="locator">The service locator.</param>
        /// <param name="contractType">The contract type to resolve.</param>
        /// <returns>An implementation of the specified contract.</returns>
        /// <exception cref="NullReferenceException">Parameter <paramref name="locator"/> is null.</exception>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="contractType"/> is null.</exception>
        /// <exception cref="ArgumentException">Type <typeparamref name="TContract"/> is not assignable from parameter <paramref name="contractType"/>,
        /// or the specified contract type is unknown for the service locator,
        /// or there are multiple implementations defined for contractType.</exception>
        public static TContract ResolveSingle<TContract>(this IRadoslavServiceLocator locator, Type contractType)
        {
            if (locator == null)
            {
                throw new NullReferenceException();
            }

            if (contractType == null)
            {
                throw new ArgumentNullException(nameof(contractType));
            }

            if (!typeof(TContract).IsAssignableFrom(contractType))
            {
                throw new ArgumentException("Type '{0}' is not assignable from '{1}'.".FormatInvariant(typeof(TContract), contractType), nameof(contractType));
            }

            return (TContract)locator.ResolveSingle(contractType);
        }

        /// <summary>
        /// Returns all implementations of a specified contract.
        /// </summary>
        /// <typeparam name="TContract">The contract type to resolve.</typeparam>
        /// <param name="locator">The service locator.</param>
        /// <returns>All implementations of the specified contract, or empty collection if
        /// there are no implementation of the specified contract.</returns>
        /// <exception cref="NullReferenceException">Parameter <paramref name="locator"/> is null.</exception>
        public static IEnumerable<TContract> ResolveMultiple<TContract>(this IRadoslavServiceLocator locator)
        {
            if (locator == null)
            {
                throw new NullReferenceException();
            }

            return locator.ResolveMultiple(typeof(TContract)).Cast<TContract>();
        }
    }
}