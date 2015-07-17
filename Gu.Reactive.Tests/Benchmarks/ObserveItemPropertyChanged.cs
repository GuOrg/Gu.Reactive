namespace Gu.Reactive.Tests.Benchmarks
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    [Explicit("Longrunning benchmarks")]
    public class ObserveItemPropertyChangedTests
    {
        // source.ObserveItemPropertyChanged(x => x.Value): 1000 Adds took 89 ms 0,090 ms each. 2015-07-17
        [TestCase(1000)]
        public void AddSimple(int n)
        {
            int count = 0;
            var source = new ObservableCollection<Fake>();
            source.ObserveItemPropertyChanged(x => x.Value)
                  .Subscribe(_ => count++);
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                source.Add(new Fake());
            }
            sw.Stop();
            Console.WriteLine("// source.ObserveItemPropertyChanged(x => x.Value): {0} Adds took {1} ms {2:F3} ms each. {3}", n, sw.ElapsedMilliseconds, sw.Elapsed.TotalMilliseconds / n, DateTime.Now.ToShortDateString());
        }

        // source.ObserveItemPropertyChanged(x => x.Next.Next.Value): 1000 Adds took 112 ms 0,113 ms each. 2015-07-17
        [TestCase(1000)]
        public void AddNested(int n)
        {
            int count = 0;
            var source = new ObservableCollection<Fake>();
            source.ObserveItemPropertyChanged(x => x.Next.Next.Value)
                  .Subscribe(_ => count++);
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                source.Add(new Fake());
            }
            sw.Stop();
            Console.WriteLine("// source.ObserveItemPropertyChanged(x => x.Next.Next.Value): {0} Adds took {1} ms {2:F3} ms each. {3}", n, sw.ElapsedMilliseconds, sw.Elapsed.TotalMilliseconds / n, DateTime.Now.ToShortDateString());
        }

        // source.ObserveItemPropertyChanged(x => x.Next.Next.Value): 1000 Adds took 112 ms 0,113 ms each. 2015-07-17
        [TestCase(1000)]
        public void AddNestedThatUpdates(int n)
        {
            int count = 0;
            var source = new ObservableCollection<Fake>();
            source.ObserveItemPropertyChanged(x => x.Next.Next.Value)
                  .Subscribe(_ => count++);
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                var fake = new Fake();
                source.Add(fake);
                fake.Next = new Level { Next = new Level { Value = 1 } };
            }
            sw.Stop();
            Console.WriteLine("// source.ObserveItemPropertyChanged(x => x.Next.Next.Value): {0} Adds took {1} ms {2:F3} ms each. {3}", n, sw.ElapsedMilliseconds, sw.Elapsed.TotalMilliseconds / n, DateTime.Now.ToShortDateString());
        }
    }
}
