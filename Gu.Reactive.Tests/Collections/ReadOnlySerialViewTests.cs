namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections.Generic;
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
        public void SetSourceReset()
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
        public void SetSourceAdd()
        {
            using (var view = new ReadOnlySerialView<int>(new[] { 1, 2, 3 }))
            {
                using (var actual = view.SubscribeAll())
                {
                    var newSource = new[] { 1, 2, 3, 4 };
                    view.SetSource(newSource);
                    CollectionAssert.AreEqual(newSource, view);
                    var expected = new List<EventArgs>
                                       {
                                           CachedEventArgs.CountPropertyChanged,
                                           CachedEventArgs.IndexerPropertyChanged,
                                           Diff.CreateAddEventArgs(4, 3),
                                           CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source")
                                       };
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                    newSource = new[] { 1, 2, 3, 4, 5 };
                    view.SetSource(newSource);
                    CollectionAssert.AreEqual(newSource, view);
                    expected.AddRange(
                        new EventArgs[]
                            {
                                CachedEventArgs.CountPropertyChanged,
                                CachedEventArgs.IndexerPropertyChanged,
                                Diff.CreateAddEventArgs(5, 4),
                                CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source")
                            });
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void SetSourceRemove()
        {
            using (var view = new ReadOnlySerialView<int>(new[] { 1, 2, 3 }))
            {
                using (var actual = view.SubscribeAll())
                {
                    var newSource = new[] { 1, 2 };
                    view.SetSource(newSource);
                    CollectionAssert.AreEqual(newSource, view);
                    var expected = new List<EventArgs>
                                       {
                                           CachedEventArgs.CountPropertyChanged,
                                           CachedEventArgs.IndexerPropertyChanged,
                                           Diff.CreateRemoveEventArgs(3, 2),
                                           CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source")
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
                                CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source")
                            });
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void SetSourceMove()
        {
            using (var view = new ReadOnlySerialView<int>(new[] { 1, 2, 3 }))
            {
                using (var actual = view.SubscribeAll())
                {
                    var newSource = new[] { 1, 3, 2 };
                    view.SetSource(newSource);
                    CollectionAssert.AreEqual(newSource, view);
                    var expected = new List<EventArgs>
                                       {
                                           CachedEventArgs.IndexerPropertyChanged,
                                           Diff.CreateMoveEventArgs(2, 2, 1),
                                           CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source")
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
                                CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source")
                            });
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void SetSourceReplace()
        {
            using (var view = new ReadOnlySerialView<int>(new[] { 1, 2, 3 }))
            {
                using (var actual = view.SubscribeAll())
                {
                    var newSource = new[] { 1, 2, 4 };
                    view.SetSource(newSource);
                    CollectionAssert.AreEqual(newSource, view);
                    var expected = new List<EventArgs>
                                       {
                                           CachedEventArgs.IndexerPropertyChanged,
                                           Diff.CreateReplaceEventArgs(4, 3, 2),
                                           CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source")
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
                                CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source")
                            });
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
