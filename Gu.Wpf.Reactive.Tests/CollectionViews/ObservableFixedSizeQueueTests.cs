namespace Gu.Wpf.Reactive.Tests.CollectionViews
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reactive.Linq;
    using FakesAndHelpers;
    using Gu.Reactive;
    using Gu.Wpf.Reactive;

    using NUnit.Framework;

    public class ObservableFixedSizeQueueTests
    {
        private const int Size = 2;
        private List<NotifyCollectionChangedEventArgs> _args;
        private ObservableFixedSizeQueue<int> _queue;
        private readonly EventArgsComparer _eventArgsComparer = new EventArgsComparer();

        [SetUp]
        public void SetUp()
        {
            _args = new List<NotifyCollectionChangedEventArgs>();
            _queue = new ObservableFixedSizeQueue<int>(Size);
            _queue.ObserveCollectionChanged(false)
                  .Subscribe(x => _args.Add(x.EventArgs));
        }

        [Test]
        public void EnqueTrimsOverflowAndNotifies()
        {
            var expected = new List<NotifyCollectionChangedEventArgs>();
            _queue.Enqueue(0);
            CollectionAssert.AreEqual(new[] { 0 }, _queue);
            expected.Add(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, 0, 0));
            CollectionAssert.AreEqual(expected, _args, _eventArgsComparer);

            _queue.Enqueue(1);
            CollectionAssert.AreEqual(new[] { 0, 1 }, _queue);
            expected.Add(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, 1, 1));
            CollectionAssert.AreEqual(expected, _args, _eventArgsComparer);

            _queue.Enqueue(2);
            CollectionAssert.AreEqual(new[] { 1, 2 }, _queue);
            expected.Add(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, 0, 0));
            expected.Add(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, 2, 1));
            CollectionAssert.AreEqual(expected, _args, _eventArgsComparer);
        }
    }
}
