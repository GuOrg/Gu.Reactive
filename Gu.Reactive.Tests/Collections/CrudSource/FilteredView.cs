#pragma warning disable CS0618 // Type or member is obsolete
namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections.ObjectModel;
    using Gu.Reactive.Tests.Helpers;
    using Microsoft.Reactive.Testing;
    using NUnit.Framework;

    public class FilteredView : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            this.Scheduler = new TestScheduler();
            (this.View as IDisposable)?.Dispose();
            this.View = new FilteredView<int>(this.Source, x => true, TimeSpan.FromMilliseconds(10), this.Scheduler);
            this.Scheduler.Start();
        }

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
    }
}