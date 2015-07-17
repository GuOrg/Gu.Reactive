namespace Gu.Reactive.Tests.Benchmarks
{
    using System;
    using System.Diagnostics;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    using Get = Gu.Reactive.Get;

    [Explicit("Longrunning benchmark")]
    public class GetTests
    {
        public Fake Fake { get; private set; }

        [SetUp]
        public void SetUp()
        {
            Fake = null;
        }

        [Test]
        public void ValueOrDefault()
        {
            int n = 1000;
            this.Fake = new Fake { Next = new Level { Name = "Johan" } };
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
                var name = path.GetValue(this).Value;
            }
            sw.Stop();
            Console.WriteLine(
                "path.Value(this).ValueOrDefault {0} times took: {1:F1} ms {2:F4} ms per call",
                n,
                sw.Elapsed.TotalMilliseconds,
                sw.Elapsed.TotalMilliseconds / n);

            sw.Restart();
            Func<Fake, string> func = x => this.Fake.Next.Name;
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
        public void GetValueCachedPath()
        {
            Fake = new Fake { Next = new Level() };
            var n = 1000;
            var path = Get.ValuePath<GetTests, string>(x => x.Fake.Next.Name);
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                var name = path.GetValue(this).Value;
            }
            sw.Stop();
            Console.WriteLine(
                "path.Value(this).ValueOrDefault {0} times took: {1:F1} ms {2:F4} ms per call",
                n,
                sw.Elapsed.TotalMilliseconds,
                sw.Elapsed.TotalMilliseconds / n);
        }
    }
}