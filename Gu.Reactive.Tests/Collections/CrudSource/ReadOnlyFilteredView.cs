namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public class ReadOnlyFilteredView : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            this.Scheduler = new TestScheduler();
            (this.View as IDisposable)?.Dispose();
            this.View = new ReadOnlyFilteredView<int>(this.Ints, x => true, TimeSpan.FromMilliseconds(10), this.Scheduler);
            this.Scheduler.Start();
            this.ActualEventArgs?.Dispose();
            this.ActualEventArgs = this.View.SubscribeAll();
        }

        [Test]
        public void InitializeFiltered()
        {
            var ints = new ObservableCollection<int>(new[] { 1, 2 });
            using (var view = ints.AsReadOnlyFilteredView(x => x < 2))
            {
                CollectionAssert.AreEqual(new[] { 1 }, view);
            }
        }

        [Test]
        public void NotifiesWithOnlyPropertyChangedSubscription()
        {
            var ints = new ObservableCollection<int>();
            using (var view = ints.AsReadOnlyFilteredView(x => true))
            {
                var changes = new List<EventArgs>();
                using (view.ObservePropertyChanged()
                           .Subscribe(x => changes.Add(x.EventArgs)))
                {
                    ints.Add(1);
                    var expected = new[]
                                       {
                                           CachedEventArgs.CountPropertyChanged,
                                           CachedEventArgs.IndexerPropertyChanged
                                       };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void NotifiesWithOnlyCollectionChangedSubscription()
        {
            var ints = new ObservableCollection<int>();
            using (var view = ints.AsReadOnlyFilteredView(x => true))
            {
                var changes = new List<EventArgs>();
                using (view.ObserveCollectionChangedSlim(false)
                           .Subscribe(x => changes.Add(x)))
                {
                    ints.Add(1);
                    var expected = new[] { Diff.CreateAddEventArgs(1, 0) };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void AddFiltered()
        {
            var ints = new ObservableCollection<int>();
            using (var view = ints.AsReadOnlyFilteredView(x => x % 2 == 0))
            {
                using (var changes = view.SubscribeAll())
                {
                    ints.Add(1);
                    CollectionAssert.IsEmpty(view);
                    CollectionAssert.IsEmpty(changes);
                }
            }
        }

        [Test]
        public void AddVisibleWhenFiltered()
        {
            var ints = new ObservableCollection<int>();
            using (var view = ints.AsFilteredView(x => x % 2 == 0))
            {
                using (var changes = view.SubscribeAll())
                {
                    var countChanges = 0;
                    using (view.ObservePropertyChanged(x => x.Count, false)
                               .Subscribe(_ => countChanges++))
                    {
                        Assert.AreEqual(0, countChanges);

                        ints.Add(2);
                        CollectionAssert.AreEqual(new[] { 2 }, view);
                        var expected = new EventArgs[]
                                           {
                                               CachedEventArgs.CountPropertyChanged,
                                               CachedEventArgs.IndexerPropertyChanged,
                                               Diff.CreateAddEventArgs(2, 0)
                                           };
                        CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                        Assert.AreEqual(1, countChanges);
                    }
                }
            }
        }

        [Test]
        public void RemoveFiltered()
        {
            var ints = new ObservableCollection<int>(new[] { 1 });
            using (var view = ints.AsReadOnlyFilteredView(x => x % 2 == 0))
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
        public void RemoveNonFiltered()
        {
            var ints = new ObservableCollection<int>(new[] { 1, 2, 3 });
            using (var view = ints.AsReadOnlyFilteredView(x => x % 2 == 0))
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
            var ints = new ObservableCollection<int>(new[] { 1 });
            using (var view = ints.AsReadOnlyFilteredView(x => x % 2 == 0))
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
            var ints = new ObservableCollection<int>(new[] { 1 });
            using (var view = ints.AsReadOnlyFilteredView(x => x % 2 == 0))
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