namespace Gu.Reactive.Tests
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using NUnit.Framework;

    public class FixedSizedQueueTests
    {
        private const int Size = 2;
        private FixedSizedQueue<int> _queue;

        [SetUp]
        public void SetUp()
        {
            _queue = new FixedSizedQueue<int>(Size);
        }

        [Test]
        public void EnqueTrimsOverflowAndNotifies()
        {
            _queue.Enqueue(0);
            CollectionAssert.AreEqual(new[] { 0 }, _queue);

            _queue.Enqueue(1);
            CollectionAssert.AreEqual(new[] { 0, 1 }, _queue);

            _queue.Enqueue(2);
            CollectionAssert.AreEqual(new[] { 1, 2 }, _queue);
        }
    }
}
