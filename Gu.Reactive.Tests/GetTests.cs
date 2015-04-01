namespace Gu.Reactive.Tests
{
    using System;
    using System.Diagnostics;

    using NUnit.Framework;

    public class GetTests
    {
        public FakeInpc Fake { get; private set; }

        [SetUp]
        public void SetUp()
        {
            Fake = null;
        }

        [Test]
        public void Benchmark()
        {
            int n = 1000;
            this.Fake = new FakeInpc { Next = new Level { Name = "Johan" } };
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                var name = Get.ValueOrDefault(this, x => this.Fake.Next.Name); // Warming things up
            }
            sw.Restart();
            for (int i = 0; i < n; i++)
            {
                var name = Get.ValueOrDefault(this, x => this.Fake.Next.Name);
            }
            sw.Stop();
            var t1 = sw.Elapsed;
            Console.WriteLine(
                "Getting: this.Fake.Next.Name {0} times took: {1:F1} ms {2:F4} ms per call",
                n,
                sw.Elapsed.TotalMilliseconds,
                sw.Elapsed.TotalMilliseconds / n);

            sw.Restart();
            var path = Get.ValuePath<GetTests, string>(x => x.Fake.Next.Name);
            for (int i = 0; i < n; i++)
            {
                var name = path.Value(this).ValueOrDefault;
            }
            sw.Stop();
            Console.WriteLine(
                "path.Value(this).ValueOrDefault {0} times took: {1:F1} ms {2:F4} ms per call",
                n,
                sw.Elapsed.TotalMilliseconds,
                sw.Elapsed.TotalMilliseconds / n);

            sw.Restart();
            Func<FakeInpc, string> func = x => this.Fake.Next.Name;
            for (int i = 0; i < n; i++)
            {
                var name = func(Fake);
            }
            sw.Stop();
            var t2 = sw.Elapsed;
            Console.WriteLine(
                "Getting: this.Fake.Next.Name {0} times using Func<FakeInpc, string> took: {1:F1} ms {2:F4} ms per call",
                n,
                sw.Elapsed.TotalMilliseconds,
                sw.Elapsed.TotalMilliseconds / n);

            Console.WriteLine("ValueOrDefault took {0:F0}x more time per call", t1.TotalMilliseconds / t2.TotalMilliseconds);
        }

        [Test]
        public void BenchmarkCachedPath()
        {
            var n = 1000;
            var path = Get.ValuePath<GetTests, string>(x => x.Fake.Next.Name);
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                var name = path.Value(this).ValueOrDefault;
            }
            sw.Stop();
            Console.WriteLine(
                "path.Value(this).ValueOrDefault {0} times took: {1:F1} ms {2:F4} ms per call",
                n,
                sw.Elapsed.TotalMilliseconds,
                sw.Elapsed.TotalMilliseconds / n);
        }

        [Test]
        public void ValuePathWhenHasValue()
        {
            this.Fake = new FakeInpc { Next = new Level { Name = "Johan" } };
            var path = Get.ValuePath<GetTests, string>(x => x.Fake.Next.Name);
            var value = path.Value(this);
            Assert.IsTrue(value.HasValue);
            Assert.AreEqual("Johan", value.ValueOrDefault);
        }

        [Test]
        public void ValuePathWhenNullInPath()
        {
            this.Fake = new FakeInpc();
            var path = Get.ValuePath<GetTests, string>(x => x.Fake.Next.Name);
            var value = path.Value(this);
            Assert.IsFalse(value.HasValue);
            Assert.AreEqual(null, value.ValueOrDefault);
        }

        [Test]
        public void GetNameWhenNull()
        {
            var name = Get.ValueOrDefault(() => this.Fake.Next.Name);
            Assert.AreEqual(null, name);

            name = Get.ValueOrDefault(this, x => this.Fake.Next.Name);
            Assert.AreEqual(null, name);
        }

        [Test, Explicit("Implement this")]
        public void GetWithMethod()
        {
            var name = Get.ValueOrDefault(() => this.Fake.Method().Next.Method().Name);
            Assert.AreEqual(null, name);

            name = Get.ValueOrDefault(this, x => this.Fake.Method().Next.Method().Name);
            Assert.AreEqual(null, name);
        }

        [Test]
        public void GetNameWhenNotNull()
        {
            this.Fake = new FakeInpc { Next = new Level { Name = "Johan" } };
            var name = Get.ValueOrDefault(() => this.Fake.Next.Name);
            Assert.AreEqual("Johan", name);

            name = Get.ValueOrDefault(this, x => this.Fake.Next.Name);
            Assert.AreEqual("Johan", name);
        }

        [Test]
        public void GetNameWhenNullExplicitDefaultValue()
        {
            var name = Get.ValueOrDefault(() => this.Fake.Next.Name, "null");
            Assert.AreEqual("null", name);

            name = Get.ValueOrDefault(this, x => this.Fake.Next.Name, "null");
            Assert.AreEqual("null", name);
        }

        [Test]
        public void GetValueWhenNullDefault()
        {
            var value = Get.ValueOrDefault(() => this.Fake.Next.Value);
            Assert.AreEqual(0, value);
            value = Get.ValueOrDefault(this, x => this.Fake.Next.Value);
            Assert.AreEqual(0, value);
        }

        [Test]
        public void GetValueWhenNullExplicit()
        {
            var value = Get.ValueOrDefault(() => this.Fake.Next.Value, int.MinValue);
            Assert.AreEqual(int.MinValue, value);

            value = Get.ValueOrDefault(this, x => this.Fake.Next.Value, int.MinValue);
            Assert.AreEqual(int.MinValue, value);
        }
    }
}
