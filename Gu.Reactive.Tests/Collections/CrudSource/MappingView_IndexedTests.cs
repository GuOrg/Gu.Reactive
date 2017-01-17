namespace Gu.Reactive.Tests.Collections
{
    using System.Collections.ObjectModel;

    using NUnit.Framework;

    public class MappingView_IndexedTests
    {
        [Test]
        public void Initializes()
        {
            var models = new ObservableCollection<Model>(new[] { new Model(1, -1), new Model(2, -1), });
            using (var indexed = models.AsMappingView((x, i) => new Vm { Index = i, Model = x }))
            {
                using (var indexedUpdating = models.AsMappingView((x, i) => new Vm { Index = i, Model = x }, (x, i) => x.UpdateIndex(i)))
                {
                    using (var indexedNewing = models.AsMappingView((x, i) => new Vm { Index = i, Model = x }, (x, i) => new Vm { Model = x.Model, Index = i }))
                    {
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
                }
            }
        }

        [TestCase(0)]
        [TestCase(1)]
        public void UpdatesOnRemoveTwoItems(int removeAt)
        {
            var models = new ObservableCollection<Model>(new[] { new Model(1, -1), new Model(2, -1) });
            using (var view = models.AsMappingView((x, i) => new Vm { Index = i, Model = x }, (x, i) => x.UpdateIndex(i)))
            {
                var old = view[removeAt == 0 ? 1 : 0];

                models.RemoveAt(removeAt);

                Assert.AreEqual(1, view.Count);
                Assert.AreSame(models[0], view[0].Model);
                Assert.AreSame(old, view[0]);
                Assert.AreEqual(0, view[0].Index);
            }
        }

        [TestCase(0)]
        [TestCase(1)]
        public void UpdatesOnRemoveThreeItems(int removeAt)
        {
            var models = new ObservableCollection<Model>(new[] { new Model(1, -1), new Model(2, -1), new Model(3, -1) });
            using (var view = models.AsMappingView((x, i) => new Vm { Index = i, Model = x }, (x, i) => x.UpdateIndex(i)))
            {
                var old = view[2];

                models.RemoveAt(removeAt);

                Assert.AreEqual(2, view.Count);
                Assert.AreSame(models[0], view[0].Model);
                Assert.AreSame(old, view[1]);
                Assert.AreEqual(0, view[0].Index);
                Assert.AreEqual(1, view[1].Index);
            }
        }

        [TestCase(0)]
        [TestCase(1)]
        public void NewsOnRemoveTwoItems(int removeAt)
        {
            var models = new ObservableCollection<Model>(new[] { new Model(1, -1), new Model(2, -1) });
            using (var view = models.AsMappingView((x, i) => new Vm { Index = i, Model = x }, (x, i) => new Vm { Model = x.Model, Index = i }))
            {
                models.RemoveAt(removeAt);

                Assert.AreEqual(1, view.Count);
                Assert.AreSame(models[0], view[0].Model);
                Assert.AreEqual(0, view[0].Index);
            }
        }

        private class Model
        {
            public Model(int value, int index)
            {
                this.Value = value;
                this.Index = index;
            }

            public int Value { get; set; }

            public int Index { get; set; }
        }

        private class Vm
        {
            public Model Model { get; set; }

            public int Index { get; set; }

            public Vm UpdateIndex(int i)
            {
                this.Index = i;
                return this;
            }
        }
    }
}