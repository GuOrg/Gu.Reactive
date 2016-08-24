namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.Diagnostics;

    using BenchmarkDotNet.Attributes;

    public class Timer
    {
        [Benchmark]
        public IDisposable Observable()
        {
            using (var timerSubscription = System.Reactive.Linq.Observable.Timer(TimeSpan.FromSeconds(1))
                                                 .Subscribe(_ => { }))
            {
                return timerSubscription;
            }
        }

        [Benchmark(Baseline = true)]
        public IDisposable ResetTimer()
        {
            using (var timer = new System.Threading.Timer(CallBack, null, TimeSpan.FromSeconds(1), TimeSpan.Zero))
            {
                return timer;
            }
        }

        [Benchmark]
        public IDisposable ChangeTimer()
        {
            using (var timer = new System.Threading.Timer(CallBack, null, TimeSpan.FromSeconds(1), TimeSpan.Zero))
            {
                timer.Change(TimeSpan.FromSeconds(2), TimeSpan.Zero);
                return timer;
            }
        }

        //[Test]
        //public async Task TimerChange()
        //{
        //    var sw = Stopwatch.StartNew();
        //    var timer = new Timer(CallBack, sw, 10, -1);
        //    timer.Change(10, -1);
        //    timer.Change(10, -1);
        //    timer.Change(10, -1);
        //    timer.Change(10, -1);
        //    await Task.Delay(50).ConfigureAwait(false);
        //}

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