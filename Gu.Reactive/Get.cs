namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;

    public static class Get
    {
        public static TValue ValueOrDefault<TSource, TValue>(
            this TSource source,
            Expression<Func<TSource, TValue>> path,
            TValue @default = default (TValue))
        {
            var valuePath = ValueVisitor<TSource, TValue>.Create(path);
            valuePath.CacheValue();
            if (!valuePath.HasValue(source))
            {
                return @default;
            }
            return valuePath.Value(source);
        }

        public static TValue ValueOrDefault<TValue>(Expression<Func<TValue>> path, TValue @default = default (TValue))
        {
            //var valuePath = ValueVisitor<object, TValue>.Create(path);
            //valuePath.CacheValue();
            //if (!valuePath.HasValue(source))
            //{
            //    return @default;
            //}
            //return valuePath.Value(source);
            throw new NotImplementedException("message");
        }

        private class PathItem<TSource, TValue>
            where TValue : class
        {
            private readonly Func<TSource, TValue> _func;

            public PathItem(Func<TSource, TValue> func)
            {
                _func = func;
            }

            public bool HasValue(TSource source)
            {
                return Value(source) != null;
            }

            public TValue Value(TSource source)
            {
                return _func(source);
            }
        }

        private class StructItem<TSource, TValue>
            where TValue : struct
        {
            private readonly Func<TSource, TValue> _func;

            public StructItem(Func<TSource, TValue> func)
            {
                _func = func;
            }

            public bool HasValue(TSource source)
            {
                return true;
            }

            public TValue Value(TSource source)
            {
                return _func(source);
            }
        }

        private class Path<TSource, TValue>
        {
            public bool HasValue(TSource source)
            {
                return Value(source) != null;
            }

            public TValue Value(TSource source)
            {
                throw new NotImplementedException("message");

                //return _func(source);
            }

            public void CacheValue()
            {
                throw new NotImplementedException();
            }

            internal void AddNode()
            {

            }
        }

        private class ValueVisitor<TSource, TValue> : ExpressionVisitor
        {
            private readonly Path<TSource, TValue> _path;

            private ValueVisitor(Path<TSource, TValue> path)
            {
                _path = path;
            }

            public static Path<TSource, TValue> Create<TSource, TValue>(Expression<Func<TSource, TValue>> path)
            {
                var visitor = new ValueVisitor<TSource, TValue>(new Path<TSource, TValue>());
                visitor.Visit(path);
                return visitor._path;
            }

            public override Expression Visit(Expression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitBlock(BlockExpression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override CatchBlock VisitCatchBlock(CatchBlock node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitConditional(ConditionalExpression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitDebugInfo(DebugInfoExpression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitDefault(DefaultExpression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitDynamic(DynamicExpression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override ElementInit VisitElementInit(ElementInit node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitExtension(Expression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitGoto(GotoExpression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitIndex(IndexExpression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitInvocation(InvocationExpression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitLabel(LabelExpression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override LabelTarget VisitLabelTarget(LabelTarget node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitListInit(ListInitExpression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitLoop(LoopExpression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override MemberBinding VisitMemberBinding(MemberBinding node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitMemberInit(MemberInitExpression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitNew(NewExpression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitNewArray(NewArrayExpression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitSwitch(SwitchExpression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override SwitchCase VisitSwitchCase(SwitchCase node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitTry(TryExpression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitTypeBinary(TypeBinaryExpression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }

            protected override Expression VisitUnary(UnaryExpression node)
            {
                throw new InvalidOperationException("Expecxting a path like x => x.Property.Method().Value");
            }
        }
    }
}
