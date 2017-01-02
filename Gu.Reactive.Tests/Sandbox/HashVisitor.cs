namespace Gu.Reactive.Tests.Sandbox
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    internal sealed class HashVisitor : ExpressionVisitor
    {
        private const int NullHashCode = 0x61E04917;
        private int hash;

        private HashVisitor()
        {
        }

        public static int GetHash(Expression<Func<object, string>> func)
        {
            return GetHash((Expression)func);
        }

        internal static int GetHash(Expression e)
        {
            var hashVisitor = new HashVisitor();
            hashVisitor.Visit(e);
            return hashVisitor.Hash;
        }

        private int Hash => this.hash;

        private void Reset()
        {
            this.hash = 0;
        }

        private void UpdateHash(int value)
        {
            this.hash = (this.hash * 397) ^ value;
        }

        private void UpdateHash(object component)
        {
            int componentHash;

            if (component == null)
            {
                componentHash = NullHashCode;
            }
            else
            {
                var member = component as MemberInfo;
                if (member != null)
                {
                    componentHash = member.Name.GetHashCode();

                    var declaringType = member.DeclaringType;
                    if (declaringType?.AssemblyQualifiedName != null)
                    {
                        componentHash = (componentHash * 397) ^ declaringType.AssemblyQualifiedName.GetHashCode();
                    }
                }
                else
                {
                    componentHash = component.GetHashCode();
                }
            }

            this.hash = (this.hash * 397) ^ componentHash;
        }

        public override Expression Visit(Expression node)
        {
            this.UpdateHash((int)node.NodeType);
            return base.Visit(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            this.UpdateHash(node.Value);
            return base.VisitConstant(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            this.UpdateHash(node.Member);
            return base.VisitMember(node);
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            this.UpdateHash(node.Member);
            return base.VisitMemberAssignment(node);
        }

        protected override MemberBinding VisitMemberBinding(MemberBinding node)
        {
            this.UpdateHash((int)node.BindingType);
            this.UpdateHash(node.Member);
            return base.VisitMemberBinding(node);
        }

        protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            this.UpdateHash((int)node.BindingType);
            this.UpdateHash(node.Member);
            return base.VisitMemberListBinding(node);
        }

        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            this.UpdateHash((int)node.BindingType);
            this.UpdateHash(node.Member);
            return base.VisitMemberMemberBinding(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            this.UpdateHash(node.Method);
            return base.VisitMethodCall(node);
        }

        protected override Expression VisitNew(NewExpression node)
        {
            this.UpdateHash(node.Constructor);
            return base.VisitNew(node);
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            this.UpdateHash(node.Type);
            return base.VisitNewArray(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            this.UpdateHash(node.Type);
            return base.VisitParameter(node);
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            this.UpdateHash(node.Type);
            return base.VisitTypeBinary(node);
        }
    }
}