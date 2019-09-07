namespace Gu.Reactive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Subjects;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public partial class ObservableExtTests
    {
        public class AsReadOnlyView
        {
            [Test]
            public void OnNextAddsOne()
            {
                using (var subject = new Subject<IEnumerable<int>>())
                {
                    using (var view = subject.AsReadOnlyView())
                    {
                        using (var actual = view.SubscribeAll())
                        {
                            subject.OnNext(new[] { 1 });
                            CollectionAssert.AreEqual(new[] { 1 }, view);
                            var expected = new List<EventArgs>
                                               {
                                                   CachedEventArgs.CountPropertyChanged,
                                                   CachedEventArgs.IndexerPropertyChanged,
                                                   Diff.CreateAddEventArgs(1, 0),
                                                   CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source"),
                                               };
                            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                            subject.OnNext(new[] { 1, 2 });
                            CollectionAssert.AreEqual(new[] { 1, 2 }, view);
                            expected.AddRange(
                                new EventArgs[]
                                    {
                                        CachedEventArgs.CountPropertyChanged,
                                        CachedEventArgs.IndexerPropertyChanged,
                                        Diff.CreateAddEventArgs(2, 1),
                                        CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source"),
                                    });
                            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                        }
                    }
                }
            }

            [Test]
            public void ObserveValueAsReadOnlyViewWhenIEnumerableOfT()
            {
                var with = new With<IEnumerable<int>>();
                using (var view = with.ObserveValue(x => x.Value).AsReadOnlyView())
                {
                    using (var actual = view.SubscribeAll())
                    {
                        with.Value = new[] { 1 };
                        CollectionAssert.AreEqual(new[] { 1 }, view);
                        var expected = new List<EventArgs>
                        {
                            CachedEventArgs.CountPropertyChanged,
                            CachedEventArgs.IndexerPropertyChanged,
                            Diff.CreateAddEventArgs(1, 0),
                            CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source"),
                        };
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                        with.Value = new[] { 1, 2 };
                        CollectionAssert.AreEqual(new[] { 1, 2 }, view);
                        expected.AddRange(
                            new EventArgs[]
                            {
                                CachedEventArgs.CountPropertyChanged,
                                CachedEventArgs.IndexerPropertyChanged,
                                Diff.CreateAddEventArgs(2, 1),
                                CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Source"),
                            });
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    }
                }
            }
        }
    }
}
