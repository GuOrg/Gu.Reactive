namespace Gu.Reactive.Tests.Collections
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    using NUnit.Framework;

    public class FixedSizedQueueTests
    {
        [Test]
        public void EnqueueTrimsOverflow()
        {
            var queue = new FixedSizedQueue<int>(2);
            queue.Enqueue(0);
            CollectionAssert.AreEqual(new[] { 0 }, queue);

            queue.Enqueue(1);
            CollectionAssert.AreEqual(new[] { 0, 1 }, queue);

            queue.Enqueue(2);
            CollectionAssert.AreEqual(new[] { 1, 2 }, queue);

            queue.Enqueue(3);
            CollectionAssert.AreEqual(new[] { 2, 3 }, queue);
        }

        [Test]
        public void SerializeRountrip()
        {
            var queue = new FixedSizedQueue<int>(2);
            var binaryFormatter = new BinaryFormatter();
            using var stream = new MemoryStream();
            queue.Enqueue(1);
            queue.Enqueue(2);
            binaryFormatter.Serialize(stream, queue);
            stream.Position = 0;
            var roundtripped = (FixedSizedQueue<int>)binaryFormatter.Deserialize(stream);
            Assert.AreEqual(queue, roundtripped);
        }
    }
}
