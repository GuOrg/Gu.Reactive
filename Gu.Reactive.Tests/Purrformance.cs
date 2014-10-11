namespace Gu.Reactive.Tests
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Reactive;

    using NUnit.Framework;

    public class Purrformance
    {
        [Test]
        public void ToObservable()
        {
            const int n = 1000;
            var observables = new IObservable<EventPattern<PropertyChangedEventArgs>>[n];
            var fake = new FakeInpc { Prop1 = false, Prop2 = true };
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                observables[i] = fake.ToObservable(x => x.Prop1, false);
            }
            Console.WriteLine("{0} fake.ToObservable(x => x.Prop1, false) took {1} ms ({2:F2} ms each)", n, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / n);
        }

        [Test]
        public void ToObservableNested()
        {
            const int n = 1000;
            var observables = new IObservable<EventPattern<PropertyChangedEventArgs>>[n];
            var fake = new FakeInpc { Prop1 = false, Prop2 = true, Next = new Level { Name = "" } };
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                observables[i] = fake.ToObservable(x => x.Next.Name, false);
            }
            Console.WriteLine("{0} fake.ToObservable(x => x.Next.Name, false); took {1} ms ({2:F2} ms each)", n, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / n);
        }

        [Test]
        public void ToObservableAndSubscribe()
        {
            const int n = 1000;
            var observables = new IDisposable[n];
            var fake = new FakeInpc { Prop1 = false, Prop2 = true };
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                observables[i] = fake.ToObservable(x => x.Prop1, false).Subscribe(x => { });
            }
            Console.WriteLine("{0} fake.ToObservable(x => x.Prop1, false).Subscribe(x=>{{}}) took {1} ms ({2:F2} ms each)", n, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / n);
        }


        [Test]
        public void ToObservableAndSubscribeNested()
        {
            const int n = 1000;
            var observables = new IDisposable[n];
            var fake = new FakeInpc { Prop1 = false, Prop2 = true, Next = new Level { Name = "" } };
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                observables[i] = fake.ToObservable(x => x.Next.Name, false).Subscribe(x => { });
            }
            Console.WriteLine("{0} fake.ToObservable(x => x.Next.Name, false).Subscribe(x => {{ }}) took {1} ms ({2:F2} ms each)", n, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / n);
        }

        [Test]
        public void React()
        {
            const int n = 1000;
            int count = 0;
            var fake = new FakeInpc { Prop1 = false, Prop2 = true };
            var stopwatch = Stopwatch.StartNew();
            var observable = fake.ToObservable(x => x.Prop1, false).Subscribe(x => count++);
            for (int i = 0; i < n; i++)
            {
                fake.Prop1 = !fake.Prop1;
            }
            Console.WriteLine("Reacting to {0} events took {1} ms ({2:F2} ms each)", n, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / n);
        }
    }
}
