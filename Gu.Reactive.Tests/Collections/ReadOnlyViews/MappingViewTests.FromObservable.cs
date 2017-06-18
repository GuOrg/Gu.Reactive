namespace Gu.Reactive.Tests.Collections.ReadOnlyViews
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Subjects;
    using Gu.Reactive.Tests.Helpers;
    using NUnit.Framework;

    public partial class MappingViewTests
    {
        public class FromObservable
        {
            [Test]
            public void FromObservableOfIEnumerable()
            {
                using (var subject = new Subject<IEnumerable<int>>())
                {
                    using (var view = subject.AsMappingView(x => x * 2))
                    {
                        using (var actual = view.SubscribeAll())
                        {
                            subject.OnNext(new[] { 1, 2, 3, 4 });
                            CollectionAssert.AreEqual(new[] { 2, 4, 6, 8 }, view);
                            var expected = new EventArgs[]
                                           {
                                               CachedEventArgs.CountPropertyChanged,
                                               CachedEventArgs.IndexerPropertyChanged,
                                               CachedEventArgs.NotifyCollectionReset
                                           };
                            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                        }
                    }

                    Assert.AreEqual(false, subject.IsDisposed);
                }
            }

            [Test]
            public void FromObservableOfMaybeIEnumerable()
            {
                using (var subject = new Subject<IMaybe<IEnumerable<int>>>())
                {
                    using (var view = subject.AsMappingView(x => x * 2))
                    {
                        using (var actual = view.SubscribeAll())
                        {
                            subject.OnNext(Maybe.Some(new[] { 1, 2, 3, 4 }));
                            CollectionAssert.AreEqual(new[] { 2, 4, 6, 8 }, view);
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
            public void ObserveValueAsMappingView()
            {
                var fake = new Fake<IEnumerable<int>>();
                using (var view = fake.ObserveValue(x => x.Value, signalInitial: true)
                                      .AsMappingView(x => x * 2))
                {
                    CollectionAssert.IsEmpty(view);
                    using (var actual = view.SubscribeAll())
                    {
                        fake.Value = new[] { 1, 2, 3, 4 };
                        CollectionAssert.AreEqual(new[] { 2, 4, 6, 8 }, view);
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
}