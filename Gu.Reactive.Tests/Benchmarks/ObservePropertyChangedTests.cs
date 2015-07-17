namespace Gu.Reactive.Tests.Benchmarks
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Reactive;
    using System.Reactive.Linq;

    using Gu.Reactive.PropertyPathStuff;
    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    [Explicit("Longrunning benchmarks")]
    [TestFixture]
    public class ObservePropertyChangedTests : INotifyPropertyChanged
    {
        const int n = 10000;

        [Test]
        public void ObservePropertyChangedSimple()
        {
            var observables = new IObservable<EventPattern<PropertyChangedEventArgs>>[n];
            var fake = new Fake { IsTrueOrNull = false, IsTrue = true };
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                observables[i] = fake.ObservePropertyChanged(x => x.IsTrueOrNull, false);
            }
            Console.WriteLine("{0} fake.ToObservable(x => x.Prop1, false) took {1} ms ({2:F4} ms each)", n, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / n);
        }

        [Test]
        public void ObservePropertyChangedNested()
        {
            var observables = new IObservable<EventPattern<PropertyChangedEventArgs>>[n];
            var fake = new Fake { IsTrueOrNull = false, IsTrue = true, Next = new Level { Name = "" } };
            fake.ObservePropertyChanged(x => x.Next.Name, false); // Warm up

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                observables[i] = fake.ObservePropertyChanged(x => x.Next.Name, false);
            }
            Console.WriteLine("{0} fake.ToObservable(x => x.Next.Name, false); took {1} ms ({2:F4} ms each)", n, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / n);
        }

        [Test]
        public void ObservePropertyChangedNestedCachedPath()
        {
            var path = PropertyPath.Create<Fake, string>(x => x.Next.Name);
            var observables = new IObservable<EventPattern<PropertyChangedEventArgs>>[n];
            var fake = new Fake { IsTrueOrNull = false, IsTrue = true, Next = new Level { Name = "" } };
            fake.ObservePropertyChanged(x => x.Next.Name, false); // Warm up

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                observables[i] = fake.ObservePropertyChanged(path, false);
            }
            Console.WriteLine("{0} fake.ToObservable(x => x.Next.Name, false); took {1} ms ({2:F4} ms each)", n, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / n);
        }

        [Test]
        public void SubscribeSimple()
        {
            var fake = new Fake { IsTrueOrNull = false, IsTrue = true };
            var observable = fake.ObservePropertyChanged(x => x.IsTrueOrNull, false);
            observable.Subscribe(x => { }); // Warm up
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                observable.Subscribe(x => { });
            }
            Console.WriteLine("{0} fake.ToObservable(x => x.Prop1, false).Subscribe(x=>{{}}) took {1} ms ({2:F4} ms each)", n, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / n);
        }

        [Test]
        public void SubscribeNested()
        {
            var fake = new Fake { IsTrueOrNull = false, IsTrue = true, Next = new Level { Name = "" } };
            var observable = fake.ObservePropertyChanged(x => x.Next.Name, false);
            observable.Subscribe(x => { }); // Warm up
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                observable.Subscribe(x => { });
            }
            Console.WriteLine("{0} fake.ToObservable(x => x.Next.Name, false).Subscribe(x => {{ }}) took {1} ms ({2:F2} ms each)", n, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / n);
        }

        [Test]
        public void ReactSimple()
        {
            int count = 0;
            var fake = new Fake { IsTrueOrNull = false, IsTrue = true };
            var observable = fake.ObservePropertyChanged(x => x.IsTrueOrNull, false).Subscribe(x => count++);
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                fake.IsTrueOrNull = !fake.IsTrueOrNull;
            }
            Console.WriteLine("Reacting to {0} events took {1} ms ({2:F4} ms each)", n, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / n);
            Assert.AreEqual(n, count);
        }

        [Test]
        public void ReactNested()
        {
            int count = 0;
            var fake = new Fake { Next = new Level() };
            var observable = fake.ObservePropertyChanged(x => x.Next.IsTrue, false).Subscribe(x => count++);
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                fake.Next.IsTrue = !fake.Next.IsTrue;
            }
            Console.WriteLine("Reacting to {0} events took {1} ms ({2:F4} ms each)", n, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / n);
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
            Console.WriteLine("Comparison += for {0} events took {1} ms ({2:F4} ms each)", n, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / n);
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
            PropertyChangedEventManager.AddHandler(this, (sender, args) => count++, "Meh");
            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < n; i++)
            {
                OnPropertyChanged(new PropertyChangedEventArgs("Meh"));
            }
            Console.WriteLine("Reacting to {0} events took {1} ms ({2:F4} ms each)", n, stopwatch.ElapsedMilliseconds, (double)stopwatch.ElapsedMilliseconds / n);
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
