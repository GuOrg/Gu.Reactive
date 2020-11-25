namespace Gu.Reactive
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Gu.Reactive.Internals;

    /// <summary>
    /// Class provides methods to obtain member names of data types.
    /// </summary>
    public static partial class NameOf
    {
        /// <summary>
        /// Returns the name of a property provided as a property expression.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the property.
        /// </typeparam>
        /// <param name="property">
        /// Property expression on the the form () =&gt; Instance.Property.
        /// </param>
        /// <param name="allowNestedProperty">
        /// Throw an exception if the provided path is a multi level path (e.g. a.b).
        /// </param>
        /// <returns>
        /// Returns the simple name of the property.
        /// </returns>
        public static string Property<T>(Expression<Func<T>> property, bool allowNestedProperty = false)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            var path = PropertyPathParser.GetPath(property);

            if (path.Count > 1 && !allowNestedProperty)
            {
                throw new ArgumentException("Trying to get the name of a nested property: " + string.Join(".", path.Select(x => x.Name)));
            }

            return path[path.Count - 1].Name;
        }

        /// <summary>
        /// Returns the name of a property provided as a property expression.
        /// </summary>
        /// <typeparam name="TSource">
        /// Type of the item.
        /// </typeparam>
        /// <param name="property">
        /// Property expression on the the form () =&gt; Instance.Property.
        /// </param>
        /// <returns>
        /// Returns the simple name of the property.
        /// </returns>
        public static string Property<TSource>(Expression<Func<TSource, object?>> property)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            var path = PropertyPathParser.GetPath(property);
            return path[path.Count - 1].Name;
        }

        /// <summary>
        /// Returns the name of a property provided as a property expression.
        /// </summary>
        /// <typeparam name="TItem">
        /// Type of the item.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// Type of the property.
        /// </typeparam>
        /// <param name="property">
        /// Property expression on the the form () =&gt; Instance.Property.
        /// </param>
        /// <returns>
        /// Returns the simple name of the property.
        /// </returns>
        public static string Property<TItem, TValue>(Expression<Func<TItem, TValue>> property)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            var path = PropertyPathParser.GetPath(property);
            return path[path.Count - 1].Name;
        }
    }
}
