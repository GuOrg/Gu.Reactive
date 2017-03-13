namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using NUnit.Framework;

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    public partial class MappingViewTests
    {
        public class CreatingUpdating
        {
            [Test]
            public void Initializes()
            {
                var source = new ObservableCollection<Model>(new[] { new Model(1), new Model(2), });
                using (var indexed = source.AsMappingView(
                    (x, i) => new Vm { Index = i, Model = x },
                    (x, i) => x.WithIndex(i)))
                {
                    using (var indexedUpdating = source.AsMappingView(
                            (x, i) => new Vm { Index = i, Model = x },
                            (x, i) => x.WithIndex(i)))
                    {
                        using (var indexedNewing = source.AsMappingView(
                                (x, i) => new Vm { Index = i, Model = x },
                                (x, i) => new Vm { Model = x.Model, Index = i }))
                        {
                            var views = new[]
                                            {
                                                indexed,
                                                indexedUpdating,
                                                indexedNewing
                                            };
                            foreach (var view in views)
                            {
                                CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));
                            }
                        }
                    }
                }
            }

            [Test]
            public void UpdatesValueType()
            {
                var source = new ObservableCollection<double>();
                using (var view = source.AsMappingView(
                    (x, i) => i + 1 + x,
                    (x, i) => i + 1 + Math.Round(x - (int)x, 1)))
                {
                    source.Add(0.1);
                    CollectionAssert.AreEqual(new[] { 1.1 }, view);

                    source.Add(0.2);
                    CollectionAssert.AreEqual(new[] { 1.1, 2.2 }, view);

                    source.Move(1, 0);
                    CollectionAssert.AreEqual(new[] { 1.2, 2.1 }, view);

                    source.Clear();
                    CollectionAssert.IsEmpty(view);

                    source.Add(0.3);
                    CollectionAssert.AreEqual(new[] { 1.3 }, view);
                }
            }

            [Test]
            public void UpdatesReferenceType()
            {
                var source = new ObservableCollection<Model>();
                using (var view = source.AsMappingView(
                    (x, i) => new Vm { Model = x, Index = i },
                    (x, i) => x.WithIndex(i)))
                {
                    CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                    CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));

                    var model = new Model(1);
                    source.Add(model);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                    CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));

                    source.Clear();
                    CollectionAssert.IsEmpty(view);
                    source.Add(model);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                    CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));
                }
            }

            [Test]
            public void UpdatesDifferentReferenceType()
            {
                var source = new ObservableCollection<Model>();
                using (var view = source.AsMappingView(
                    (x, i) => new Vm(x, i),
                    (x, i) => x.WithIndex(i)))
                {
                    source.Add(new Model(1));
                    CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                    CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));

                    source.Add(new Model(1));
                    Assert.AreNotSame(view[0], view[1]);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                    CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));

                    source.Move(1, 0);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                    CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));

                    source.Clear();
                    CollectionAssert.IsEmpty(view);

                    source.Add(new Model(3));
                    CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                    CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));
                }
            }

            [Test]
            public void UpdatesSameReferenceType()
            {
                var source = new ObservableCollection<Model>();
                using (var view = source.AsMappingView(
                    (x, i) => new Vm(x, i),
                    (x, i) => x.WithIndex(i)))
                {
                    var model = new Model(1);
                    source.Add(model);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                    CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));

                    source.Add(model);
                    Assert.AreNotSame(view[0], view[1]);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                    CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));

                    source.Move(1, 0);
                    Assert.AreNotSame(view[0], view[1]);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                    CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));

                    source.Clear();
                    CollectionAssert.IsEmpty(view);

                    source.Add(model);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                    CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));
                }
            }

            [TestCase(0)]
            [TestCase(1)]
            public void UpdatesOnRemoveTwoItems(int removeAt)
            {
                var models = new ObservableCollection<Model>(new[] { new Model(1), new Model(2) });
                using (var view = models.AsMappingView(
                        (x, i) => new Vm { Index = i, Model = x },
                        (x, i) => x.WithIndex(i)))
                {
                    var old = view[removeAt == 0
                                       ? 1
                                       : 0];

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
                var models =
                    new ObservableCollection<Model>(new[] { new Model(1), new Model(2), new Model(3) });
                using (
                    var view = models.AsMappingView(
                        (x, i) => new Vm { Index = i, Model = x },
                        (x, i) => x.WithIndex(i)))
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
                var models = new ObservableCollection<Model>(new[] { new Model(1), new Model(2) });
                using (
                    var view = models.AsMappingView(
                        (x, i) => new Vm { Index = i, Model = x },
                        (x, i) => new Vm { Model = x.Model, Index = i }))
                {
                    models.RemoveAt(removeAt);

                    Assert.AreEqual(1, view.Count);
                    Assert.AreSame(models[0], view[0].Model);
                    Assert.AreEqual(0, view[0].Index);
                }
            }
        }
    }
}