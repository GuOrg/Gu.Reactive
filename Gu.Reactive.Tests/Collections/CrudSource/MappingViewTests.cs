// ReSharper disable All
namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public partial class MappingViewTests
    {
        [Test]
        public void InitializesValueType()
        {
            var ints = new ObservableCollection<int>(new[] { 1, 1, 1, 2, 2, 2, 3, 3, 3 });
            using (var view = ints.AsMappingView(x => new Model(x)))
            {
                Assert.AreNotSame(view[0], view[1]);
                CollectionAssert.AreEqual(ints, view.Select(x => x.Value));

                using (var vmView = view.AsMappingView(x => new Vm { Model = x }))
                {
                    Assert.AreNotSame(view[0], view[1]);
                    CollectionAssert.AreEqual(view, vmView.Select(x => x.Model));
                }

                using (var indexedView = view.AsMappingView((x, i) => new Vm { Model = x, Index = i }, (x, i) => x))
                {
                    CollectionAssert.AreEqual(view, indexedView.Select(x => x.Model));
                    CollectionAssert.AreEqual(Enumerable.Range(0, 9), indexedView.Select(x => x.Index));
                }
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
            var models = new ObservableCollection<int>();
            using (var view = models.AsMappingView(x => new Vm { Value = x }))
            {
                models.Add(1);
                Assert.AreEqual(1, view.Count);

                models.Add(1);
                Assert.AreEqual(2, view.Count);
                Assert.AreNotSame(view[0], view[1]);
                Assert.AreEqual(1, view[0].Value);
                Assert.AreEqual(1, view[1].Value);

                var vm = view[0];
                models.Clear();
                CollectionAssert.IsEmpty(view);
                models.Add(1);
                Assert.AreNotSame(vm, view[0]);
                Assert.AreEqual(1, view[0].Value);
            }
        }

        [Test]
        public void NotifiesAdd()
        {
            var ints = new ObservableCollection<int>();
            using (var modelView = ints.AsMappingView(x => new Model(x)))
            {
                using (var modelViewChanges = modelView.SubscribeAll())
                {
                    using (var indexedView = modelView.AsMappingView((x, i) => new Vm { Model = x, Index = i }, (x, i) => x))
                    {
                        using (var indexedChanges = indexedView.SubscribeAll())
                        {
                            using (var vmView = modelView.AsMappingView(x => new Vm { Model = x }))
                            {
                                using (var vmViewChanges = vmView.SubscribeAll())
                                {
                                    ints.Add(1);
                                    var expected = new EventArgs[]
                                                       {
                                                           CachedEventArgs.CountPropertyChanged,
                                                           CachedEventArgs.IndexerPropertyChanged,
                                                           null
                                                       };

                                    expected[2] = Diff.CreateAddEventArgs(modelView[0], 0);
                                    CollectionAssert.AreEqual(expected, modelViewChanges, EventArgsComparer.Default);

                                    expected[2] = Diff.CreateAddEventArgs(vmView[0], 0);
                                    CollectionAssert.AreEqual(expected, vmViewChanges, EventArgsComparer.Default);

                                    expected[2] = Diff.CreateAddEventArgs(indexedView[0], 0);
                                    CollectionAssert.AreEqual(expected, indexedChanges, EventArgsComparer.Default);
                                }
                            }
                        }
                    }
                }
            }
        }

        [Test]
        public void NotifiesRemove()
        {
            var ints = new ObservableCollection<int>(new[] { 1 });
            using (var modelView = ints.AsMappingView(x => new Model(x)))
            {
                using (var modelViewChanges = modelView.SubscribeAll())
                {
                    using (var vmView = modelView.AsMappingView(x => new Vm { Model = x }))
                    {
                        using (var vmViewChanges = vmView.SubscribeAll())
                        {
                            var oldModel = modelView[0];
                            var oldView = vmView[0];
                            ints.Remove(1);

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

        private class Model
        {
            public Model(int value)
            {
                this.Value = value;
            }

            public int Value { get; }

            public int Index { get; set; }
        }

        private class Vm
        {
            public Model Model { get; set; }

            public int Value { get; set; }

            public int Index { get; set; }
        }
    }
}
