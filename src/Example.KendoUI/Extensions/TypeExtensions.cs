using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.KendoUI.Extensions
{
    /// <summary>
    /// <see cref="TypeExtensions"/> static class, provides extension methods for <see cref="Type"/> objects.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Get the default value for the specified type.
        /// </summary>
        /// <param name="type">Type you want the default value for.</param>
        /// <returns>Default value for the specified type.</returns>
        public static object GetDefaultValue(this Type type)
        {
            if (type == typeof(string))
                return null;
            else if (type.IsValueType)
                return Activator.CreateInstance(type);

            return null;
        }

        /// <summary>
        /// Get the IEnumerable generic value type.
        /// </summary>
        /// <param name="type">Type object.</param>
        /// <returns>IEnumerable of Type.</returns>
        public static IEnumerable<Type> GetGenericIEnumerableTypes(this Type type)
        {
            return type.GetInterfaces().Where(t => t.IsGenericType == true && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)).Select(t => t.GetGenericArguments()[0]);
        }

        /// <summary>
        /// Determine whether the specified type is nullable.
        /// </summary>
        /// <param name="type">Type object.</param>
        /// <returns>True if the type is nullable.</returns>
        public static bool IsNullable(this Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// Determine whether the specified type is of IEnumerable.
        /// </summary>
        /// <param name="type">Type object.</param>
        /// <returns>True if the type is of type IEnumerable.</returns>
        public static bool IsIEnumerable(this Type type)
        {
            return (type != typeof(string) && type.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)));
        }

        /// <summary>
        /// Get the specified custom <see cref="Attribute"/> for the specified <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">The attribute type.</typeparam>
        /// <param name="type">The object type you want the attribute for.</param>
        /// <param name="inherit">Whether to include inherited attributes.</param>
        /// <returns>The attribute of the specified type.</returns>
        public static T GetCustomAttribute<T>(this Type type, bool inherited = false) where T : Attribute
        {
            return (T)type.GetCustomAttributes(typeof(T), inherited).FirstOrDefault();
        }
    }
}
