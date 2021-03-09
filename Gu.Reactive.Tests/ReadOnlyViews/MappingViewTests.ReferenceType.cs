namespace Gu.Reactive.Tests.ReadOnlyViews
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Gu.Reactive.Tests.Helpers;
    using NUnit.Framework;

    public static partial class MappingViewTests
    {
        public static class ReferenceType
        {
            [Test]
            public static void Initializes()
            {
                var model1 = Collections.ReadOnlyViews.MappingViewTests.Model.Create(1);
                var model2 = Collections.ReadOnlyViews.MappingViewTests.Model.Create(2);
                var model3 = Collections.ReadOnlyViews.MappingViewTests.Model.Create(3);
                var source = new ObservableCollection<Collections.ReadOnlyViews.MappingViewTests.Model<int>>(
                    new[]
                        {
                        model1,
                        model1,
                        model1,
                        model2,
                        model2,
                        model2,
                        model3,
                        model3,
                        model3,
                        });
                using var view = source.AsMappingView(Collections.ReadOnlyViews.MappingViewTests.Vm.Create);
                Assert.AreSame(view[0], view[1]);
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));
            }

            [Test]
            public static void Updates()
            {
                var source = new ObservableCollection<Collections.ReadOnlyViews.MappingViewTests.Model<int>>();
                using var view = source.AsMappingView(Collections.ReadOnlyViews.MappingViewTests.Vm.Create);
                var model = Collections.ReadOnlyViews.MappingViewTests.Model.Create(1);
                source.Add(model);
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));

                source.Clear();
                CollectionAssert.IsEmpty(view);

                source.Add(model);
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));
            }

            [Test]
            public static void Refresh()
            {
                var model1 = Collections.ReadOnlyViews.MappingViewTests.Model.Create(1);
                var model2 = Collections.ReadOnlyViews.MappingViewTests.Model.Create(2);
                var model3 = Collections.ReadOnlyViews.MappingViewTests.Model.Create(3);
                var source = new ObservableBatchCollection<Collections.ReadOnlyViews.MappingViewTests.Model<int>>(
                    new[]
                        {
                        model1,
                        model1,
                        model1,
                        model2,
                        model2,
                        model2,
                        model3,
                        model3,
                        model3,
                        });
                using var view = source.AsMappingView(Collections.ReadOnlyViews.MappingViewTests.Vm.Create);
                using var expected = source.SubscribeAll();
                using var actual = view.SubscribeAll();
                CollectionAssert.IsEmpty(actual);
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));

                source.AddRange(new[] { model1, model2, model2 });
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                CollectionAssert.AreEqual(expected, actual);

                source.Clear();
                CollectionAssert.IsEmpty(view);
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
            }

            [Test]
            public static void Caches()
            {
                var source = new ObservableCollection<Collections.ReadOnlyViews.MappingViewTests.Model<int>>();
                using var view = source.AsMappingView(Collections.ReadOnlyViews.MappingViewTests.Vm.Create);
                var model = Collections.ReadOnlyViews.MappingViewTests.Model.Create(1);
                source.Add(model);
                Assert.AreEqual(1, view.Count);

                source.Add(model);
                Assert.AreEqual(2, view.Count);
                Assert.AreSame(view[0], view[1]);

                var vm = view[0];
                source.Clear();
                CollectionAssert.IsEmpty(view);
                source.Add(model);
                Assert.AreNotSame(vm, view[0]);
                Assert.AreSame(vm.Model, view[0].Model);
            }

            [Test]
            public static void CachesWhenNotEmpty()
            {
                var model1 = Collections.ReadOnlyViews.MappingViewTests.Model.Create(1);
                var model2 = Collections.ReadOnlyViews.MappingViewTests.Model.Create(2);
                var model3 = Collections.ReadOnlyViews.MappingViewTests.Model.Create(3);
                var source = new ObservableCollection<Collections.ReadOnlyViews.MappingViewTests.Model<int>>(
                    new[]
                        {
                        model1,
                        model1,
                        model1,
                        model2,
                        model2,
                        model2,
                        model3,
                        model3,
                        model3,
                        });

                using var view = source.AsMappingView(Collections.ReadOnlyViews.MappingViewTests.Vm.Create);
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));

                var model4 = Collections.ReadOnlyViews.MappingViewTests.Model.Create(4);
                source.Add(model4);
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));

                source.Add(model4);
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));

                source.Add(model1);
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));

                source.Clear();
                CollectionAssert.IsEmpty(view);
            }

            [Test]
            public static void Add()
            {
                var source = new ObservableCollection<Collections.ReadOnlyViews.MappingViewTests.Model<int>>();
                using var view = source.AsMappingView(Collections.ReadOnlyViews.MappingViewTests.Vm.Create);
                using var actual = view.SubscribeAll();
                var model1 = Collections.ReadOnlyViews.MappingViewTests.Model.Create(1);
                source.Add(model1);
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                var expected = new List<EventArgs>
                {
                    CachedEventArgs.CountPropertyChanged,
                    CachedEventArgs.IndexerPropertyChanged,
                    Diff.CreateAddEventArgs(view[0], 0),
                };
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                source.Add(model1);
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                expected.AddRange(new EventArgs[]
                {
                    CachedEventArgs.CountPropertyChanged,
                    CachedEventArgs.IndexerPropertyChanged,
                    Diff.CreateAddEventArgs(view[1], 1),
                });
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                source.Add(Collections.ReadOnlyViews.MappingViewTests.Model.Create(2));
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                expected.AddRange(new EventArgs[]
                {
                    CachedEventArgs.CountPropertyChanged,
                    CachedEventArgs.IndexerPropertyChanged,
                    Diff.CreateAddEventArgs(view[2], 2),
                });
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
            }

            [Test]
            public static void Remove()
            {
                var model1 = Collections.ReadOnlyViews.MappingViewTests.Model.Create(1);
                var model2 = Collections.ReadOnlyViews.MappingViewTests.Model.Create(2);
                var model3 = Collections.ReadOnlyViews.MappingViewTests.Model.Create(3);
                var source = new ObservableCollection<Collections.ReadOnlyViews.MappingViewTests.Model<int>>(
                    new[]
                        {
                        model1,
                        model1,
                        model1,
                        model2,
                        model2,
                        model2,
                        model3,
                        model3,
                        model3,
                        });
                using var view = source.AsMappingView(Collections.ReadOnlyViews.MappingViewTests.Vm.Create);
                using var actual = view.SubscribeAll();
                var mapped0 = view[0];
                source.RemoveAt(0);
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                var expected = new List<EventArgs>
                {
                    CachedEventArgs.CountPropertyChanged,
                    CachedEventArgs.IndexerPropertyChanged,
                    Diff.CreateRemoveEventArgs(mapped0, 0),
                };
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
            }

            [Test]
            public static void Replace()
            {
                var model1 = Collections.ReadOnlyViews.MappingViewTests.Model.Create(1);
                var model2 = Collections.ReadOnlyViews.MappingViewTests.Model.Create(2);
                var model3 = Collections.ReadOnlyViews.MappingViewTests.Model.Create(3);
                var source = new ObservableCollection<Collections.ReadOnlyViews.MappingViewTests.Model<int>>(
                    new[]
                        {
                        model1,
                        model1,
                        model1,
                        model2,
                        model2,
                        model2,
                        model3,
                        model3,
                        model3,
                        });
                using var view = source.AsMappingView(Collections.ReadOnlyViews.MappingViewTests.Vm.Create);
                using var actual = view.SubscribeAll();
                var old = view[0];
                var @new = view[5];
                source[0] = source[5];
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                var expected = new List<EventArgs>
                {
                    CachedEventArgs.IndexerPropertyChanged,
                    Diff.CreateReplaceEventArgs(@new, old, 0),
                };
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
            }

            [Test]
            public static void Move()
            {
                var model1 = Collections.ReadOnlyViews.MappingViewTests.Model.Create(1);
                var model2 = Collections.ReadOnlyViews.MappingViewTests.Model.Create(2);
                var model3 = Collections.ReadOnlyViews.MappingViewTests.Model.Create(3);
                var source = new ObservableCollection<Collections.ReadOnlyViews.MappingViewTests.Model<int>>(
                    new[]
                        {
                        model1,
                        model1,
                        model1,
                        model2,
                        model2,
                        model2,
                        model3,
                        model3,
                        model3,
                        });
                using var view = source.AsMappingView(Collections.ReadOnlyViews.MappingViewTests.Vm.Create);
                using var actual = view.SubscribeAll();
                source.Move(0, 4);
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                var expected = new List<EventArgs>
                {
                    CachedEventArgs.IndexerPropertyChanged,
                    Diff.CreateMoveEventArgs(view[4], 4, 0),
                };
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
            }

            [Test]
            public static void Clear()
            {
                var model1 = Collections.ReadOnlyViews.MappingViewTests.Model.Create(1);
                var model2 = Collections.ReadOnlyViews.MappingViewTests.Model.Create(2);
                var model3 = Collections.ReadOnlyViews.MappingViewTests.Model.Create(3);
                var source = new ObservableCollection<Collections.ReadOnlyViews.MappingViewTests.Model<int>>(
                    new[]
                        {
                        model1,
                        model1,
                        model1,
                        model2,
                        model2,
                        model2,
                        model3,
                        model3,
                        model3,
                        });
                using var view = source.AsMappingView(Collections.ReadOnlyViews.MappingViewTests.Vm.Create);
                using var actual = view.SubscribeAll();
                source.Clear();
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                var expected = new List<EventArgs>
                {
                    CachedEventArgs.CountPropertyChanged,
                    CachedEventArgs.IndexerPropertyChanged,
                    CachedEventArgs.NotifyCollectionReset,
                };
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
            }
        }
    }
}
