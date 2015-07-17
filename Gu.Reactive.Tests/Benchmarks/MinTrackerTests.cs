namespace Gu.Reactive.Tests.Benchmarks
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    [Explicit("Longrunning benchmarks")]
    public class MinTrackerTests
    {
        [TestCase(10000), Explicit("Longrunning benchmark")]
        public void Simple(int n)
        {
            var ints = new ObservableCollection<int>();
            var tracker = ints.TrackMin(-1);
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                ints.Add(i);
            }
            sw.Stop();
            // 10000 updates took 1172 ms 0,117 ms each
            // 10000 updates took 10 ms 0,001 ms each
            Console.WriteLine("// Simple: {0} updates took {1} ms {2:F3} ms each. {3}", n, sw.ElapsedMilliseconds, sw.Elapsed.TotalMilliseconds / n, DateTime.Now.ToShortDateString());
        }

        [TestCase(1000)]
        public void Nested(int n)
        {
            var ints = new ObservableCollection<Fake>();
            using (var tracker = ints.TrackMin(x => x.Value, -1, false))
            {
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < n; i++)
                {
                    ints.Add(new Fake { Value = i });
                }
                sw.Stop();
                // Nested: 1000 updates took 17 ms 0,018 ms each. 2015-07-17
                Console.WriteLine("// Nested: {0} updates took {1} ms {2:F3} ms each. {3}", n, sw.ElapsedMilliseconds, sw.Elapsed.TotalMilliseconds / n, DateTime.Now.ToShortDateString());
            }
        }

        [TestCase(1000)]
        public void WhenTrackingChanges(int n)
        {
            var ints = new ObservableCollection<Fake>();
            using (var tracker = ints.TrackMin(x => x.Value, -1, true))
            {
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < n; i++)
                {
                    ints.Add(new Fake { Value = i });
                }
                sw.Stop();
                // WhenTrackingChanges: 1000 updates took 197 ms 0,198 ms each. 2015-07-17
                Console.WriteLine("// WhenTrackingChanges: {0} updates took {1} ms {2:F3} ms each. {3}", n, sw.ElapsedMilliseconds, sw.Elapsed.TotalMilliseconds / n, DateTime.Now.ToShortDateString());
            }
        }
    }
}