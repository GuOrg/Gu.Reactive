namespace Gu.Reactive.Tests.NameOf_
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;

    using Gu.Reactive.Tests.Fakes;

    using NUnit.Framework;


    // ReSharper disable once InconsistentNaming
    [Explicit("Longrunning benchmarks")]
    public class BenchmarksTests
    {
        const int n = 10000;

        [Test]
        // ReSharper disable once InconsistentNaming
        public void NameOf_Property()
        {
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                var name = NameOf.Property<Fake, string>(x => x.Next.Next.Name);
            }
            Console.WriteLine("{0} NameOf.Property<Fake, string>(x => x.Next.Next.Name) took {1} ms ({2:F4} ms each)", n, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / n);
            // 10000 NameOf.Property<Fake, string>(x => x.Next.Next.Name) took 89 ms (0,0089 ms each)
        }

        [Test]
        // ReSharper disable once InconsistentNaming
        public void Nop_Test()
        {
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                Nop<Fake, string>(x => x.Next.Next.Name);
            }
            Console.WriteLine("{0} Nop<Fake, string>(x => x.Next.Next.Name); took {1} ms ({2:F4} ms each)", n, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / n);
            // 10000 NameOf.Property<Fake, string>(x => x.Next.Next.Name) took 69 ms (0,0069 ms each)
        }

        private static void Nop<TSource, TValue>(Expression<Func<TSource, TValue>> func)
        {
        }
    }
}