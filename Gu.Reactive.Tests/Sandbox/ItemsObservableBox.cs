namespace Gu.Reactive.Tests.Sandbox
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Gu.Reactive.PropertyPathStuff;
    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    [Explicit("Sandbox")]
    public class ItemsObservableBoxTests
    {
        [TestCase(1000)]
        public void AddNested(int n)
        {
            var source = new ObservableCollection<Fake>();
            var path = PropertyPath.Create<Fake, int>(x => x.Next.Next.Value);
            var view = source.AsMappingView(x => x.ObservePropertyChanged(path, true));
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                var fake = new Fake();
                source.Add(fake);
            }
            sw.Stop();
            Console.WriteLine("// source.ObserveItemPropertyChanged(x => x.Next.Next.Value): {0} Adds took {1} ms {2:F3} ms each. {3}", n, sw.ElapsedMilliseconds, sw.Elapsed.TotalMilliseconds / n, DateTime.Now.ToShortDateString());
        }

        [TestCase(1000)]
        public void AddNestedMerge(int n)
        {
            int count = 0;
            var source = new ObservableCollection<Fake>();
            var path = PropertyPath.Create<Fake, int>(x => x.Next.Next.Value);
            var view = source.AsMappingView(x => x.ObservePropertyChanged(path, true));
            var sw = Stopwatch.StartNew();
            var subject = new Subject<IObservable<EventPattern<PropertyChangedEventArgs>>>();
            subject.Switch().Publish().RefCount().Subscribe(_ => count++);
            for (int i = 0; i < n; i++)
            {
                var fake = new Fake();
                source.Add(fake);
                subject.OnNext(view.Merge());
            }
            sw.Stop();
            Console.WriteLine("// source.ObserveItemPropertyChanged(x => x.Next.Next.Value): {0} Adds took {1} ms {2:F3} ms each. {3}", n, sw.ElapsedMilliseconds, sw.Elapsed.TotalMilliseconds / n, DateTime.Now.ToShortDateString());
        }

        [TestCase(1000)]
        public void AddNestedThatUpdates(int n)
        {
            var source = new ObservableCollection<Fake>();
            var path = PropertyPath.Create<Fake, int>(x => x.Next.Next.Value);
            var view = source.AsMappingView(x => x.ObservePropertyChanged(path, true));
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
