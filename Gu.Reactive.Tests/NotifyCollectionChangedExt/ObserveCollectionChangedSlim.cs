// ReSharper disable ClassNeverInstantiated.Global
namespace Gu.Reactive.Tests.NotifyCollectionChangedExt
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using Gu.Reactive.Tests.Helpers;
    using NUnit.Framework;

    public static class ObserveCollectionChangedSlim
    {
        [Test]
        public static void SignalsInitial()
        {
            var changes = new List<NotifyCollectionChangedEventArgs>();
            var source = new ObservableCollection<int>();
            var observable = source.ObserveCollectionChangedSlim(signalInitial: true);
            using (observable.Subscribe(changes.Add))
            {
                CollectionAssert.AreEqual(new[] { CachedEventArgs.NotifyCollectionReset }, changes);
            }

            CollectionAssert.AreEqual(new[] { CachedEventArgs.NotifyCollectionReset }, changes);
            var expected = new[]
            {
                    CachedEventArgs.NotifyCollectionReset,
                    CachedEventArgs.NotifyCollectionReset,
                };

            using (observable.Subscribe(changes.Add))
            {
                CollectionAssert.AreEqual(expected, changes);
            }

            CollectionAssert.AreEqual(expected, changes);
        }

        [Test]
        public static void DoesNotSignalInitial()
        {
            var changes = new List<NotifyCollectionChangedEventArgs>();
            var source = new ObservableCollection<int>();
            using (source.ObserveCollectionChangedSlim(signalInitial: false)
                         .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);
            }
        }

        [Test]
        public static void Reacts()
        {
            var changes = new List<NotifyCollectionChangedEventArgs>();
            var source = new ObservableCollection<int>();
            using (source.ObserveCollectionChangedSlim(signalInitial: false)
                         .Subscribe(changes.Add))
            {
                source.Add(1);
                CollectionAssert.AreEqual(new[] { Diff.CreateAddEventArgs(1, 0) }, changes, EventArgsComparer.Default);
            }
        }
    }
}
