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
        public class ReferenceType
        {
            [Test]
            public void Initializes()
            {
                var model1 = Model.Create(1);
                var model2 = Model.Create(2);
                var model3 = Model.Create(3);
                var source = new ObservableCollection<Model<int>>(
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
                using (var view = source.AsMappingView(Vm.Create))
                {
                    Assert.AreSame(view[0], view[1]);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                }
            }

            [Test]
            public void Updates()
            {
                var source = new ObservableCollection<Model<int>>();
                using (var view = source.AsMappingView(Vm.Create))
                {
                    var model = Model.Create(1);
                    source.Add(model);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Model));

                    source.Clear();
                    CollectionAssert.IsEmpty(view);

                    source.Add(model);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                }
            }

            [Test]
            public void Refresh()
            {
                var model1 = Model.Create(1);
                var model2 = Model.Create(2);
                var model3 = Model.Create(3);
                var source = new ObservableBatchCollection<Model<int>>(
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
                using (var view = source.AsMappingView(Vm.Create))
                {
                    using (var expected = source.SubscribeAll())
                    {
                        using (var actual = view.SubscribeAll())
                        {
                            CollectionAssert.IsEmpty(actual);
                            CollectionAssert.AreEqual(source, view.Select(x => x.Model));

                            source.AddRange(new[] { model1, model2, model2 });
                            CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                            CollectionAssert.AreEqual(expected, actual);

                            source.Clear();
                            CollectionAssert.IsEmpty(view);
                            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                        }
                    }
                }
            }

            [Test]
            public void Caches()
            {
                var source = new ObservableCollection<Model<int>>();
                using (var view = source.AsMappingView(Vm.Create))
                {
                    var model = Model.Create(1);
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
            }

            [Test]
            public void CachesWhenNotEmpty()
            {
                var model1 = Model.Create(1);
                var model2 = Model.Create(2);
                var model3 = Model.Create(3);
                var source = new ObservableCollection<Model<int>>(
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

                using (var view = source.AsMappingView(Vm.Create))
                {
                    CollectionAssert.AreEqual(source, view.Select(x => x.Model));

                    var model4 = Model.Create(4);
                    source.Add(model4);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Model));

                    source.Add(model4);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Model));

                    source.Add(model1);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Model));

                    source.Clear();
                    CollectionAssert.IsEmpty(view);
                }
            }

            [Test]
            public void Add()
            {
                var source = new ObservableCollection<Model<int>>();
                using (var view = source.AsMappingView(Vm.Create))
                {
                    using (var actual = view.SubscribeAll())
                    {
                        var model1 = Model.Create(1);
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

                        source.Add(Model.Create(2));
                        CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                        expected.AddRange(new EventArgs[]
                        {
                            CachedEventArgs.CountPropertyChanged,
                            CachedEventArgs.IndexerPropertyChanged,
                            Diff.CreateAddEventArgs(view[2], 2),
                        });
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    }
                }
            }

            [Test]
            public void Remove()
            {
                var model1 = Model.Create(1);
                var model2 = Model.Create(2);
                var model3 = Model.Create(3);
                var source = new ObservableCollection<Model<int>>(
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
                using (var view = source.AsMappingView(Vm.Create))
                {
                    using (var actual = view.SubscribeAll())
                    {
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
                }
            }

            [Test]
            public void Replace()
            {
                var model1 = Model.Create(1);
                var model2 = Model.Create(2);
                var model3 = Model.Create(3);
                var source = new ObservableCollection<Model<int>>(
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
                using (var view = source.AsMappingView(Vm.Create))
                {
                    using (var actual = view.SubscribeAll())
                    {
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
                }
            }

            [Test]
            public void Move()
            {
                var model1 = Model.Create(1);
                var model2 = Model.Create(2);
                var model3 = Model.Create(3);
                var source = new ObservableCollection<Model<int>>(
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
                using (var view = source.AsMappingView(Vm.Create))
                {
                    using (var actual = view.SubscribeAll())
                    {
                        source.Move(0, 4);
                        CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                        var expected = new List<EventArgs>
                        {
                            CachedEventArgs.IndexerPropertyChanged,
                            Diff.CreateMoveEventArgs(view[4], 4, 0),
                        };
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    }
                }
            }

            [Test]
            public void Clear()
            {
                var model1 = Model.Create(1);
                var model2 = Model.Create(2);
                var model3 = Model.Create(3);
                var source = new ObservableCollection<Model<int>>(
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
                using (var view = source.AsMappingView(Vm.Create))
                {
                    using (var actual = view.SubscribeAll())
                    {
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
    }
}