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
        private ObservableCollection<int> _source;
        private TimeSpan _deferTime;
        private IThrottledView<int> _throttledView;
        private readonly List<NotifyCollectionChangedEventArgs> _changes = new List<NotifyCollectionChangedEventArgs>();

        [SetUp]
        public void SetUp()
        {
            _source = new ObservableCollection<int>(new[] { 1, 2, 3 });
            _deferTime = TimeSpan.FromMilliseconds(10);
            _throttledView = _source.AsThrottledView(_deferTime);
            _changes.Clear();
        }

        [Test]
        public void OneChangeOneNotification()
        {
            _throttledView.CollectionChanged += (_, e) => _changes.Add(e);
            _source.Add(4);
            _throttledView.Refresh();
            CollectionAssert.AreEqual(_source, _throttledView);
            var expected = new[] { new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, 4, 3) };
            CollectionAssert.AreEqual(expected, _changes, EventArgsComparer.Default);
        }

        [Test]
        public void ManyChangeOneReset()
        {
            _throttledView.CollectionChanged += (_, e) => _changes.Add(e);
            for (int i = 0; i < 10; i++)
            {
                _source.Add(i);
            }
            _throttledView.Refresh();
            CollectionAssert.AreEqual(_source, _throttledView);
            var expected = new[] { new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset) };
            CollectionAssert.AreEqual(expected, _changes, EventArgsComparer.Default);
        }

        [Test]
        public void TwoBurstsTwoResets()
        {
            _throttledView.CollectionChanged += (_, e) => _changes.Add(e);
            for (int i = 0; i < 10; i++)
            {
                _source.Add(i);
            }
            _throttledView.Refresh();
            for (int i = 0; i < 10; i++)
            {
                _source.Add(i);
            }
            _throttledView.Refresh();
            CollectionAssert.AreEqual(_source, _throttledView);
            var expected = new[]
            {
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset),
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)
            };
            CollectionAssert.AreEqual(expected, _changes, EventArgsComparer.Default);
        }
    }
}
