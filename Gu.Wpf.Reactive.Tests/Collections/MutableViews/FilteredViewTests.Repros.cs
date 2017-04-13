namespace Gu.Wpf.Reactive.Tests.Collections.MutableViews
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reactive.Subjects;

    using Gu.Reactive;
    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public partial class FilteredViewTests
    {
        public class Repros
        {
            [Test]
            public void Repro1()
            {
                var source = new ObservableCollection<int> { 1, 2, 3 };
                var scheduler = new TestScheduler();
                var max = 5;
                using (var trigger = new Subject<object>())
                {
                    // ReSharper disable once AccessToModifiedClosure
                    using (var view = new FilteredView<int>(source, x => x < max, TimeSpan.FromMilliseconds(10), scheduler, leaveOpen: true, triggers: trigger))
                    {
                        using (var actual = view.SubscribeAll())
                        {
                            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, view);
                            CollectionAssert.IsEmpty(actual);

                            max = 2;
                            trigger.OnNext(null);
                            scheduler.Start();

                            var expected = new List<EventArgs>
                                               {
                                                   CachedEventArgs.CountPropertyChanged,
                                                   CachedEventArgs.IndexerPropertyChanged,
                                                   CachedEventArgs.NotifyCollectionReset
                                               };
                            CollectionAssert.AreEqual(new[] { 1 }, view);
                            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                            for (var i = 0; i < 10; i++)
                            {
                                source.Add(i);
                            }

                            scheduler.Start();
                            CollectionAssert.AreEqual(new[] { 1, 0, 1 }, view);
                            expected.AddRange(new EventArgs[]
                                               {
                                                   CachedEventArgs.CountPropertyChanged,
                                                   CachedEventArgs.IndexerPropertyChanged,
                                                   CachedEventArgs.NotifyCollectionReset
                                               });
                            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                            source.Clear();
                            scheduler.Start();
                            CollectionAssert.IsEmpty(view);
                            expected.AddRange(new EventArgs[]
                                               {
                                                   CachedEventArgs.CountPropertyChanged,
                                                   CachedEventArgs.IndexerPropertyChanged,
                                                   CachedEventArgs.NotifyCollectionReset
                                               });
                            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                        }
                    }
                }
            }

            [Test]
            public void Repro2()
            {
                var source = new ObservableCollection<int> { 1, 2, 3 };
                var scheduler = new TestScheduler();
                var max = 5;
                using (var trigger = new Subject<object>())
                {
                    // ReSharper disable once AccessToModifiedClosure
                    using (var view = new FilteredView<int>(source, x => x < max, TimeSpan.FromMilliseconds(10), scheduler, leaveOpen: true, triggers: trigger))
                    {
                        using (var actual = view.SubscribeAll())
                        {
                            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, view);
                            CollectionAssert.IsEmpty(actual);

                            max = 2;
                            trigger.OnNext(null);
                            scheduler.Start();

                            var expected = new List<EventArgs>
                                               {
                                                   CachedEventArgs.CountPropertyChanged,
                                                   CachedEventArgs.IndexerPropertyChanged,
                                                   CachedEventArgs.NotifyCollectionReset
                                               };
                            CollectionAssert.AreEqual(new[] { 1 }, view);
                            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                            for (var i = 4; i < 10; i++)
                            {
                                source.Add(i);
                            }

                            scheduler.Start();
                            CollectionAssert.AreEqual(new[] { 1 }, view);
                            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                            source.Clear();
                            scheduler.Start();
                            CollectionAssert.IsEmpty(view);
                            expected.AddRange(new EventArgs[]
                                               {
                                                   CachedEventArgs.CountPropertyChanged,
                                                   CachedEventArgs.IndexerPropertyChanged,
                                                   Diff.CreateRemoveEventArgs(1, 0)
                                               });
                            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                        }
                    }
                }
            }

            [Test]
            public void Repro3()
            {
                var source = new ObservableCollection<int> { 1, 2, 3 };
                var scheduler = new TestScheduler();
                var max = 5;
                using (var trigger = new Subject<object>())
                {
                    // ReSharper disable once AccessToModifiedClosure
                    using (var view = new FilteredView<int>(source, x => x < max, TimeSpan.Zero, scheduler, leaveOpen: true, triggers: trigger))
                    {
                        using (var actual = view.SubscribeAll())
                        {
                            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, view);
                            CollectionAssert.IsEmpty(actual);

                            max = 2;
                            trigger.OnNext(null);
                            scheduler.Start();
                            var expected = new List<EventArgs>
                                               {
                                                   CachedEventArgs.CountPropertyChanged,
                                                   CachedEventArgs.IndexerPropertyChanged,
                                                   CachedEventArgs.NotifyCollectionReset
                                               };
                            CollectionAssert.AreEqual(new[] { 1 }, view);
                            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                            source.Clear();
                            view.Refresh();
                            scheduler.Start();
                            CollectionAssert.IsEmpty(view);
                            expected.AddRange(new EventArgs[]
                                               {
                                                   CachedEventArgs.CountPropertyChanged,
                                                   CachedEventArgs.IndexerPropertyChanged,
                                                   Diff.CreateRemoveEventArgs(1, 0)
                                               });
                            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                        }
                    }
                }
            }
        }
    }
}