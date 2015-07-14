namespace Gu.Reactive.Tests.Collections
{
    using System.Collections.ObjectModel;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class ReadOnlySerialViewTests
    {
        [Test]
        public void Intialize()
        {
            var ints = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var view = new ReadOnlySerialView<int>(ints);
            CollectionAssert.AreEqual(ints, view);
        }

        [Test]
        public void IntializeWithNull()
        {
            var view = new ReadOnlySerialView<int>(null);
            CollectionAssert.IsEmpty(view);
        }

        [Test]
        public void SetSource()
        {
            var ints = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var view = new ReadOnlySerialView<int>(ints);

            var changes = view.SubscribeAll();

            var newInts = new ObservableCollection<int>(new[] { 4, 5 });
            view.SetSource(newInts);
            CollectionAssert.AreEqual(newInts, view);
            CollectionAssert.AreEqual(Diff.ResetEventArgsCollection, changes, EventArgsComparer.Default);
        }

        [Test]
        public void SetSourceNull()
        {
            var ints = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var view = new ReadOnlySerialView<int>(ints);

            var changes = view.SubscribeAll();

            view.SetSource(null);
            CollectionAssert.IsEmpty(view);
            CollectionAssert.AreEqual(Diff.ResetEventArgsCollection, changes, EventArgsComparer.Default);
        }

        [Test]
        public void ClearSource()
        {
            var ints = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var view = new ReadOnlySerialView<int>(ints);

            var changes = view.SubscribeAll();

            view.ClearSource();
            CollectionAssert.IsEmpty(view);
            CollectionAssert.AreEqual(Diff.ResetEventArgsCollection, changes, EventArgsComparer.Default);
        }

        [Test]
        public void Updates()
        {
            var ints = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var expectedChanges = ints.SubscribeAll();

            var view = new ReadOnlySerialView<int>(ints);
            var actualChanges = view.SubscribeAll();

            ints.Add(4);
            CollectionAssert.AreEqual(ints, view);
            CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);

            ints.RemoveAt(0);
            CollectionAssert.AreEqual(ints, view);
            CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);

            ints.Clear();
            CollectionAssert.AreEqual(ints, view);
            CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);
        }
    }
}
