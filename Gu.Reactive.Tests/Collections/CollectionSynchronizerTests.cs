namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public class CollectionSynchronizerTests : INotifyPropertyChanged, INotifyCollectionChanged, IEnumerable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        [Test]
        public void Initialize()
        {
            var source = new[] { 1, 2, 3 };
            var synchronizer = new CollectionSynchronizer<int>(source);
            CollectionAssert.AreEqual(source, synchronizer.Current);
        }

        [Test]
        public void RefreshNulls()
        {
            var source = new[] { 1, 2, 3 };
            var synchronizer = new CollectionSynchronizer<int>(new int[0]);
            var current = synchronizer.Current;
            synchronizer.Refresh(this, source, null, null, null, null);
            CollectionAssert.AreEqual(source, synchronizer.Current);
            Assert.AreSame(current, synchronizer.Current);
        }

        [Test]
        public void RefreshSignals()
        {
            var source = new ObservableCollection<int>();
            var synchronizer = new CollectionSynchronizer<int>(source);
            var expected = source.SubscribeAll();
            var actual = this.SubscribeAll();
            source.Add(1);
            synchronizer.Refresh(this, source, null, null, PropertyChanged, CollectionChanged);
            CollectionAssert.AreEqual(source, synchronizer.Current);
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public void RefreshSignalsWithScheduler()
        {
            var source = new ObservableCollection<int>();
            var synchronizer = new CollectionSynchronizer<int>(source);
            var expected = source.SubscribeAll();
            var actual = this.SubscribeAll();
            source.Add(1);
            var scheduler = new TestScheduler();
            synchronizer.Refresh(this, source, null, scheduler, PropertyChanged, CollectionChanged);
            CollectionAssert.AreEqual(source, synchronizer.Current);
            CollectionAssert.IsEmpty(actual.OfType<NotifyCollectionChangedEventArgs>());
            scheduler.Start();
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
