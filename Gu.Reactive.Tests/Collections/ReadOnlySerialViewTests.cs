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
            using (var view = new ReadOnlySerialView<int>(ints))
            {
                CollectionAssert.AreEqual(ints, view);
            }
        }

        [Test]
        public void IntializeWithNull()
        {
            using (var view = new ReadOnlySerialView<int>(null))
            {
                CollectionAssert.IsEmpty(view);
            }
        }

        [Test]
        public void SetSource()
        {
            using (var view = new ReadOnlySerialView<int>(new[] { 1, 2, 3 }))
            {
                using (var changes = view.SubscribeAll())
                {
                    var newSource = new[] { 4, 5 };
                    view.SetSource(newSource);
                    CollectionAssert.AreEqual(newSource, view);
                    CollectionAssert.AreEqual(CachedEventArgs.ResetEventArgsCollection, changes, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void SetSourceNull()
        {
            using (var view = new ReadOnlySerialView<int>(new[] { 1, 2, 3 }))
            {
                using (var changes = view.SubscribeAll())
                {
                    view.SetSource(null);
                    CollectionAssert.IsEmpty(view);
                    CollectionAssert.AreEqual(CachedEventArgs.ResetEventArgsCollection, changes, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void SetSourceToEqual()
        {
            using (var view = new ReadOnlySerialView<int>(new[] { 1, 2, 3 }))
            {
                using (var changes = view.SubscribeAll())
                {
                    view.SetSource(new[] { 1, 2, 3 });
                    CollectionAssert.AreEqual(new[] { 1, 2, 3 }, view);
                    CollectionAssert.IsEmpty(changes);
                }
            }
        }

        [Test]
        public void ClearSource()
        {
            var ints = new ObservableCollection<int>(new[] { 1, 2, 3 });
            using (var view = new ReadOnlySerialView<int>(ints))
            {
                using (var changes = view.SubscribeAll())
                {
                    view.ClearSource();
                    CollectionAssert.IsEmpty(view);
                    CollectionAssert.AreEqual(CachedEventArgs.ResetEventArgsCollection, changes, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void Updates()
        {
            var ints = new ObservableCollection<int>(new[] { 1, 2, 3 });
            using (var expectedChanges = ints.SubscribeAll())
            {
                using (var view = new ReadOnlySerialView<int>(ints))
                {
                    using (var actualChanges = view.SubscribeAll())
                    {
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
        }
    }
}
