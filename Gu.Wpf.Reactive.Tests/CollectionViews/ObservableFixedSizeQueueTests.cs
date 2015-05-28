namespace Gu.Wpf.Reactive.Tests.CollectionViews
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    using Gu.Wpf.Reactive;

    using NUnit.Framework;

    public class ObservableFixedSizeQueueTests
    {
        private const int Size = 2;
        private const NotifyCollectionChangedAction Add = NotifyCollectionChangedAction.Add;
        private const NotifyCollectionChangedAction Remove = NotifyCollectionChangedAction.Remove;
        private List<NotifyCollectionChangedEventArgs> _args;
        private ObservableFixedSizeQueue<int> _queue;

        [SetUp]
        public void SetUp()
        {
            _args = new List<NotifyCollectionChangedEventArgs>();
            _queue = new ObservableFixedSizeQueue<int>(Size);
            _queue.CollectionChanged += (_, e) => _args.Add(e);
        }

        [Test]
        public void EnqueTrimsOverflowAndNotifies()
        {
            _queue.Enqueue(0);
            CollectionAssert.AreEqual(new[] { 0 }, _queue);
            CollectionAssert.AreEqual(new[] { 0 }, _args.SelectMany(x => x.NewItems.Cast<int>()));
            CollectionAssert.AreEqual(new[] { Add }, _args.Select(x => x.Action));
            CollectionAssert.AreEqual(new[] { 0 }, _args.Select(x => x.NewStartingIndex));
            CollectionAssert.AreEqual(new[] { -1 }, _args.Select(x => x.OldStartingIndex));

            _queue.Enqueue(1);
            CollectionAssert.AreEqual(new[] { 0, 1 }, _queue);
            CollectionAssert.AreEqual(new[] { 0, 1 }, _args.SelectMany(x => x.NewItems.Cast<int>()));
            CollectionAssert.AreEqual(new[] { Add, Add }, _args.Select(x => x.Action));
            CollectionAssert.AreEqual(new[] { 0, 1 }, _args.Select(x => x.NewStartingIndex));
            CollectionAssert.AreEqual(new[] { -1, -1 }, _args.Select(x => x.OldStartingIndex));

            _queue.Enqueue(2);
            CollectionAssert.AreEqual(new[] { 1, 2 }, _queue);
            CollectionAssert.AreEqual(new[] { 0, 1, 2 }, _args.Where(x => x.NewItems != null).SelectMany(x => x.NewItems.Cast<int>()));
            CollectionAssert.AreEqual(new[] { 0 }, _args.Where(x => x.OldItems != null).SelectMany(x => x.OldItems.Cast<int>()));
            CollectionAssert.AreEqual(new[] { 0, 1, 2, -1 }, _args.Select(x => x.NewStartingIndex));
            CollectionAssert.AreEqual(new[] { -1, -1, -1, 0 }, _args.Select(x => x.OldStartingIndex));
            CollectionAssert.AreEqual(new[] { Add, Add, Add, Remove }, _args.Select(x => x.Action));
        }
    }
}
