namespace Gu.Reactive.Tests.Sandbox
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Linq.Expressions;

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
}
