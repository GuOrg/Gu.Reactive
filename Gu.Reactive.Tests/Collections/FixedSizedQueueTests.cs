namespace Gu.Reactive.Tests.Collections
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

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
        public void EnqueueTrims()
        {
            _queue.Enqueue(0);
            CollectionAssert.AreEqual(new[] { 0 }, _queue);

            _queue.Enqueue(1);
            CollectionAssert.AreEqual(new[] { 0, 1 }, _queue);

            _queue.Enqueue(2);
            CollectionAssert.AreEqual(new[] { 1, 2 }, _queue);
        }

        [Test]
        public void SerializeRountrip()
        {
            var binaryFormatter = new BinaryFormatter();
            var stream = new MemoryStream();
            _queue.Enqueue(1);
            _queue.Enqueue(2);
            binaryFormatter.Serialize(stream, _queue);
            stream.Position = 0;
            var roundtripped = (FixedSizedQueue<int>)binaryFormatter.Deserialize(stream);
            Assert.AreEqual(_queue, roundtripped);
        }
    }
}
