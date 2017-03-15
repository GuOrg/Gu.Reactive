namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections.ObjectModel;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class ReadOnlySerialViewTests
    {
        [Test]
        public void Intialize()
        {
            var source = new ObservableCollection<int>(new[] { 1, 2, 3 });
            using (var view = new ReadOnlySerialView<int>(source))
            {
                CollectionAssert.AreEqual(source, view);
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
                using (var actual = view.SubscribeAll())
                {
                    var newSource = new[] { 4, 5 };
                    view.SetSource(newSource);
                    CollectionAssert.AreEqual(newSource, view);
                    var expected = new EventArgs[]
                                       {
                                           CachedEventArgs.CountPropertyChanged,
                                           CachedEventArgs.IndexerPropertyChanged,
                                           CachedEventArgs.NotifyCollectionReset,
                                           CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source")
                                       };
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void SetSourceNull()
        {
            using (var view = new ReadOnlySerialView<int>(new[] { 1, 2, 3 }))
            {
                using (var actual = view.SubscribeAll())
                {
                    view.SetSource(null);
                    CollectionAssert.IsEmpty(view);
                    var expected = new EventArgs[]
                                       {
                                           CachedEventArgs.CountPropertyChanged,
                                           CachedEventArgs.IndexerPropertyChanged,
                                           CachedEventArgs.NotifyCollectionReset,
                                           CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source")
                                       };
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void SetSourceToEqual()
        {
            using (var view = new ReadOnlySerialView<int>(new[] { 1, 2, 3 }))
            {
                using (var actual = view.SubscribeAll())
                {
                    view.SetSource(new[] { 1, 2, 3 });
                    CollectionAssert.AreEqual(new[] { 1, 2, 3 }, view);
                    var expected = new[] { CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source") };
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void ClearSource()
        {
            var source = new ObservableCollection<int>(new[] { 1, 2, 3 });
            using (var view = new ReadOnlySerialView<int>(source))
            {
                using (var actual = view.SubscribeAll())
                {
                    view.ClearSource();
                    CollectionAssert.IsEmpty(view);
                    var expected = new EventArgs[]
                                          {
                                              CachedEventArgs.CountPropertyChanged,
                                              CachedEventArgs.IndexerPropertyChanged,
                                              CachedEventArgs.NotifyCollectionReset,
                                              CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source")
                                          };
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void Updates()
        {
            var source = new ObservableCollection<int>(new[] { 1, 2, 3 });
            using (var expected = source.SubscribeAll())
            {
                using (var view = new ReadOnlySerialView<int>(source))
                {
                    using (var actual = view.SubscribeAll())
                    {
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
                }
            }
        }
    }
}
