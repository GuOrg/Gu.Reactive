namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class ConstantVisitor : ExpressionVisitor
    {
        /// <summary>
        /// The _path.
        /// </summary>
        private readonly List<ConstantExpression> constants = new List<ConstantExpression>();

        private ConstantVisitor()
        {
        }

        public static IReadOnlyList<ConstantExpression> GetConstants<T>(Expression<Func<T>> e)
        {
            var visitor = new ConstantVisitor();
            visitor.Visit(e);
            var constants = visitor.constants;
            return constants;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            this.constants.Add(node);
            return base.VisitConstant(node);
        }
    }
}