namespace Gu.Reactive.Tests.Collections
{
    using System.Collections.ObjectModel;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class MappingView_IndexedTests
    {
        [Test]
        public void Initializes()
        {
            var models = new ObservableCollection<Model>(new[] { new Model(1, -1), new Model(2, -1), });
            var indexed = models.AsMappingView((x, i) => new Vm { Index = i, Model = x });
            var indexedUpdating = models.AsMappingView((x, i) => new Vm { Index = i, Model = x }, (x, i) => x.UpdateIndex(i));
            var indexedNewing = models.AsMappingView((x, i) => new Vm { Index = i, Model = x }, (x, i) => new Vm { Model = x.Model, Index = i });
            var views = new[]
            {
                indexed,
                indexedUpdating,
                indexedNewing
            };
            foreach (var view in views)
            {
                Assert.AreSame(models[0], view[0].Model);
                Assert.AreEqual(0, view[0].Index);

                Assert.AreSame(models[1], view[1].Model);
                Assert.AreEqual(1, view[1].Index);
            }
        }

        [TestCase(0)]
        [TestCase(1)]
        public void UpdatesOnRemoveTwoItems(int removeAt)
        {
            var models = new ObservableCollection<Model>(new[] { new Model(1, -1), new Model(2, -1) });
            var view = models.AsMappingView((x, i) => new Vm { Index = i, Model = x }, (x, i) => x.UpdateIndex(i));
            var old = view[removeAt == 0 ? 1 : 0];

            models.RemoveAt(removeAt);

            Assert.AreEqual(1, view.Count);
            Assert.AreSame(models[0], view[0].Model);
            Assert.AreSame(old, view[0]);
            Assert.AreEqual(0, view[0].Index);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void UpdatesOnRemoveThreeItems(int removeAt)
        {
            var models = new ObservableCollection<Model>(new[] { new Model(1, -1), new Model(2, -1), new Model(3, -1) });
            var view = models.AsMappingView((x, i) => new Vm { Index = i, Model = x }, (x, i) => x.UpdateIndex(i));
            var old = view[2];

            models.RemoveAt(removeAt);

            Assert.AreEqual(2, view.Count);
            Assert.AreSame(models[0], view[0].Model);
            Assert.AreSame(old, view[1]);
            Assert.AreEqual(0, view[0].Index);
            Assert.AreEqual(1, view[1].Index);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void NewsOnRemoveTwoItems(int removeAt)
        {
            var models = new ObservableCollection<Model>(new[] { new Model(1, -1), new Model(2, -1) });
            var view = models.AsMappingView((x, i) => new Vm { Index = i, Model = x }, (x, i) => new Vm { Model = x.Model, Index = i });

            models.RemoveAt(removeAt);

            Assert.AreEqual(1, view.Count);
            Assert.AreSame(models[0], view[0].Model);
            Assert.AreEqual(0, view[0].Index);
        }

        private class Model
        {
            public Model(int value, int index)
            {
                Value = value;
                Index = index;
            }

            public int Value { get; set; }

            public int Index { get; set; }
        }

        private class Vm
        {
            public Model Model { get; set; }

            public int Value { get; set; }

            public int Index { get; set; }

            public Vm UpdateIndex(int i)
            {
                Index = i;
                return this;
            }
        }
    }


    public class MappingViewTests
    {
        [Test]
        public void Initializes()
        {
            var ints = new ObservableCollection<int>(new[] { 1 });
            var modelView = ints.AsMappingView(x => new Model(x));

            Assert.AreEqual(1, modelView.Count);
            Assert.AreEqual(1, modelView[0].Value);

            var vmView = modelView.AsMappingView(x => new Vm { Model = x });
            Assert.AreEqual(1, vmView.Count);
            Assert.AreSame(modelView[0], vmView[0].Model);

            var indexedVmView = modelView.AsMappingView((x, i) => new Vm { Model = x, Index = i });
            Assert.AreEqual(1, indexedVmView.Count);
            Assert.AreSame(modelView[0], indexedVmView[0].Model);
            Assert.AreEqual(0, indexedVmView[0].Index);
        }

        [Test]
        public void Updates()
        {
            var models = new ObservableCollection<Model>();
            var view = models.AsMappingView((x, i) => new Vm { Model = x, Index = i });
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

        [Test]
        public void CachesRefTypes()
        {
            var models = new ObservableCollection<Model>();
            var view = models.AsMappingView(x => new Vm { Model = x });
            var model = new Model(1);
            models.Add(model);
            var vm = view[0];
            models.Clear();
            CollectionAssert.IsEmpty(view);
            models.Add(model);
            Assert.AreSame(vm, view[0]);
            Assert.AreSame(vm.Model, view[0].Model);
        }

        [Test]
        public void DoesNotCacheValueTypes()
        {
            var models = new ObservableCollection<int>();
            var view = models.AsMappingView(x => new Vm { Value = x });
            models.Add(1);
            var vm = view[0];
            models.Clear();
            CollectionAssert.IsEmpty(view);
            models.Add(1);
            Assert.AreNotSame(vm, view[0]);
            Assert.AreEqual(1, view[0].Value);
        }

        [Test]
        public void NotifiesAdd()
        {
            var ints = new ObservableCollection<int>();
            var expected = ints.SubscribeAll();
            var modelView = ints.AsMappingView(x => new Model(x));
            var modelViewChanges = modelView.SubscribeAll();

            var indexedView = modelView.AsMappingView((x, i) => new Vm { Model = x, Index = i });
            var indexedChanges = indexedView.SubscribeAll();

            var vmView = modelView.AsMappingView(x => new Vm { Model = x });
            var vmViewChanges = vmView.SubscribeAll();

            ints.Add(1);

            expected[2] = Diff.CreateAddEventArgs(modelView[0], 0);
            CollectionAssert.AreEqual(expected, modelViewChanges, EventArgsComparer.Default);
            expected[2] = Diff.CreateAddEventArgs(vmView[0], 0);
            CollectionAssert.AreEqual(expected, vmViewChanges, EventArgsComparer.Default);

            expected[2] = Diff.CreateAddEventArgs(indexedView[0], 0);
            CollectionAssert.AreEqual(expected, indexedChanges, EventArgsComparer.Default);
        }

        [Test]
        public void NotifiesRemove()
        {
            var ints = new ObservableCollection<int>(new[] { 1 });
            var expected = ints.SubscribeAll();
            var modelView = ints.AsMappingView(x => new Model(x));
            var modelViewChanges = modelView.SubscribeAll();
            var vmView = modelView.AsMappingView(x => new Vm { Model = x });
            var vmViewChanges = vmView.SubscribeAll();

            var oldModel = modelView[0];
            var oldView = vmView[0];
            ints.Remove(1);

            expected[2] = Diff.CreateRemoveEventArgs(oldModel, 0);
            CollectionAssert.AreEqual(expected, modelViewChanges, EventArgsComparer.Default);
            expected[2] = Diff.CreateRemoveEventArgs(oldView, 0);
            CollectionAssert.AreEqual(expected, vmViewChanges, EventArgsComparer.Default);
        }

        private class Model
        {
            public Model(int value)
            {
                Value = value;
            }

            public int Value { get; set; }

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
