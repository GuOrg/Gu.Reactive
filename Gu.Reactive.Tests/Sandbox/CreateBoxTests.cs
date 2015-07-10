namespace Gu.Reactive.Tests.Sandbox
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Reactive.Disposables;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.Reactive.Testing;
    using NUnit.Framework;

    public class CreateBoxTests
    {
        public event EventHandler Event;
        [Test, Explicit]
        public void Stucko()
        {
            var scheduler = new TestScheduler();
            var observable = Observable.Create<EventArgs>(x =>
            {
                scheduler.Schedule(TimeSpan.FromMilliseconds(15),
                                   () => x.OnNext(EventArgs.Empty));
                return Disposable.Empty;
            });
            observable.Buffer(() => observable.Throttle(TimeSpan.FromMilliseconds(10), scheduler))
                      .Subscribe(Console.WriteLine);
            scheduler.Start();
        }

        [Test]
        public void Publish()
        {
            var scheduler = new TestScheduler();
            var observable = Observable.Create<EventArgs>(x =>
            {
                EventHandler h = (_, e) =>
                {
                    x.OnNext(e);
                };
                this.Event += h;
                return Disposable.Create(() => this.Event -= h);
            });
            var shared = observable.Publish().RefCount();
            var buffering = shared.Buffer(() => shared.Throttle(TimeSpan.FromMilliseconds(15), scheduler));
            buffering.Subscribe(Console.WriteLine);
            scheduler.Schedule(TimeSpan.Zero, () => Event(this, EventArgs.Empty));
            scheduler.Start();
        }
    }
}
