namespace Gu.Reactive.Tests.Collections.ReadOnlyViews
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public partial class MappingViewTests
    {
        public class Creating
        {
            [Test]
            public void InitializesValueType()
            {
                var source = new ObservableCollection<int> { 1, 1, 1, 2, 2, 2, 3, 3, 3 };
                using (var view = source.AsMappingView(Model.Create))
                {
                    Assert.AreNotSame(view[0], view[1]);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Value));

                    using (var vmView = view.AsMappingView(Vm.Create, leaveOpen: true))
                    {
                        Assert.AreNotSame(view[0], view[1]);
                        CollectionAssert.AreEqual(view, vmView.Select(x => x.Model));
                    }

                    using (var indexedView = view.AsMappingView(Vm.Create, (x, i) => x, leaveOpen: true))
                    {
                        CollectionAssert.AreEqual(view, indexedView.Select(x => x.Model));
                        CollectionAssert.AreEqual(Enumerable.Range(0, 9), indexedView.Select(x => x.Index));
                    }
                }
            }

            [Test]
            public void InitializesReferenceType()
            {
                var source = new ObservableCollection<Model<int>> { Model.Create(1), Model.Create(2), null, null, Model.Create(3) };
                using (var view = source.AsMappingView(Vm.Create))
                {
                    Assert.AreNotSame(view[0], view[1]);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Value));
                }
            }

            [Test]
            public void UpdatesValueType()
            {
                var source = new ObservableCollection<int>();
                using (var view = source.AsMappingView(x => x * x))
                {
                    source.Add(1);
                    CollectionAssert.AreEqual(new[] { 1 }, view);

                    source.Add(2);
                    CollectionAssert.AreEqual(new[] { 1, 4 }, view);

                    source.Move(1, 0);
                    CollectionAssert.AreEqual(new[] { 4, 1 }, view);

                    source.Clear();
                    CollectionAssert.IsEmpty(view);

                    source.Add(3);
                    CollectionAssert.AreEqual(new[] { 9 }, view);
                }
            }

            [Test]
            public void DoesNotCacheValueTypes()
            {
                var source = new ObservableCollection<int>();
                using (var view = source.AsMappingView(Model.Create))
                {
                    source.Add(1);
                    Assert.AreEqual(1, view.Count);

                    source.Add(1);
                    Assert.AreEqual(2, view.Count);
                    Assert.AreNotSame(view[0], view[1]);
                    Assert.AreEqual(1, view[0].Value);
                    Assert.AreEqual(1, view[1].Value);

                    var vm = view[0];
                    source.Clear();
                    CollectionAssert.IsEmpty(view);
                    source.Add(1);
                    Assert.AreNotSame(vm, view[0]);
                    Assert.AreEqual(1, view[0].Value);
                }
            }

            [Test]
            public void Add()
            {
                var source = new ObservableCollection<int> { 1, 2, 3 };
                using (var view = source.AsMappingView(Model.Create))
                {
                    CollectionAssert.AreEqual(source, view.Select(x => x.Value));

                    using (var actual = view.SubscribeAll())
                    {
                        source.Add(4);
                        var expected = new List<EventArgs>
                                           {
                                               CachedEventArgs.CountPropertyChanged,
                                               CachedEventArgs.IndexerPropertyChanged,
                                               Diff.CreateAddEventArgs(view[3], 3)
                                           };

                        CollectionAssert.AreEqual(source, view.Select(x => x.Value));
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                        source.Add(5);
                        expected.AddRange(
                            new EventArgs[]
                                {
                                    CachedEventArgs.CountPropertyChanged,
                                    CachedEventArgs.IndexerPropertyChanged,
                                    Diff.CreateAddEventArgs(view[4], 4)
                                });

                        CollectionAssert.AreEqual(source, view.Select(x => x.Value));
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    }
                }
            }

            [Test]
            public void Remove()
            {
                var source = new ObservableCollection<int> { 1 };
                using (var modelView = source.AsMappingView(Model.Create))
                {
                    using (var modelViewChanges = modelView.SubscribeAll())
                    {
                        using (var vmView = modelView.AsMappingView(Vm.Create))
                        {
                            using (var vmViewChanges = vmView.SubscribeAll())
                            {
                                var oldModel = modelView[0];
                                var oldView = vmView[0];
                                source.Remove(1);

                                var expected = new EventArgs[]
                                                   {
                                                   CachedEventArgs.CountPropertyChanged,
                                                   CachedEventArgs.IndexerPropertyChanged,
                                                   null
                                                   };
                                expected[2] = Diff.CreateRemoveEventArgs(oldModel, 0);
                                CollectionAssert.AreEqual(expected, modelViewChanges, EventArgsComparer.Default);
                                expected[2] = Diff.CreateRemoveEventArgs(oldView, 0);
                                CollectionAssert.AreEqual(expected, vmViewChanges, EventArgsComparer.Default);
                            }
                        }
                    }
                }
            }

            [Test]
            public void Replace()
            {
                var source = new ObservableCollection<int>
                {
                    1,
                    1,
                    1,
                    2,
                    2,
                    2,
                    3,
                    3,
                    3,
                };
                using (var view = source.AsMappingView(x => x * x))
                {
                    using (var actual = view.SubscribeAll())
                    {
                        CollectionAssert.AreEqual(source.Select(x => x * x), view);

                        source[0] = 5;
                        CollectionAssert.AreEqual(source.Select(x => x * x), view);
                        var expected = new List<EventArgs>
                        {
                            CachedEventArgs.IndexerPropertyChanged,
                            Diff.CreateReplaceEventArgs(view[0], 1, 0)
                        };
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    }
                }
            }

            [TestCase(0, 1)]
            [TestCase(1, 0)]
            [TestCase(0, 2)]
            [TestCase(2, 0)]
            [TestCase(1, 2)]
            [TestCase(2, 1)]
            public void Move(int from, int to)
            {
                var source = new ObservableCollection<int>
                {
                    1,
                    2,
                    3,
                };
                using (var view = source.AsMappingView(x => x * x))
                {
                    using (var actual = view.SubscribeAll())
                    {
                        CollectionAssert.AreEqual(source.Select(x => x * x), view);

                        source.Move(from, to);
                        CollectionAssert.AreEqual(source.Select(x => x * x), view);
                        var expected = new List<EventArgs>
                        {
                            CachedEventArgs.IndexerPropertyChanged,
                            Diff.CreateMoveEventArgs(view[to], to, from)
                        };
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    }
                }
            }
        }
    }
}