// ReSharper disable All
namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class MappingViewTests
    {
        [Test]
        public void Initializes()
        {
            var ints = new ObservableCollection<int>(new[] { 1 });
            using (var modelView = ints.AsMappingView(x => new Model(x)))
            {
                Assert.AreEqual(1, modelView.Count);
                Assert.AreEqual(1, modelView[0].Value);

                using (var vmView = modelView.AsMappingView(x => new Vm { Model = x }))
                {
                    Assert.AreEqual(1, vmView.Count);
                    Assert.AreSame(modelView[0], vmView[0].Model);
                }

                using (var indexedVmView = modelView.AsMappingView((x, i) => new Vm { Model = x, Index = i }))
                {
                    Assert.AreEqual(1, indexedVmView.Count);
                    Assert.AreSame(modelView[0], indexedVmView[0].Model);
                    Assert.AreEqual(0, indexedVmView[0].Index);
                }
            }
        }

        [Test]
        public void Updates()
        {
            var models = new ObservableCollection<Model>();
            using (var view = models.AsMappingView((x, i) => new Vm { Model = x, Index = i }))
            {
                var model = new Model(1);
                models.Add(model);
                Assert.AreEqual(1, view.Count);
                Assert.AreEqual(1, view[0].Model.Value);
                Assert.AreEqual(0, view[0].Index);
                models.Clear();
                CollectionAssert.IsEmpty(view);
                models.Add(model);
                Assert.AreEqual(1, view.Count);
            }
        }

        [Test]
        public void CachesRefTypes()
        {
            var models = new ObservableCollection<Model>();
            using (var view = models.AsMappingView(x => new Vm { Model = x }))
            {
                var model = new Model(1);
                models.Add(model);
                var vm = view[0];
                models.Clear();
                CollectionAssert.IsEmpty(view);
                models.Add(model);
                Assert.AreSame(vm, view[0]);
                Assert.AreSame(vm.Model, view[0].Model);
            }
        }

        [Test]
        public void DoesNotCacheValueTypes()
        {
            var models = new ObservableCollection<int>();
            using (var view = models.AsMappingView(x => new Vm { Value = x }))
            {
                models.Add(1);
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
            var expected = ints.SubscribeAll();
            using (var modelView = ints.AsMappingView(x => new Model(x)))
            {
                var modelViewChanges = modelView.SubscribeAll();

                using (var indexedView = modelView.AsMappingView((x, i) => new Vm { Model = x, Index = i }))
                {
                    var indexedChanges = indexedView.SubscribeAll();

                    List<EventArgs> vmViewChanges;
                    using (var vmView = modelView.AsMappingView(x => new Vm { Model = x }))
                    {
                        vmViewChanges = vmView.SubscribeAll();

                        ints.Add(1);

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

        [Test]
        public void NotifiesRemove()
        {
            var ints = new ObservableCollection<int>(new[] { 1 });
            var expected = ints.SubscribeAll();
            using (var modelView = ints.AsMappingView(x => new Model(x)))
            {
                var modelViewChanges = modelView.SubscribeAll();
                using (var vmView = modelView.AsMappingView(x => new Vm { Model = x }))
                {
                    var vmViewChanges = vmView.SubscribeAll();
                    var oldModel = modelView[0];
                    var oldView = vmView[0];
                    ints.Remove(1);

                    expected[2] = Diff.CreateRemoveEventArgs(oldModel, 0);
                    CollectionAssert.AreEqual(expected, modelViewChanges, EventArgsComparer.Default);
                    expected[2] = Diff.CreateRemoveEventArgs(oldView, 0);
                    CollectionAssert.AreEqual(expected, vmViewChanges, EventArgsComparer.Default);
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
