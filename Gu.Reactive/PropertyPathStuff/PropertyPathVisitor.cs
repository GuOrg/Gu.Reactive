#pragma warning disable SA1600 // Elements must be documented, internal
namespace Gu.Reactive.PropertyPathStuff
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// The path expression visitor.
    /// </summary>
    [Obsolete("Don't use this.")]
    internal class PropertyPathVisitor : ExpressionVisitor
    {
        private readonly Expression expression;
        private readonly List<MemberInfo> path = new List<MemberInfo>();

        private PropertyPathVisitor(Expression expression)
        {
            this.expression = expression;
        }

        [Obsolete("Don't use this.")]
        internal static IReadOnlyList<MemberInfo> GetPath<TSource, TResult>(Expression<Func<TSource, TResult>> expression)
        {
            var visitor = new PropertyPathVisitor(expression);
            visitor.Visit(expression);
            visitor.path.Reverse();
            return visitor.path;
        }

        [Obsolete("Don't use this.")]
        internal static IReadOnlyList<MemberInfo> GetPath<T>(Expression<Func<T>> expression)
        {
            var visitor = new PropertyPathVisitor(expression);
            visitor.Visit(expression);
            visitor.path.Reverse();
            return visitor.path;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            this.path.Add(node.Member);
            return base.VisitMember(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override Expression VisitDefault(DefaultExpression node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override Expression VisitDynamic(DynamicExpression node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override ElementInit VisitElementInit(ElementInit node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override Expression VisitExtension(Expression node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override Expression VisitGoto(GotoExpression node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override Expression VisitIndex(IndexExpression node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override Expression VisitInvocation(InvocationExpression node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override Expression VisitLabel(LabelExpression node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override LabelTarget VisitLabelTarget(LabelTarget node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override Expression VisitListInit(ListInitExpression node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override Expression VisitLoop(LoopExpression node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override MemberBinding VisitMemberBinding(MemberBinding node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            this.path.Add(node.Method);
            return base.VisitMethodCall(node);
        }

        protected override Expression VisitNew(NewExpression node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override Expression VisitSwitch(SwitchExpression node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override SwitchCase VisitSwitchCase(SwitchCase node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override Expression VisitTry(TryExpression node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            throw new ArgumentException($"Expecxting a path like x.Property.Value was {this.expression}", nameof(node));
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            return base.VisitUnary(node);
        }
    }
}
