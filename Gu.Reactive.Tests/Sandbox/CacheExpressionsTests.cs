namespace Gu.Reactive.Tests.Sandbox
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    [Explicit("Sandbox")]
    public class CacheExpressionsTests
    {
        private ConcurrentDictionary<Expression, int> dictionary;

        public Fake Fake { get; set; }

        [SetUp]
        public void SetUp()
        {
            this.dictionary = new ConcurrentDictionary<Expression, int>();
        }

        [Test]
        public void TestNameTest()
        {
            this.Fake = new Fake { Next = new Level { Name = "Johan" } };

            for (int i = 0; i < 100; i++)
            {
                this.AddOrUpdate(x => this.Fake.Next.Name, i);
            }

            Assert.AreEqual(1, this.dictionary.Count);
        }

        [Test]
        public void Benchmark()
        {
            int n = 1000;
            this.Fake = new Fake { Next = new Level { Name = "Johan" } };
            var sw = Stopwatch.StartNew();

            for (int i = 0; i < n; i++)
            {
                var h = HashVisitor.GetHash(x => this.Fake.Next.Name); // Warming things up
            }

            sw.Restart();
            for (int i = 0; i < n; i++)
            {
                var h = HashVisitor.GetHash(x => this.Fake.Next.Name);
            }

            sw.Stop();
            var t1 = sw.Elapsed;
            Console.WriteLine(
                "Getting: this.Fake.Next.Name {0} times took: {1:F1} ms {2:F4} ms per call",
                n,
                sw.Elapsed.TotalMilliseconds,
                sw.Elapsed.TotalMilliseconds / n);

            sw.Restart();
            for (int i = 0; i < n; i++)
            {
                Expression<Func<object, string>> expression = x => this.Fake.Next.Name;
                var h = expression.ToString();
            }

            sw.Stop();
            Console.WriteLine(
                "expression.ToString() {0} times took: {1:F1} ms {2:F4} ms per call",
                n,
                sw.Elapsed.TotalMilliseconds,
                sw.Elapsed.TotalMilliseconds / n);
        }

        public void AddOrUpdate(Expression<Func<object, string>> expression, int i)
        {
            var hashCode = expression.GetHashCode();
            this.dictionary.AddOrUpdate(expression, _ => i, (_, __) => i);
        }
    }

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
