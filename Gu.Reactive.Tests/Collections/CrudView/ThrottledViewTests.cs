#pragma warning disable CS0618 // Type or member is obsolete
namespace Gu.Reactive.Tests.Collections.CrudView
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class ThrottledViewTests
    {
        [Test]
        public void OneChangeOneNotification()
        {
            var changes = new List<NotifyCollectionChangedEventArgs>();
            var source = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var deferTime = TimeSpan.FromMilliseconds(10);
            using (var throttledView = source.AsThrottledView(deferTime))
            {
                throttledView.CollectionChanged += (_, e) => changes.Add(e);
                source.Add(4);
                throttledView.Refresh();
                CollectionAssert.AreEqual(source, throttledView);
                var expected = new[] { new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, 4, 3) };
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
            }
        }

        [Test]
        public void ManyAddsOneReset()
        {
            var changes = new List<NotifyCollectionChangedEventArgs>();
            var source = new ObservableCollection<int>(new[] { 1, 2, 3 });
            using (var throttledView = source.AsThrottledView(TimeSpan.FromMilliseconds(100)))
            {
                throttledView.CollectionChanged += (_, e) => changes.Add(e);
                for (var i = 4; i < 10; i++)
                {
                    source.Add(i);
                }

                throttledView.Refresh();
                CollectionAssert.AreEqual(source, throttledView);
                var expected = new[] { new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset) };
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
            }
        }

        [Test]
        public void TwoBurstsTwoResets()
        {
            var changes = new List<NotifyCollectionChangedEventArgs>();
            var source = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var deferTime = TimeSpan.FromMilliseconds(10);
            using (var throttledView = source.AsThrottledView(deferTime))
            {
                throttledView.CollectionChanged += (_, e) => changes.Add(e);
                for (int i = 0; i < 10; i++)
                {
                    source.Add(i);
                }

                throttledView.Refresh();
                for (int i = 0; i < 10; i++)
                {
                    source.Add(i);
                }

                throttledView.Refresh();
                CollectionAssert.AreEqual(source, throttledView);

                var expected = new[]
                                   {
                                       new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset),
                                       new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)
                                   };
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
            }
        }
    }
}
