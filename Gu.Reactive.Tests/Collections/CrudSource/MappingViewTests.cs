namespace Gu.Reactive.Tests.Collections.CrudSource
{
    using System.Collections.ObjectModel;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    class MappingViewTests
    {
        [Test]
        public void Updates()
        {
            var models = new ObservableCollection<Model>();
            var view = models.AsMappingView(x => new Vm { Model = x });
            var model = new Model(1);
            models.Add(model);
            Assert.AreEqual(1, view.Count);
            Assert.AreEqual(1, view[0].Model.Value);
            var vm = view[0];
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
        public void Notifies()
        {
            var models = new ObservableCollection<Model>();
            var expected = models.SubscribeAll();
            var view = models.AsMappingView(x => new Vm { Model = x });
            var actual = view.SubscribeAll();
            models.Add(new Model(1));
            expected[2] = Diff.CreateAddEventArgs(view[0], 0);
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        private class Model
        {
            public Model(int value)
            {
                Value = value;
            }

            public int Value { get; set; }
        }

        private class Vm
        {
            public Model Model { get; set; }
            public int Value { get; set; }
        }
    }
}
