﻿namespace Gu.Reactive.Tests
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Reactive;
    using System.Reactive.Linq;
    using NUnit.Framework;
    [Explicit("Benchmarks")]
    public class Purrformance : INotifyPropertyChanged
    {
        const int n = 10000000;

        [Test]
        public void ToObservable()
        {
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
            int n = 1000;
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
            int n = 1000;
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
            int count = 0;
            var fake = new FakeInpc { Prop1 = false, Prop2 = true };
            var stopwatch = Stopwatch.StartNew();
            var observable = fake.ToObservable(x => x.Prop1, false).Subscribe(x => count++);
            for (int i = 0; i < n; i++)
            {
                fake.Prop1 = !fake.Prop1;
            }
            Console.WriteLine("Reacting to {0} events took {1} ms ({2:F2} ms each)", n, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / n);
            Assert.AreEqual(n, count);
        }


        [Test]
        public void ReactNested()
        {
            int count = 0;
            var fake = new FakeInpc { Next = new Level()};
            var stopwatch = Stopwatch.StartNew();
            var observable = fake.ToObservable(x => x.Next.Value, false).Subscribe(x => count++);
            for (int i = 0; i < n; i++)
            {
                fake.Next.Value = !fake.Next.Value;
            }
            Console.WriteLine("Reacting to {0} events took {1} ms ({2:F2} ms each)", n, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / n);
            Assert.AreEqual(n, count);
        }


        [Test]
        public void BaselineVanillaEvent()
        {
            int count = 0;
            var stopwatch = Stopwatch.StartNew();
            Comparison += (sender, args) => count++;
            for (int i = 0; i < n; i++)
            {
                OnComparison();
            }
            Console.WriteLine("Comparison += for {0} events took {1} ms ({2:F3} ms each)", n, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / n);
            Assert.AreEqual(n, count);
        }

        [Test]
        public void SubscribeVanillaEvent()
        {
            int count = 0;
            var stopwatch = Stopwatch.StartNew();
            Observable.FromEventPattern<EventHandler, EventArgs>(x => Comparison += x, x => Comparison -= x)
                      .Subscribe(x => count++);
            for (int i = 0; i < n; i++)
            {
                OnComparison();
            }
            Console.WriteLine("Observable.FromEventPattern.Subscribe() to {0} events took {1} ms ({2:F3} ms each)", n, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / n);
            Assert.AreEqual(n, count);
        }

        [Test]
        public void BaselinePropertyChanged()
        {
            int count = 0;
            var stopwatch = Stopwatch.StartNew();
            PropertyChanged += (sender, args) =>
            {
                if (string.IsNullOrEmpty(args.PropertyName) || args.PropertyName == "Meh")
                {
                    count++;
                }
            };
            for (int i = 0; i < n; i++)
            {
                OnPropertyChanged(new PropertyChangedEventArgs("Meh"));
            }
            Console.WriteLine("Reacting to {0} events took {1} ms ({2:F3} ms each)", n, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / n);
            Assert.AreEqual(n, count);
        }

        [Test]
        public void BaselinePropertyChangedEventManager()
        {
            int count = 0;
            var stopwatch = Stopwatch.StartNew();
            PropertyChangedEventManager.AddHandler(this, (sender, args) => count++, "Meh");

            for (int i = 0; i < n; i++)
            {
                OnPropertyChanged(new PropertyChangedEventArgs("Meh"));
            }
            Console.WriteLine("Reacting to {0} events took {1} ms ({2:F3} ms each)", n, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / n);
            Assert.AreEqual(n, count);
        }

        public event EventHandler Comparison;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnComparison()
        {
            var handler = Comparison;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
