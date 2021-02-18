// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.Reactive.Tests.Collections.ReadOnlyViews
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reactive.Subjects;
    using Gu.Reactive.Tests.Helpers;
    using NUnit.Framework;

    public static class ReadOnlySerialViewTests
    {
        [Test]
        public static void Initalize()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using var view = new ReadOnlySerialView<int>(source);
            CollectionAssert.AreEqual(source, view);
        }

        [Test]
        public static void InitializeWithNull()
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            using var view = new ReadOnlySerialView<int>();
            CollectionAssert.IsEmpty(view);
        }

        [Test]
        public static void DoesNotDisposeInner()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using var filteredView = source.AsReadOnlyFilteredView(x => true);
            using (var serialView = new ReadOnlySerialView<int>(filteredView))
            {
                CollectionAssert.AreEqual(filteredView, source);
                CollectionAssert.AreEqual(serialView, source);
            }

            CollectionAssert.AreEqual(filteredView, source);
        }

        [Test]
        public static void SetSourceNoChange()
        {
            using var view = new ReadOnlySerialView<int>(new[] { 1, 2, 3 });
            using var actual = view.SubscribeAll();
            view.SetSource(new[] { 1, 2, 3 });
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, view);
            var expected = new[] { CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source") };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

            view.SetSource(new ObservableCollection<int> { 1, 2, 3 });
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, view);
            expected = new[]
            {
                CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source"),
                CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source"),
            };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public static void SetSourceReset()
        {
            using var view = new ReadOnlySerialView<int>(new[] { 1, 2, 3 });
            using var actual = view.SubscribeAll();
            var newSource1 = new[] { 4, 5 };
            view.SetSource(newSource1);
            CollectionAssert.AreEqual(newSource1, view);
            var expected = new List<EventArgs>
            {
                CachedEventArgs.CountPropertyChanged,
                CachedEventArgs.IndexerPropertyChanged,
                CachedEventArgs.NotifyCollectionReset,
                CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source"),
            };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

            var newSource2 = new ObservableCollection<int> { 6, 7 };
            view.SetSource(newSource2);
            CollectionAssert.AreEqual(newSource2, view);
            expected.AddRange(new EventArgs[]
            {
                CachedEventArgs.CountPropertyChanged,
                CachedEventArgs.IndexerPropertyChanged,
                CachedEventArgs.NotifyCollectionReset,
                CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source"),
            });
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public static void SetSourceAdd()
        {
            using var view = new ReadOnlySerialView<int>(new[] { 1, 2, 3 });
            using var actual = view.SubscribeAll();
            var newSource1 = new[] { 1, 2, 3, 4 };
            view.SetSource(newSource1);
            CollectionAssert.AreEqual(newSource1, view);
            var expected = new List<EventArgs>
            {
                CachedEventArgs.CountPropertyChanged,
                CachedEventArgs.IndexerPropertyChanged,
                Diff.CreateAddEventArgs(4, 3),
                CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source"),
            };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

            var newSource2 = new ObservableCollection<int> { 1, 2, 3, 4, 5 };
            view.SetSource(newSource2);
            CollectionAssert.AreEqual(newSource2, view);
            expected.AddRange(
                new EventArgs[]
                {
                    CachedEventArgs.CountPropertyChanged,
                    CachedEventArgs.IndexerPropertyChanged,
                    Diff.CreateAddEventArgs(5, 4),
                    CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source"),
                });
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public static void SetSourceRemove()
        {
            using var view = new ReadOnlySerialView<int>(new[] { 1, 2, 3 });
            using var actual = view.SubscribeAll();
            var newSource = new[] { 1, 2 };
            view.SetSource(newSource);
            CollectionAssert.AreEqual(newSource, view);
            var expected = new List<EventArgs>
            {
                CachedEventArgs.CountPropertyChanged,
                CachedEventArgs.IndexerPropertyChanged,
                Diff.CreateRemoveEventArgs(3, 2),
                CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source"),
            };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

            newSource = new[] { 1 };
            view.SetSource(newSource);
            CollectionAssert.AreEqual(newSource, view);
            expected.AddRange(
                new EventArgs[]
                {
                    CachedEventArgs.CountPropertyChanged,
                    CachedEventArgs.IndexerPropertyChanged,
                    Diff.CreateRemoveEventArgs(2, 1),
                    CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source"),
                });
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public static void SetSourceMove()
        {
            using var view = new ReadOnlySerialView<int>(new[] { 1, 2, 3 });
            using var actual = view.SubscribeAll();
            var newSource = new[] { 1, 3, 2 };
            view.SetSource(newSource);
            CollectionAssert.AreEqual(newSource, view);
            var expected = new List<EventArgs>
            {
                CachedEventArgs.IndexerPropertyChanged,
                Diff.CreateMoveEventArgs(2, 2, 1),
                CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source"),
            };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

            newSource = new[] { 3, 1, 2 };
            view.SetSource(newSource);
            CollectionAssert.AreEqual(newSource, view);
            expected.AddRange(
                new EventArgs[]
                {
                    CachedEventArgs.IndexerPropertyChanged,
                    Diff.CreateMoveEventArgs(1, 1, 0),
                    CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source"),
                });
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public static void SetSourceReplace()
        {
            using var view = new ReadOnlySerialView<int>(new[] { 1, 2, 3 });
            using var actual = view.SubscribeAll();
            var newSource = new[] { 1, 2, 4 };
            view.SetSource(newSource);
            CollectionAssert.AreEqual(newSource, view);
            var expected = new List<EventArgs>
            {
                CachedEventArgs.IndexerPropertyChanged,
                Diff.CreateReplaceEventArgs(4, 3, 2),
                CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source"),
            };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

            newSource = new[] { 5, 2, 4 };
            view.SetSource(newSource);
            CollectionAssert.AreEqual(newSource, view);
            expected.AddRange(
                new EventArgs[]
                {
                    CachedEventArgs.IndexerPropertyChanged,
                    Diff.CreateReplaceEventArgs(5, 1, 0),
                    CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source"),
                });
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public static void SetSourceNull()
        {
            using var view = new ReadOnlySerialView<int>(new[] { 1, 2, 3 });
            using var actual = view.SubscribeAll();
            view.SetSource(null);
            CollectionAssert.IsEmpty(view);
            var expected = new EventArgs[]
            {
                CachedEventArgs.CountPropertyChanged,
                CachedEventArgs.IndexerPropertyChanged,
                CachedEventArgs.NotifyCollectionReset,
                CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source"),
            };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public static void SetSourceToEqual()
        {
            using var view = new ReadOnlySerialView<int>(new[] { 1, 2, 3 });
            using var actual = view.SubscribeAll();
            view.SetSource(new[] { 1, 2, 3 });
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, view);
            var expected = new[] { CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source") };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public static void ClearSource()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using var view = new ReadOnlySerialView<int>(source);
            using var actual = view.SubscribeAll();
            view.ClearSource();
            CollectionAssert.IsEmpty(view);
            var expected = new EventArgs[]
            {
                CachedEventArgs.CountPropertyChanged,
                CachedEventArgs.IndexerPropertyChanged,
                CachedEventArgs.NotifyCollectionReset,
                CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source"),
            };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public static void Updates()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using var expected = source.SubscribeAll();
            using var view = new ReadOnlySerialView<int>(source);
            using var actual = view.SubscribeAll();
            source.Add(4);
            CollectionAssert.AreEqual(source, view);
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

            source.RemoveAt(0);
            CollectionAssert.AreEqual(source, view);
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

            source.Clear();
            CollectionAssert.AreEqual(source, view);
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public static void FromObservableOfIEnumerable()
        {
            using var subject = new Subject<IEnumerable<int>>();
            using (var view = subject.AsReadOnlyView())
            {
                using var actual = view.SubscribeAll();
                subject.OnNext(new[] { 1, 2 });
                CollectionAssert.AreEqual(new[] { 1, 2 }, view);
                var expected = new EventArgs[]
                {
                    CachedEventArgs.CountPropertyChanged,
                    CachedEventArgs.IndexerPropertyChanged,
                    CachedEventArgs.NotifyCollectionReset,
                    CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source"),
                };
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
            }

            Assert.AreEqual(false, subject.IsDisposed);
        }

        [Test]
        public static void FromObservableOfMaybeIEnumerable()
        {
            using var subject = new Subject<IMaybe<IEnumerable<int>>>();
            using (var view = subject.AsReadOnlyView())
            {
                using var actual = view.SubscribeAll();
                subject.OnNext(Maybe.Some(new[] { 1, 2 }));
                CollectionAssert.AreEqual(new[] { 1, 2 }, view);
                var expected = new List<EventArgs>
                {
                    CachedEventArgs.CountPropertyChanged,
                    CachedEventArgs.IndexerPropertyChanged,
                    CachedEventArgs.NotifyCollectionReset,
                    CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source"),
                };
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                subject.OnNext(Maybe.None<IEnumerable<int>>());
                CollectionAssert.IsEmpty(view);
                expected.AddRange(
                    CachedEventArgs.CountPropertyChanged,
                    CachedEventArgs.IndexerPropertyChanged,
                    CachedEventArgs.NotifyCollectionReset,
                    CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source"));
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
            }

            Assert.AreEqual(false, subject.IsDisposed);
        }

        [Test]
        public static void ObserveValueAsReadOnlyView()
        {
            var fake = new Fake<IEnumerable<int>?>();
            using var view = fake.ObserveValue(x => x.Value, signalInitial: true)
                                 .AsReadOnlyView();
            CollectionAssert.IsEmpty(view);
            using var actual = view.SubscribeAll();
            fake.Value = new[] { 1, 2 };
            CollectionAssert.AreEqual(new[] { 1, 2 }, view);
            var expected = new List<EventArgs>
            {
                CachedEventArgs.CountPropertyChanged,
                CachedEventArgs.IndexerPropertyChanged,
                CachedEventArgs.NotifyCollectionReset,
                CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source"),
            };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

            fake.Value = null;
            CollectionAssert.IsEmpty(view);
            expected.AddRange(
                CachedEventArgs.CountPropertyChanged,
                CachedEventArgs.IndexerPropertyChanged,
                CachedEventArgs.NotifyCollectionReset,
                CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source"));
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }
    }
}
