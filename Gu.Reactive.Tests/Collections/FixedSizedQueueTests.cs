namespace Gu.Reactive.Tests.Collections
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    using NUnit.Framework;

    public class FixedSizedQueueTests
    {
        private const int Size = 2;
        private FixedSizedQueue<int> queue;

        [SetUp]
        public void SetUp()
        {
            this.queue = new FixedSizedQueue<int>(Size);
        }

        [Test]
        public void EnqueueTrims()
        {
            this.queue.Enqueue(0);
            CollectionAssert.AreEqual(new[] { 0 }, this.queue);

            this.queue.Enqueue(1);
            CollectionAssert.AreEqual(new[] { 0, 1 }, this.queue);

            this.queue.Enqueue(2);
            CollectionAssert.AreEqual(new[] { 1, 2 }, this.queue);
        }

        [Test]
        public void SerializeRountrip()
        {
            var binaryFormatter = new BinaryFormatter();
            var stream = new MemoryStream();
            this.queue.Enqueue(1);
            this.queue.Enqueue(2);
            binaryFormatter.Serialize(stream, this.queue);
            stream.Position = 0;
            var roundtripped = (FixedSizedQueue<int>)binaryFormatter.Deserialize(stream);
            Assert.AreEqual(this.queue, roundtripped);
        }
    }
}
