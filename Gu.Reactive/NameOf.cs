namespace Gu.Reactive
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Gu.Reactive.PropertyPathStuff;

    /// <summary>
    /// Class provides methods to obtain member names of data types.
    /// </summary>
    [Obsolete("To be removed.")]
    public static class NameOf
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
        /// Throw an exception if the provided path is a multi level path (e.g. a.b)
        /// </param>
        /// <returns>
        /// Returns the simple name of the property.
        /// </returns>
        [Obsolete("Use nameof instead")]
        //// ReSharper disable once UnusedParameter.Global
        public static string Property<T>(Expression<Func<T>> property, bool allowNestedProperty = false)
        {
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
        /// Type of the item
        /// </typeparam>
        /// <param name="property">
        /// Property expression on the the form () =&gt; Instance.Property.
        /// </param>
        /// <returns>
        /// Returns the simple name of the property.
        /// </returns>
        [Obsolete("Use nameof instead")]
        public static string Property<TSource>(Expression<Func<TSource, object>> property)
        {
            var path = PropertyPathParser.GetPath(property);
            return path[path.Count - 1].Name;
        }

        /// <summary>
        /// Returns the name of a property provided as a property expression.
        /// </summary>
        /// <typeparam name="TItem">
        /// Type of the item
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
        [Obsolete("Use nameof instead")]
        public static string Property<TItem, TValue>(Expression<Func<TItem, TValue>> property)
        {
            var path = PropertyPathParser.GetPath(property);
            return path[path.Count - 1].Name;
        }

        /// <summary>
        /// Get the name of a method.
        /// </summary>
        /// <param name="method">An expression specifying a method.</param>
        /// <returns> The name of the method specified by <paramref name="method"/>.</returns>
        [Obsolete("Use nameof instead")]
        public static string Method(Expression<Action> method)
        {
            return ((MethodCallExpression)method.Body).Method.Name;
        }

        /// <summary>
        /// Get the name of a method.
        /// </summary>
        /// <typeparam name="TSource">The type containing the method.</typeparam>
        /// <param name="method">An expression specifying a method.</param>
        /// <returns> The name of the method specified by <paramref name="method"/>.</returns>
        [Obsolete("Use nameof instead")]
        public static string Method<TSource>(Expression<Action<TSource>> method)
        {
            return ((MethodCallExpression)method.Body).Method.Name;
        }

        /// <summary>
        /// Get the name of a method.
        /// </summary>
        /// <typeparam name="TResult">The type of the returnvalue of the method.</typeparam>
        /// <param name="method">An expression specifying a method.</param>
        /// <returns> The name of the method specified by <paramref name="method"/>.</returns>
        [Obsolete("Use nameof instead")]
        public static string Method<TResult>(Expression<Func<TResult>> method)
        {
            return ((MethodCallExpression)method.Body).Method.Name;
        }

        /// <summary>
        /// Get the name of a method.
        /// </summary>
        /// <typeparam name="TClass">The type containing the method.</typeparam>
        /// <typeparam name="TReturnValue">The type of the returnvalue of the method.</typeparam>
        /// <param name="method">An expression specifying a method.</param>
        /// <returns> The name of the method specified by <paramref name="method"/>.</returns>
        [Obsolete("Use nameof instead", error: true)]
        public static string Method<TClass, TReturnValue>(Expression<Func<TClass, TReturnValue>> method)
        {
            return ((MethodCallExpression)method.Body).Method.Name;
        }

        /// <summary>
        /// Get the name of the arguments of a method.
        /// </summary>
        /// <typeparam name="T">The type of the returnvalue of the method.</typeparam>
        /// <param name="method">An expression specifying a method.</param>
        /// <returns> The name of the arguments of the method specified by <paramref name="method"/>.</returns>
        [Obsolete("To be removed.")]
        public static Argument[] Arguments<T>(Expression<Func<T>> method)
        {
            var methodCallExpression = (MethodCallExpression)method.Body;
            return Arguments(methodCallExpression);
        }

        /// <summary>
        /// Get the name of the arguments of a method.
        /// </summary>
        /// <param name="method">An expression specifying a method.</param>
        /// <returns> The name of the arguments of the method specified by <paramref name="method"/>.</returns>
        [Obsolete("To be removed.")]
        public static Argument[] Arguments(Expression<Action> method)
        {
            var methodCallExpression = (MethodCallExpression)method.Body;
            return Arguments(methodCallExpression);
        }

        /// <summary>
        /// Get the name of the arguments of a method.
        /// </summary>
        /// <param name="method">An expression specifying a method.</param>
        /// <returns> The name of the arguments of the method specified by <paramref name="method"/>.</returns>
        [Obsolete("To be removed.")]
        public static Argument[] Arguments(MethodCallExpression method)
        {
            var names = method.Method.GetParameters()
                       .Select(x => x.Name)
                       .ToArray();
            var values = method.Arguments
                               .Select(GetValue)
                               .ToArray();
            return names.Zip(values, (s, o) => new Argument(s, o))
                        .ToArray();
        }

        [Obsolete("To be removed.")]
        private static object GetValue(MemberExpression e)
        {
            var memberExpression = e.Expression as MemberExpression;
            if (memberExpression != null)
            {
                return GetValue(memberExpression);
            }

            var container = ((ConstantExpression)e.Expression).Value;
            var fieldInfo = e.Member as FieldInfo;

            if (fieldInfo != null)
            {
                return fieldInfo.GetValue(container);
            }

            var propertyInfo = e.Member as PropertyInfo;
            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(container);
            }

            throw new ArgumentException("Failed to get value", nameof(e));
        }

        private static object GetValue(Expression e)
        {
            var me = e as MemberExpression;
            if (me != null)
            {
                return GetValue(me);
            }

            var ce = e as ConstantExpression;
            if (ce != null)
            {
                return ce.Value;
            }

            throw new NotImplementedException("message");
        }

        /// <summary>The argument.</summary>
        public class Argument
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Argument"/> class.
            /// </summary>
            /// <param name="name">
            /// The name.
            /// </param>
            /// <param name="value">
            /// The value.
            /// </param>
            public Argument(string name, object value)
            {
                this.Name = name;
                this.Value = value;
            }

            /// <summary>
            /// Gets the name.
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Gets the value.
            /// </summary>
            public object Value { get; }
        }
    }
}
