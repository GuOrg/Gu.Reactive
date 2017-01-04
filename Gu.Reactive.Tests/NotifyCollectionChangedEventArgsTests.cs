namespace Gu.Reactive.Tests
{
    using System.Collections.Specialized;

    using NUnit.Framework;

    public class NotifyCollectionChangedEventArgsTests
    {
        [Test]
        public void Add()
        {
            var expected = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, 1, 0);
            var actual = expected.As<int>();

            Assert.AreEqual(expected.Action, actual.Action);
            Assert.AreEqual(expected.NewStartingIndex, actual.NewStartingIndex);
            CollectionAssert.AreEqual(expected.NewItems, actual.NewItems);
            Assert.AreEqual(expected.OldStartingIndex, actual.OldStartingIndex);
            CollectionAssert.AreEqual(new int[0], actual.OldItems);
        }

        [Test]
        public void Remove()
        {
            var expected = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, 1, 0);
            var actual = expected.As<int>();

            Assert.AreEqual(expected.Action, actual.Action);
            Assert.AreEqual(expected.NewStartingIndex, actual.NewStartingIndex);
            CollectionAssert.AreEqual(new int[0], actual.NewItems);
            Assert.AreEqual(expected.OldStartingIndex, actual.OldStartingIndex);
            CollectionAssert.AreEqual(expected.OldItems, actual.OldItems);
        }
    }
}
