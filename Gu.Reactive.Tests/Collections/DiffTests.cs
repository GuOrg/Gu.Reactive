namespace Gu.Reactive.Tests.Collections
{
    using System.Collections.ObjectModel;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class DiffTests
    {
        [Test]
        public void NoChange()
        {
            var actual = Diff.CollectionChange(new[] { 1, 2, 3 }, new[] { 1, 2, 3 });
            Assert.IsNull(actual);
        }

        [Test]
        public void AddToEmpty()
        {
            var ints = new ObservableCollection<int>();
            var expected = ints.CollectionChange();
            ints.Add(1);

            var actual = Diff.CollectionChange(new int[0], ints);

            AssertEx.AreEqual(expected.Value, actual);
        }

        [Test]
        public void Add()
        {
            var before = new[] { 1, 2, 3 };
            var ints = new ObservableCollection<int>(before);
            var expected = ints.CollectionChange();
            ints.Add(4);

            var actual = Diff.CollectionChange(before, ints);

            AssertEx.AreEqual(expected.Value, actual);
        }

        [Test]
        public void AddRefType()
        {
            var before = new[] { new Fake(), new Fake(), new Fake() };
            var ints = new ObservableCollection<Fake>(before);
            var expected = ints.CollectionChange();
            ints.Add(new Fake());

            var actual = Diff.CollectionChange(before, ints);

            AssertEx.AreEqual(expected.Value, actual);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void Insert(int index)
        {
            var before = new[] { 1, 2, 3 };
            var ints = new ObservableCollection<int>(before);
            var expected = ints.CollectionChange();

            ints.Insert(index, 4);
            var actual = Diff.CollectionChange(before, ints);

            AssertEx.AreEqual(expected.Value, actual);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void Remove(int index)
        {
            var before = new[] { 1, 2, 3 };
            var ints = new ObservableCollection<int>(before);
            var expected = ints.CollectionChange();

            ints.RemoveAt(index);
            var actual = Diff.CollectionChange(before, ints);

            AssertEx.AreEqual(expected.Value, actual);
        }

        [Test]
        public void RemoveLast()
        {
            var before = new[] { 1 };
            var ints = new ObservableCollection<int>(before);
            var expected = ints.CollectionChange();

            ints.Remove(1);
            var actual = Diff.CollectionChange(before, ints);

            AssertEx.AreEqual(expected.Value, actual);
        }

        [Test]
        public void Move()
        {
            var before = new[] { 1, 2, 3 };
            var ints = new ObservableCollection<int>(before);
            var expected = ints.CollectionChange();

            ints.Move(1, 2);
            var actual = Diff.CollectionChange(before, ints);

            AssertEx.AreEqual(expected.Value, actual);
        }

        [Test]
        public void Replace()
        {
            var before = new[] { 1, 2, 3 };
            var ints = new ObservableCollection<int>(before);
            var expected = ints.CollectionChange();

            ints[0] = 5;
            var actual = Diff.CollectionChange(before, ints);

            AssertEx.AreEqual(expected.Value, actual);
        }

        [Test]
        public void Clear()
        {
            var before = new[] { 1, 2, 3 };
            var ints = new ObservableCollection<int>(before);
            var expected = ints.CollectionChange();

            ints.Clear();
            var actual = Diff.CollectionChange(before, ints);

            AssertEx.AreEqual(expected.Value, actual);
        }

        [Test]
        public void ClearOne()
        {
            var before = new[] { 1 };
            var ints = new ObservableCollection<int>(before);

            ints.Clear();
            var actual = Diff.CollectionChange(new[] { 1 }, ints);

            AssertEx.AreEqual(Diff.CreateRemoveEventArgs(1, 0), actual);
        }
    }
}
