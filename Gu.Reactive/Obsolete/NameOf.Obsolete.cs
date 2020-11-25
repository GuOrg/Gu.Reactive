namespace Gu.Reactive
{
    using System;
    using System.Linq.Expressions;

    public static partial class NameOf
    {

        /// <summary>
        /// Get the name of a method.
        /// </summary>
        /// <param name="method">An expression specifying a method.</param>
        /// <returns> The name of the method specified by <paramref name="method"/>.</returns>
        [Obsolete("This will be removed in future version.")]
        public static string Method(Expression<Action> method)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            return ((MethodCallExpression)method.Body).Method.Name;
        }

        /// <summary>
        /// Get the name of a method.
        /// </summary>
        /// <typeparam name="TSource">The type containing the method.</typeparam>
        /// <param name="method">An expression specifying a method.</param>
        /// <returns> The name of the method specified by <paramref name="method"/>.</returns>
        [Obsolete("This will be removed in future version.")]
        public static string Method<TSource>(Expression<Action<TSource>> method)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            return ((MethodCallExpression)method.Body).Method.Name;
        }

        /// <summary>
        /// Get the name of a method.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value of the method.</typeparam>
        /// <param name="method">An expression specifying a method.</param>
        /// <returns> The name of the method specified by <paramref name="method"/>.</returns>
        [Obsolete("This will be removed in future version.")]
        public static string Method<TResult>(Expression<Func<TResult>> method)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            return ((MethodCallExpression)method.Body).Method.Name;
        }

        /// <summary>
        /// Get the name of a method.
        /// </summary>
        /// <typeparam name="TClass">The type containing the method.</typeparam>
        /// <typeparam name="TReturnValue">The type of the return value of the method.</typeparam>
        /// <param name="method">An expression specifying a method.</param>
        /// <returns> The name of the method specified by <paramref name="method"/>.</returns>
        [Obsolete("This will be removed in future version.")]
        public static string Method<TClass, TReturnValue>(Expression<Func<TClass, TReturnValue>> method)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            return ((MethodCallExpression)method.Body).Method.Name;
        }
    }
}
