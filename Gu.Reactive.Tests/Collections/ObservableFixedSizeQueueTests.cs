namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections.Generic;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class ObservableFixedSizeQueueTests
    {
        [Test]
        public void EnqueueWhenEmpty()
        {
            var queue = new ObservableFixedSizeQueue<int>(2);
            using var actual = queue.SubscribeAll();
            queue.Enqueue(1);
            CollectionAssert.AreEqual(new[] { 1 }, queue);
            var expected = CreateAddEventArgsCollection(1, 0);
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public void EnqueueWhenHasOne()
        {
            var queue = new ObservableFixedSizeQueue<int>(2);
            queue.Enqueue(1);
            using var actual = queue.SubscribeAll();
            queue.Enqueue(2);
            CollectionAssert.AreEqual(new[] { 1, 2 }, queue);
            var expected = CreateAddEventArgsCollection(2, 1);
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public void EnqueueTrimsOverflowAndNotifies()
        {
            var queue = new ObservableFixedSizeQueue<int>(2);
            queue.Enqueue(1);
            queue.Enqueue(2);
            using var actual = queue.SubscribeAll();
            queue.Enqueue(3);
            CollectionAssert.AreEqual(new[] { 2, 3 }, queue);
            var expected = new EventArgs[]
            {
                Diff.CreateRemoveEventArgs(1, 0),
                Diff.CreateAddEventArgs(3, 1),
            };

            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        private static IReadOnlyList<EventArgs> CreateAddEventArgsCollection(object item, int index)
        {
            return new EventArgs[]
                       {
                           CachedEventArgs.CountPropertyChanged,
                           Diff.CreateAddEventArgs(item, index),
                       };
        }
    }
}
