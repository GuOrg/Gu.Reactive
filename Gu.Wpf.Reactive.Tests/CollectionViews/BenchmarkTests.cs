namespace Gu.Wpf.Reactive.Tests.CollectionViews
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Gu.Reactive;

    using NUnit.Framework;

    [Explicit]
    public class BenchmarkTests
    {
        [TestCase(1000)]
        public void ResetTimerObservable(int n)
        {
            IDisposable timerSubscription;
            timerSubscription = Observable.Timer(TimeSpan.FromSeconds(1))
               .Subscribe(_ => Console.WriteLine("meh"));
            timerSubscription.Dispose();
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                timerSubscription = Observable.Timer(TimeSpan.FromSeconds(1))
                               .Subscribe(_ => Console.WriteLine("meh"));
                timerSubscription.Dispose();
            }
            sw.Stop();
            Console.WriteLine("Resetting {0} times took {1:F0} ms {2:F3} ms on average", n, sw.Elapsed.TotalMilliseconds, sw.Elapsed.TotalMilliseconds / n);
        }

        [TestCase(1000)]
        public void ResetTimer(int n)
        {
            var timer = new Timer(CallBack, null, TimeSpan.FromSeconds(1), TimeSpan.Zero);
            timer.Dispose();
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                timer = new Timer(CallBack, null, TimeSpan.FromSeconds(1), TimeSpan.Zero);
                timer.Dispose();
            }
            sw.Stop();
            Console.WriteLine("Resetting {0} times took {1:F0} ms {2:F3} ms on average", n, sw.Elapsed.TotalMilliseconds, sw.Elapsed.TotalMilliseconds / n);
        }

        [TestCase(1000)]
        public void ChangeTimer(int n)
        {
            var timer = new Timer(CallBack, null, TimeSpan.FromSeconds(1), TimeSpan.Zero);
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                timer.Change(TimeSpan.FromSeconds(1), TimeSpan.Zero);
            }
            sw.Stop();
            Console.WriteLine("Resetting {0} times took {1:F0} ms {2:F3} ms on average", n, sw.Elapsed.TotalMilliseconds, sw.Elapsed.TotalMilliseconds / n);
        }

        [Test]
        public async Task TimerChange()
        {
            var sw = Stopwatch.StartNew();
            var timer = new Timer(CallBack, sw, 10, -1);
            timer.Change(10, -1);
            timer.Change(10, -1);
            timer.Change(10, -1);
            timer.Change(10, -1);
            await Task.Delay(50);
        }

        [TestCase(1000)]
        public void AsThrottledView(int n)
        {
            var ints = new ObservableCollection<int>();
            var view = ints.AsThrottledView(TimeSpan.FromMilliseconds(10));
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                ints.Add(i);
            }
            sw.Stop();
            Console.WriteLine("adding {0} took: {1} ms {2:F3} ms each", n, sw.ElapsedMilliseconds, sw.Elapsed.TotalMilliseconds / n);

            sw.Restart();
            for (int i = 0; i < n; i++)
            {
                view.Add(i);
            }
            sw.Stop();
            Console.WriteLine("adding {0} took: {1} ms {2:F3} ms each", n, sw.ElapsedMilliseconds, sw.Elapsed.TotalMilliseconds / n);
        }

        private void CallBack(object state)
        {
            var sw = state as Stopwatch;
            if (sw != null)
            {
                Console.WriteLine("CallBack {0} ms", sw.ElapsedMilliseconds);
            }
            else
            {
                Console.WriteLine("CallBack");
            }
        }
    }
}
