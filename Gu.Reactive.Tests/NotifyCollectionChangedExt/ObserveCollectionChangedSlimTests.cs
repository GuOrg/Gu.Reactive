namespace Gu.Reactive.Tests.NotifyCollectionChangedExt
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using Gu.Reactive.Tests.Helpers;
    using NUnit.Framework;

    public class ObserveCollectionChangedSlimTests
    {
        [Test]
        public void SignalsInitial()
        {
            var changes = new List<NotifyCollectionChangedEventArgs>();
            var source = new ObservableCollection<int>();
            var observable = source.ObserveCollectionChangedSlim(true);
            using (observable.Subscribe(changes.Add))
            {
            }

            CollectionAssert.AreEqual(new[] { CachedEventArgs.NotifyCollectionReset }, changes);
            using (observable.Subscribe(changes.Add))
            {
            }

            CollectionAssert.AreEqual(new[] { CachedEventArgs.NotifyCollectionReset, CachedEventArgs.NotifyCollectionReset }, changes);
        }

        [Test]
        public void DoesNotSignalInitial()
        {
            var changes = new List<NotifyCollectionChangedEventArgs>();
            var source = new ObservableCollection<int>();
            using (source.ObserveCollectionChangedSlim(false)
                         .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);
            }
        }

        [Test]
        public void Reacts()
        {
            var changes = new List<NotifyCollectionChangedEventArgs>();
            var source = new ObservableCollection<int>();
            using (source.ObserveCollectionChangedSlim(false)
                         .Subscribe(changes.Add))
            {
                source.Add(1);
                CollectionAssert.AreEqual(new[] { Diff.CreateAddEventArgs(1, 0) }, changes, EventArgsComparer.Default);
            }
        }
    }
}
