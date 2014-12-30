// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PathExpressionVisitor.cs" company="">
//   
// </copyright>
// <summary>
//   The path expression visitor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// The path expression visitor.
    /// </summary>
    public class PathExpressionVisitor : ExpressionVisitor
    {
        /// <summary>
        /// The _path.
        /// </summary>
        private readonly List<MemberExpression> _path = new List<MemberExpression>();

        /// <summary>
        /// The get path.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <typeparam name="TSource">
        /// </typeparam>
        /// <typeparam name="TResult">
        /// </typeparam>
        /// <returns>
        /// The <see cref="MemberExpression[]"/>.
        /// </returns>
        public static MemberExpression[] GetPath<TSource, TResult>(Expression<Func<TSource, TResult>> expression)
        {
            var visitor = new PathExpressionVisitor();
            visitor.Visit(expression.Body);
            return Enumerable.Reverse(visitor._path).ToArray();
        }

        /// <summary>
        /// The get path.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="MemberExpression[]"/>.
        /// </returns>
        public static MemberExpression[] GetPath<T>(Expression<Func<T>> expression)
        {
            var visitor = new PathExpressionVisitor();
            visitor.Visit(expression.Body);
            return Enumerable.Reverse(visitor._path).ToArray();
        }

        /// <summary>
        /// The visit member.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            _path.Add(node);
            return base.VisitMember(node);
        }
    }
}
