namespace Gu.Reactive.Tests.Collections.ReadOnlyViews
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reactive.Subjects;
    using Gu.Reactive.Internals;
    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public static class ReadOnlyFilteredViewTests
    {
        [Test]
        public static void InitializeFiltered()
        {
            var source = new ObservableCollection<int> { 1, 2 };
            var scheduler = new TestScheduler();
            using (var view = source.AsReadOnlyFilteredView(x => x < 2, scheduler))
            {
                CollectionAssert.AreEqual(new[] { 1 }, view);
            }
        }

        [Test]
        public static void InitializeFilteredBuffered()
        {
            var source = new ObservableCollection<int> { 1, 2 };
            var scheduler = new TestScheduler();
            using (var view = source.AsReadOnlyFilteredView(x => x < 2, TimeSpan.FromMilliseconds(100), scheduler))
            {
                CollectionAssert.AreEqual(new[] { 1 }, view);
            }
        }

        [Test]
        public static void DoesNotDisposeInner()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using (var filtered1 = source.AsReadOnlyFilteredView(x => true, leaveOpen: true))
            {
                using (var filtered2 = filtered1.AsReadOnlyFilteredView(x => true, leaveOpen: true))
                {
                    CollectionAssert.AreEqual(filtered1, source);
                    CollectionAssert.AreEqual(filtered2, source);
                }

                CollectionAssert.AreEqual(filtered1, source);
            }
        }

        [Test]
        public static void DisposesInnerByDefault()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using (var filtered1 = source.AsReadOnlyFilteredView(x => true, leaveOpen: true))
            {
                using (var filtered2 = filtered1.AsReadOnlyFilteredView(x => true))
                {
                    CollectionAssert.AreEqual(filtered1, source);
                    CollectionAssert.AreEqual(filtered2, source);
                }

                _ = Assert.Throws<ObjectDisposedException>(() => _ = filtered1.Count);
            }
        }

        [Test]
        public static void DisposesInnerExplicit()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using (var filtered1 = source.AsReadOnlyFilteredView(x => true, leaveOpen: true))
            {
                using (var filtered2 = filtered1.AsReadOnlyFilteredView(x => true, leaveOpen: false))
                {
                    CollectionAssert.AreEqual(filtered1, source);
                    CollectionAssert.AreEqual(filtered2, source);
                }

                _ = Assert.Throws<ObjectDisposedException>(() => _ = filtered1.Count);
            }
        }

        [Test]
        public static void NotifiesAdd()
        {
            var source = new ObservableCollection<int>();
            var scheduler = new TestScheduler();
            using (var view = source.AsReadOnlyFilteredView(x => true, scheduler))
            {
                using (var expected = source.SubscribeAll())
                {
                    using (var actual = view.SubscribeAll())
                    {
                        source.Add(1);
                        scheduler.Start();
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    }
                }
            }
        }

        [Test]
        public static void NotifiesAddBuffered()
        {
            var source = new ObservableCollection<int>();
            var scheduler = new TestScheduler();
            using (var view = source.AsReadOnlyFilteredView(x => true, TimeSpan.FromMilliseconds(100), scheduler))
            {
                using (var expected = source.SubscribeAll())
                {
                    using (var actual = view.SubscribeAll())
                    {
                        source.Add(1);
                        scheduler.Start();
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    }
                }
            }
        }

        [Test]
        public static void AddFiltered()
        {
            var source = new ObservableCollection<int>();
            var scheduler = new TestScheduler();
            using (var view = source.AsReadOnlyFilteredView(x => x % 2 == 0, scheduler))
            {
                using (var changes = view.SubscribeAll())
                {
                    source.Add(1);
                    scheduler.Start();
                    CollectionAssert.IsEmpty(view);
                    CollectionAssert.IsEmpty(changes);
                }
            }
        }

        [Test]
        public static void AddFilteredBuffered()
        {
            var source = new ObservableCollection<int>();
            var scheduler = new TestScheduler();
            using (var view = source.AsReadOnlyFilteredView(x => x % 2 == 0, TimeSpan.FromMilliseconds(100), scheduler))
            {
                using (var changes = view.SubscribeAll())
                {
                    source.Add(1);
                    scheduler.Start();
                    CollectionAssert.IsEmpty(view);
                    CollectionAssert.IsEmpty(changes);
                }
            }
        }

        [Test]
        public static void AddManyFiltered()
        {
            var source = new ObservableCollection<int>();
            var scheduler = new TestScheduler();
            using (var view = source.AsReadOnlyFilteredView(x => x % 2 == 0, scheduler))
            {
                using (var changes = view.SubscribeAll())
                {
                    source.Add(1);
                    source.Add(3);
                    source.Add(5);
                    scheduler.Start();
                    CollectionAssert.IsEmpty(view);
                    CollectionAssert.IsEmpty(changes);
                }
            }
        }

        [Test]
        public static void AddManyFilteredBuffered()
        {
            var source = new ObservableCollection<int>();
            var scheduler = new TestScheduler();
            using (var view = source.AsReadOnlyFilteredView(x => x % 2 == 0, TimeSpan.FromMilliseconds(100), scheduler))
            {
                using (var changes = view.SubscribeAll())
                {
                    source.Add(1);
                    source.Add(3);
                    source.Add(5);
                    scheduler.Start();
                    CollectionAssert.IsEmpty(view);
                    CollectionAssert.IsEmpty(changes);
                }
            }
        }

        [Test]
        public static void AddVisibleWhenFiltered()
        {
            var source = new ObservableCollection<int>();
            var scheduler = new TestScheduler();
            using (var view = source.AsReadOnlyFilteredView(x => x % 2 == 0, scheduler))
            {
                scheduler.Start();
                using (var actual = view.SubscribeAll())
                {
                    CollectionAssert.IsEmpty(view);
                    CollectionAssert.IsEmpty(actual);

                    source.Add(2);
                    scheduler.Start();
                    CollectionAssert.AreEqual(new[] { 2 }, view);
                    var expected = new EventArgs[]
                                       {
                                           CachedEventArgs.CountPropertyChanged,
                                           CachedEventArgs.IndexerPropertyChanged,
                                           Diff.CreateAddEventArgs(2, 0),
                                       };
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public static void AddVisibleWhenFilteredBuffered()
        {
            var source = new ObservableCollection<int>();
            var scheduler = new TestScheduler();
            using (var view = source.AsReadOnlyFilteredView(x => x % 2 == 0, TimeSpan.FromMilliseconds(100), scheduler))
            {
                scheduler.Start();
                using (var actual = view.SubscribeAll())
                {
                    CollectionAssert.IsEmpty(view);
                    CollectionAssert.IsEmpty(actual);

                    source.Add(2);
                    scheduler.Start();
                    CollectionAssert.AreEqual(new[] { 2 }, view);
                    var expected = new EventArgs[]
                                       {
                                               CachedEventArgs.CountPropertyChanged,
                                               CachedEventArgs.IndexerPropertyChanged,
                                               Diff.CreateAddEventArgs(2, 0),
                                       };
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public static void AddManyVisibleWhenFiltered()
        {
            var source = new ObservableCollection<int>();
            var scheduler = new TestScheduler();
            using (var view = source.AsReadOnlyFilteredView(x => x % 2 == 0, scheduler))
            {
                scheduler.Start();
                CollectionAssert.IsEmpty(view);
                using (var actual = view.SubscribeAll())
                {
                    source.Add(1);
                    scheduler.Start();
                    CollectionAssert.IsEmpty(view);
                    var expected = new List<EventArgs>();
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                    source.Add(2);
                    scheduler.Start();
                    CollectionAssert.AreEqual(new[] { 2 }, view);
                    expected.AddRange(
                        new EventArgs[]
                            {
                                CachedEventArgs.CountPropertyChanged,
                                CachedEventArgs.IndexerPropertyChanged,
                                Diff.CreateAddEventArgs(2, 0),
                            });
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                    source.Add(3);
                    scheduler.Start();
                    CollectionAssert.AreEqual(new[] { 2 }, view);
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                    source.Add(4);
                    scheduler.Start();
                    CollectionAssert.AreEqual(new[] { 2, 4 }, view);
                    expected.AddRange(
                        new EventArgs[]
                            {
                                CachedEventArgs.CountPropertyChanged,
                                CachedEventArgs.IndexerPropertyChanged,
                                Diff.CreateAddEventArgs(4, 1),
                            });
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                    source.Add(5);
                    scheduler.Start();
                    CollectionAssert.AreEqual(new[] { 2, 4 }, view);
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                    source.Add(6);
                    scheduler.Start();
                    CollectionAssert.AreEqual(new[] { 2, 4, 6 }, view);
                    expected.AddRange(
                        new EventArgs[]
                            {
                                CachedEventArgs.CountPropertyChanged,
                                CachedEventArgs.IndexerPropertyChanged,
                                Diff.CreateAddEventArgs(6, 2),
                            });
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public static void AddManyVisibleWhenFilteredBuffered()
        {
            var source = new ObservableCollection<int>();
            var scheduler = new TestScheduler();
            using (var view = source.AsReadOnlyFilteredView(x => x % 2 == 0, TimeSpan.FromMilliseconds(100), scheduler))
            {
                using (var changes = view.SubscribeAll())
                {
                    source.Add(1);
                    source.Add(2);
                    source.Add(3);
                    source.Add(4);
                    source.Add(5);
                    source.Add(6);
                    scheduler.Start();
                    CollectionAssert.AreEqual(new[] { 2, 4, 6 }, view);
                    var expected = new EventArgs[]
                                       {
                                               CachedEventArgs.CountPropertyChanged,
                                               CachedEventArgs.IndexerPropertyChanged,
                                               CachedEventArgs.NotifyCollectionReset,
                                       };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public static void RemoveFiltered()
        {
            var source = new ObservableCollection<int> { 1 };
            var scheduler = new TestScheduler();
            using (var view = source.AsReadOnlyFilteredView(x => x % 2 == 0, scheduler))
            {
                using (var changes = view.SubscribeAll())
                {
                    source.Remove(1);
                    scheduler.Start();
                    CollectionAssert.IsEmpty(view);
                    CollectionAssert.IsEmpty(changes);
                }
            }
        }

        [Test]
        public static void RemoveFilteredBuffered()
        {
            var source = new ObservableCollection<int> { 1 };
            var scheduler = new TestScheduler();
            using (var view = source.AsReadOnlyFilteredView(x => x % 2 == 0, TimeSpan.FromMilliseconds(100), scheduler))
            {
                using (var changes = view.SubscribeAll())
                {
                    source.Remove(1);
                    scheduler.Start();
                    CollectionAssert.IsEmpty(view);
                    CollectionAssert.IsEmpty(changes);
                }
            }
        }

        [Test]
        public static void RemoveVisible()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            var scheduler = new TestScheduler();
            using (var view = source.AsReadOnlyFilteredView(x => x % 2 == 0, scheduler))
            {
                scheduler.Start();
                using (var actual = view.SubscribeAll())
                {
                    source.Remove(2);
                    scheduler.Start();
                    CollectionAssert.IsEmpty(view);
                    var expected = new EventArgs[]
                                       {
                                           CachedEventArgs.CountPropertyChanged,
                                           CachedEventArgs.IndexerPropertyChanged,
                                           Diff.CreateRemoveEventArgs(2, 0),
                                       };
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public static void RemoveVisibleBuffered()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            var scheduler = new TestScheduler();
            using (var view = source.AsReadOnlyFilteredView(x => x % 2 == 0, TimeSpan.FromMilliseconds(100), scheduler))
            {
                scheduler.Start();
                using (var actual = view.SubscribeAll())
                {
                    CollectionAssert.AreEqual(new[] { 2 }, view);
                    CollectionAssert.IsEmpty(actual);

                    source.Remove(2);
                    scheduler.Start();
                    CollectionAssert.IsEmpty(view);
                    var expected = new EventArgs[]
                                       {
                                           CachedEventArgs.CountPropertyChanged,
                                           CachedEventArgs.IndexerPropertyChanged,
                                           Diff.CreateRemoveEventArgs(2, 0),
                                       };
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public static void ReplaceFiltered()
        {
            var source = new ObservableCollection<int> { 1 };
            var scheduler = new TestScheduler();
            using (var view = source.AsReadOnlyFilteredView(x => x % 2 == 0, scheduler))
            {
                using (var changes = view.SubscribeAll())
                {
                    source[0] = 3;
                    scheduler.Start();
                    CollectionAssert.IsEmpty(view);
                    CollectionAssert.IsEmpty(changes);
                }
            }
        }

        [Test]
        public static void ReplaceFilteredBuffered()
        {
            var source = new ObservableCollection<int> { 1 };
            var scheduler = new TestScheduler();
            using (var view = source.AsReadOnlyFilteredView(x => x % 2 == 0, TimeSpan.FromMilliseconds(100), scheduler))
            {
                using (var changes = view.SubscribeAll())
                {
                    source[0] = 3;
                    scheduler.Start();
                    CollectionAssert.IsEmpty(view);
                    CollectionAssert.IsEmpty(changes);
                }
            }
        }

        [Test]
        public static void ReplaceVisible()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            var scheduler = new TestScheduler();
            using (var view = source.AsReadOnlyFilteredView(x => x % 2 == 0, scheduler))
            {
                scheduler.Start();
                using (var actual = view.SubscribeAll())
                {
                    CollectionAssert.AreEqual(new[] { 2 }, view);
                    CollectionAssert.IsEmpty(actual);

                    source[1] = 4;
                    scheduler.Start();
                    CollectionAssert.AreEqual(new[] { 4 }, view);
                    var expected = new EventArgs[]
                                       {
                                           CachedEventArgs.IndexerPropertyChanged,
                                           Diff.CreateReplaceEventArgs(4, 2, 0),
                                       };
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public static void ReplaceVisibleBuffered()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            var scheduler = new TestScheduler();
            using (var view = source.AsReadOnlyFilteredView(x => x % 2 == 0, TimeSpan.FromMilliseconds(100), scheduler))
            {
                scheduler.Start();
                scheduler.Start();
                using (var actual = view.SubscribeAll())
                {
                    CollectionAssert.AreEqual(new[] { 2 }, view);
                    CollectionAssert.IsEmpty(actual);

                    source[1] = 4;
                    scheduler.Start();
                    CollectionAssert.AreEqual(new[] { 4 }, view);
                    var expected = new EventArgs[]
                                       {
                                           CachedEventArgs.IndexerPropertyChanged,
                                           Diff.CreateReplaceEventArgs(4, 2, 0),
                                       };
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public static void ReplaceFilteredWithVisible()
        {
            var source = new ObservableCollection<int> { 1 };
            var scheduler = new TestScheduler();
            using (var view = source.AsReadOnlyFilteredView(x => x % 2 == 0, scheduler))
            {
                scheduler.Start();
                using (var changes = view.SubscribeAll())
                {
                    source[0] = 2;
                    scheduler.Start();
                    CollectionAssert.AreEqual(new[] { 2 }, view);
                    var expected = new EventArgs[]
                                       {
                                           CachedEventArgs.CountPropertyChanged,
                                           CachedEventArgs.IndexerPropertyChanged,
                                           Diff.CreateAddEventArgs(2, 0),
                                       };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public static void ReplaceFilteredWithVisibleBuffered()
        {
            var source = new ObservableCollection<int> { 1 };
            var scheduler = new TestScheduler();
            using (var view = source.AsReadOnlyFilteredView(x => x % 2 == 0, TimeSpan.FromMilliseconds(100), scheduler))
            {
                using (var changes = view.SubscribeAll())
                {
                    source[0] = 2;
                    scheduler.Start();
                    CollectionAssert.AreEqual(new[] { 2 }, view);
                    var expected = new EventArgs[]
                                       {
                                           CachedEventArgs.CountPropertyChanged,
                                           CachedEventArgs.IndexerPropertyChanged,
                                           Diff.CreateAddEventArgs(2, 0),
                                       };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public static void FromObservableOfIEnumerable()
        {
            using (var subject = new Subject<IEnumerable<int>>())
            {
                using (var view = subject.AsReadOnlyFilteredView(x => x % 2 == 0))
                {
                    using (var actual = view.SubscribeAll())
                    {
                        subject.OnNext(new[] { 1, 2, 3, 4 });
                        CollectionAssert.AreEqual(new[] { 2, 4 }, view);
                        var expected = new EventArgs[]
                                       {
                                           CachedEventArgs.CountPropertyChanged,
                                           CachedEventArgs.IndexerPropertyChanged,
                                           CachedEventArgs.NotifyCollectionReset,
                                       };
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    }
                }

                Assert.AreEqual(false, subject.IsDisposed);
            }
        }

        [Test]
        public static void FromObservableOfMaybeIEnumerable()
        {
            using (var subject = new Subject<IMaybe<IEnumerable<int>>>())
            {
                using (var view = subject.AsReadOnlyFilteredView(x => x % 2 == 0))
                {
                    using (var actual = view.SubscribeAll())
                    {
                        subject.OnNext(Maybe.Some(new[] { 1, 2, 3, 4 }));
                        CollectionAssert.AreEqual(new[] { 2, 4 }, view);
                        var expected = new List<EventArgs>
                                       {
                                           CachedEventArgs.CountPropertyChanged,
                                           CachedEventArgs.IndexerPropertyChanged,
                                           CachedEventArgs.NotifyCollectionReset,
                                       };
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                        subject.OnNext(Maybe.None<IEnumerable<int>>());
                        CollectionAssert.IsEmpty(view);
                        expected.AddRange(
                            CachedEventArgs.CountPropertyChanged,
                            CachedEventArgs.IndexerPropertyChanged,
                            CachedEventArgs.NotifyCollectionReset);
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    }
                }

                Assert.AreEqual(false, subject.IsDisposed);
            }
        }

        [Test]
        public static void ObserveValueAsReadOnlyFilteredView()
        {
            var fake = new Fake<IEnumerable<int>>();
            using (var view = fake.ObserveValue(x => x.Value, signalInitial: true)
                                  .AsReadOnlyFilteredView(x => x % 2 == 0))
            {
                CollectionAssert.IsEmpty(view);
                using (var actual = view.SubscribeAll())
                {
                    fake.Value = new[] { 1, 2, 3, 4 };
                    CollectionAssert.AreEqual(new[] { 2, 4 }, view);
                    var expected = new List<EventArgs>
                                   {
                                       CachedEventArgs.CountPropertyChanged,
                                       CachedEventArgs.IndexerPropertyChanged,
                                       CachedEventArgs.NotifyCollectionReset,
                                   };
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                    fake.Value = null;
                    CollectionAssert.IsEmpty(view);
                    expected.AddRange(
                        CachedEventArgs.CountPropertyChanged,
                        CachedEventArgs.IndexerPropertyChanged,
                        CachedEventArgs.NotifyCollectionReset);
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }
    }
}
