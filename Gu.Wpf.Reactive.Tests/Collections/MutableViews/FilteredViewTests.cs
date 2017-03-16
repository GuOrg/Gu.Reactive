#pragma warning disable CS0618 // Type or member is obsolete
namespace Gu.Wpf.Reactive.Tests.Collections.MutableViews
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Reactive.Subjects;

    using Gu.Reactive;
    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public class FilteredViewTests
    {
        [Test]
        public void InitializeFiltered()
        {
            var source = new ObservableCollection<int> { 1, 2 };
            using (var view = source.AsFilteredView(x => x < 2))
            {
                CollectionAssert.AreEqual(new[] { 1 }, view);
            }
        }

        [Test]
        public void AddFiltered()
        {
            var source = new ObservableCollection<int>();
            using (var view = source.AsFilteredView(x => x % 2 == 0))
            {
                using (var changes = view.SubscribeAll())
                {
                    source.Add(1);
                    CollectionAssert.IsEmpty(view);
                    CollectionAssert.IsEmpty(changes);
                }
            }
        }

        [Test]
        public void AddVisibleWhenFiltered()
        {
            var source = new ObservableCollection<int>();
            using (var view = source.AsFilteredView(x => x % 2 == 0))
            {
                using (var expected = source.SubscribeAll())
                {
                    using (var changes = view.SubscribeAll())
                    {
                        source.Add(2);
                        CollectionAssert.AreEqual(source, view);
                        CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                    }
                }
            }
        }

        [Test]
        public void RemoveFiltered()
        {
            var ints = new ObservableCollection<int> { 1 };
            using (var view = ints.AsFilteredView(x => x % 2 == 0))
            {
                using (var changes = view.SubscribeAll())
                {
                    ints.Remove(1);
                    CollectionAssert.IsEmpty(view);
                    CollectionAssert.IsEmpty(changes);
                }
            }
        }

        [Test]
        public void RemoveVisible()
        {
            var ints = new ObservableCollection<int> { 1, 2, 3 };
            using (var view = ints.AsFilteredView(x => x % 2 == 0))
            {
                using (var changes = view.SubscribeAll())
                {
                    ints.Remove(2);
                    CollectionAssert.IsEmpty(view);
                    var expected = new EventArgs[]
                                       {
                                           CachedEventArgs.CountPropertyChanged,
                                           CachedEventArgs.IndexerPropertyChanged,
                                           Diff.CreateRemoveEventArgs(2, 0)
                                       };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void ReplaceFiltered()
        {
            var ints = new ObservableCollection<int> { 1 };
            using (var view = ints.AsFilteredView(x => x % 2 == 0))
            {
                using (var changes = view.SubscribeAll())
                {
                    ints[0] = 3;
                    CollectionAssert.IsEmpty(view);
                    CollectionAssert.IsEmpty(changes);
                }
            }
        }

        [Test]
        public void ReplaceFilteredWithVisible()
        {
            var ints = new ObservableCollection<int> { 1 };
            using (var view = ints.AsFilteredView(x => x % 2 == 0))
            {
                using (var changes = view.SubscribeAll())
                {
                    ints[0] = 2;
                    CollectionAssert.AreEqual(new[] { 2 }, view);
                    var expected = new EventArgs[]
                                       {
                                           CachedEventArgs.CountPropertyChanged,
                                           CachedEventArgs.IndexerPropertyChanged,
                                           Diff.CreateAddEventArgs(2, 0)
                                       };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void Refresh()
        {
            var ints = new List<int> { 1, 2, 3 };
            var scheduler = new TestScheduler();
            using (var view = ints.AsFilteredView(x => true, scheduler, new Subject<object>()))
            {
                using (var actual = view.SubscribeAll())
                {
                    view.Filter = x => x < 2;
                    view.Refresh();
                    scheduler.Start();
                    var expected = new List<EventArgs>();
                    expected.Add(new PropertyChangedEventArgs("Filter"));
                    expected.AddRange(
                        new EventArgs[]
                        {
                            CachedEventArgs.CountPropertyChanged,
                            CachedEventArgs.IndexerPropertyChanged,
                            CachedEventArgs.NotifyCollectionReset
                        });
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    CollectionAssert.AreEqual(new[] { 1 }, view);

                    view.Refresh();
                    scheduler.Start();
                    ////expected.AddRange(Diff.ResetEventArgsCollection);
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    CollectionAssert.AreEqual(new[] { 1 }, view);
                }
            }
        }
    }
}
