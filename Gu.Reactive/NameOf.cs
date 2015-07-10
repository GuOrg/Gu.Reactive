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
    public static class NameOf
    {
        /// <summary>
        /// Returns the name of a property provided as a property expression.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the property.
        /// </typeparam>
        /// <param name="propertyExpression">
        /// Property expression on the the form () =&gt; Instance.Property.
        /// </param>
        /// <param name="allowNestedProperty">
        /// Throw an exception if the provided path is a multi level path (e.g. a.b)
        /// </param>
        /// <returns>
        /// Returns the simple name of the property.
        /// </returns>
        public static string Property<T>(Expression<Func<T>> propertyExpression, bool allowNestedProperty = false)
        {
            var path = PropertyPathVisitor.GetPath(propertyExpression);

            if (path.Count > 1 && !allowNestedProperty)
            {
                throw new ArgumentException("Trying to get the name of a nested property: " + string.Join(".", path.Select(x => x.Name)));
            }

            var memberInfo = path.Last();
            if (!(memberInfo is PropertyInfo))
            {
                throw new ArgumentException("The expression is for a method", "propertyExpression");
            }
            return memberInfo.Name;
        }

        /// <summary>
        /// Returns the name of a property provided as a property expression.
        /// </summary>
        /// <typeparam name="TProperty">
        /// Type of the property.
        /// </typeparam>
        /// <typeparam name="TItem">
        /// Type of the item
        /// </typeparam>
        /// <param name="propertyExpression">
        /// Property expression on the the form () =&gt; Instance.Property.
        /// </param>
        /// <returns>
        /// Returns the simple name of the property.
        /// </returns>
        public static string Property<TItem>(Expression<Func<TItem, object>> propertyExpression)
        {
            var path = PropertyPathVisitor.GetPath(propertyExpression);
            var memberInfo = path.Last();
            if (!(memberInfo is PropertyInfo))
            {
                throw new ArgumentException("The expression is for a method", "propertyExpression");
            }
            return memberInfo.Name;
        }

        /// <summary>
        /// Returns the name of a property provided as a property expression.
        /// </summary>
        /// <typeparam name="TValue">
        /// Type of the property.
        /// </typeparam>
        /// <typeparam name="TItem">
        /// Type of the item
        /// </typeparam>
        /// <param name="propertyExpression">
        /// Property expression on the the form () =&gt; Instance.Property.
        /// </param>
        /// <returns>
        /// Returns the simple name of the property.
        /// </returns>
        public static string Property<TItem, TValue>(Expression<Func<TItem, TValue>> propertyExpression)
        {
            var path = PropertyPathVisitor.GetPath(propertyExpression);
            var memberInfo = path.Last();
            if (!(memberInfo is PropertyInfo))
            {
                throw new ArgumentException("The expression is for a method", "propertyExpression");
            }
            return memberInfo.Name;
        }

        /// <summary>
        /// The method.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Method(Expression<Action> action)
        {
            return ((MethodCallExpression)action.Body).Method.Name;
        }

        /// <summary>
        /// The method.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Method<T>(Expression<Action<T>> action)
        {
            return ((MethodCallExpression)action.Body).Method.Name;
        }

        /// <summary>
        /// The method.
        /// </summary>
        /// <param name="func">
        /// The func.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Method<T>(Expression<Func<T>> func)
        {
            return ((MethodCallExpression)func.Body).Method.Name;
        }

        /// <summary>
        /// The method.
        /// </summary>
        /// <param name="func">
        /// The func.
        /// </param>
        /// <typeparam name="TClass">
        /// </typeparam>
        /// <typeparam name="TReturnValue">
        /// </typeparam>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Method<TClass, TReturnValue>(Expression<Func<TClass, TReturnValue>> func)
        {
            return ((MethodCallExpression)func.Body).Method.Name;
        }

        /// <summary>
        /// The arguments.
        /// </summary>
        /// <param name="func">
        /// The func.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="Argument[]"/>.
        /// </returns>
        public static Argument[] Arguments<T>(Expression<Func<T>> func)
        {
            var method = (MethodCallExpression)func.Body;
            return Arguments(method);
        }

        /// <summary>
        /// The arguments.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <returns>
        /// The <see cref="Argument[]"/>.
        /// </returns>
        public static Argument[] Arguments(Expression<Action> action)
        {
            var method = (MethodCallExpression)action.Body;
            return Arguments(method);
        }

        /// <summary>
        /// The arguments.
        /// </summary>
        /// <param name="method">
        /// The method.
        /// </param>
        /// <returns>
        /// The <see cref="Argument[]"/>.
        /// </returns>
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

        /// <summary>
        /// The get value.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        public static object GetValue(MemberExpression e)
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

            throw new ArgumentException("Failed to get value", "e");
        }

        /// <summary>
        /// The get value.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
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

        /// <summary>
        /// The argument.
        /// </summary>
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
                Name = name;
                Value = value;
            }

            /// <summary>
            /// Gets the name.
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// Gets the value.
            /// </summary>
            public object Value { get; private set; }
        }
    }
}
