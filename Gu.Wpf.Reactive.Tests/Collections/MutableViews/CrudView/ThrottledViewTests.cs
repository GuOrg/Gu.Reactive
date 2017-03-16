#pragma warning disable CS0618 // Type or member is obsolete
namespace Gu.Wpf.Reactive.Tests.Collections.MutableViews.CrudView
{
    using System;
    using System.Collections.ObjectModel;

    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public class ThrottledViewTests : CrudViewTests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.Scheduler = new TestScheduler();
#pragma warning disable GU0036 // Don't dispose injected.
            (this.View as IDisposable)?.Dispose();
#pragma warning restore GU0036 // Don't dispose injected.
            this.View = this.Ints.AsThrottledView(TimeSpan.FromMilliseconds(10), this.Scheduler);
        }

        [Test]
        public void AddToViewTestScheduler()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            var scheduler = new TestScheduler();
            using (var view = source.AsThrottledView(TimeSpan.FromMilliseconds(10), scheduler))
            {
                scheduler.Start();
                using (var expected = source.SubscribeAll())
                {
                    using (var actual = view.SubscribeAll())
                    {
                        view.Add(4);
                        scheduler.Start();
                        CollectionAssert.AreEqual(source, view);
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    }
                }
            }
        }
    }
}
