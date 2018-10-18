using System;
using System.Reflection;

namespace Radoslav
{
    public static partial class Helpers
    {
        /// <summary>
        /// Creates an instance of a given type and returns it as a specified type.
        /// </summary>
        /// <typeparam name="T">The type to return the created instance.</typeparam>
        /// <param name="type">The type to instantiate.</param>
        /// <returns>An instance of the specified type.</returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentException">Parameter <paramref name="type"/> has no parameterless constructor.</exception>
        public static T CreateInstance<T>(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return (T)type.CreateInstance();
        }

        private static object CreateInstance(this Type type)
        {
            ConstructorInfo defaultConstructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);

            if (defaultConstructor == null)
            {
                throw new ArgumentException("Type '{0}' has no parameterless constructor.".FormatInvariant(type), "type");
            }

            return defaultConstructor.Invoke(null);
        }
    }
}