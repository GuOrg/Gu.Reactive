namespace Gu.Reactive.Tests.ReadOnlyViews
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using NUnit.Framework;

    public static partial class MappingViewTests
    {
        public static class CreatingUpdating
        {
            [Test]
            public static void Initializes()
            {
                var source = new ObservableCollection<Collections.ReadOnlyViews.MappingViewTests.Model<int>> { Collections.ReadOnlyViews.MappingViewTests.Model.Create(1), Collections.ReadOnlyViews.MappingViewTests.Model.Create(2), };
                using var indexed = source.AsMappingView(
                    Collections.ReadOnlyViews.MappingViewTests.Vm.Create,
                    (x, i) => x.WithIndex(i));
                using var indexedUpdating = source.AsMappingView(
                    Collections.ReadOnlyViews.MappingViewTests.Vm.Create,
                    (x, i) => x.WithIndex(i));
                using var indexedNewing = source.AsMappingView(
                    Collections.ReadOnlyViews.MappingViewTests.Vm.Create,
                    (x, i) => Collections.ReadOnlyViews.MappingViewTests.Vm.Create(x.Model, i));
                var views = new[]
                {
                    indexed,
                    indexedUpdating,
                    indexedNewing,
                };
                foreach (var view in views)
                {
                    CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                    CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));
                }
            }

            [Test]
            public static void UpdatesValueType()
            {
                var source = new ObservableCollection<double>();
                using var view = source.AsMappingView(
                    (x, i) => i + 1 + x,
                    (x, i) => i + 1 + Math.Round(x - (int)x, 1));
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

            [Test]
            public static void UpdatesReferenceType()
            {
                var source = new ObservableCollection<Collections.ReadOnlyViews.MappingViewTests.Model<int>>();
                using var view = source.AsMappingView(
                    Collections.ReadOnlyViews.MappingViewTests.Vm.Create,
                    (x, i) => x.WithIndex(i));
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));

                var model = Collections.ReadOnlyViews.MappingViewTests.Model.Create(1);
                source.Add(model);
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));

                source.Clear();
                CollectionAssert.IsEmpty(view);
                source.Add(model);
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));
            }

            [Test]
            public static void UpdatesReferenceTypeNulls()
            {
                var source = new ObservableCollection<Collections.ReadOnlyViews.MappingViewTests.Model<int>?>();
                using var view = source.AsMappingView(
                    Collections.ReadOnlyViews.MappingViewTests.Vm.Create,
                    (x, i) => x.WithIndex(i));
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));

                var model = Collections.ReadOnlyViews.MappingViewTests.Model.Create(1);
                source.Add(model);
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));

                source.Add(null);
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));

                source.Add(null);
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));
                Assert.AreNotSame(view[1], view[2]);

                source.Clear();
                CollectionAssert.IsEmpty(view);
                source.Add(model);
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));
            }

            [Test]
            public static void UpdatesDifferentReferenceType()
            {
                var source = new ObservableCollection<Collections.ReadOnlyViews.MappingViewTests.Model<int>>();
                using var view = source.AsMappingView(
                    Collections.ReadOnlyViews.MappingViewTests.Vm.Create,
                    (x, i) => x.WithIndex(i));
                source.Add(Collections.ReadOnlyViews.MappingViewTests.Model.Create(1));
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));

                source.Add(Collections.ReadOnlyViews.MappingViewTests.Model.Create(1));
                Assert.AreNotSame(view[0], view[1]);
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));

                source.Move(1, 0);
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));

                source.Clear();
                CollectionAssert.IsEmpty(view);

                source.Add(Collections.ReadOnlyViews.MappingViewTests.Model.Create(3));
                CollectionAssert.AreEqual(source, view.Select(x => x.Model));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Index));
            }

            [Test]
            public static void UpdatesSameReferenceType()
            {
                var source = new ObservableCollection<Collections.ReadOnlyViews.MappingViewTests.Model<int>>();
                using var view = source.AsMappingView(
                    Collections.ReadOnlyViews.MappingViewTests.Vm.Create,
                    (x, i) => x.WithIndex(i));
                var model = Collections.ReadOnlyViews.MappingViewTests.Model.Create(1);
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

            [TestCase(0)]
            [TestCase(1)]
            public static void UpdatesOnRemoveTwoItems(int removeAt)
            {
                var source = new ObservableCollection<Collections.ReadOnlyViews.MappingViewTests.Model<int>> { Collections.ReadOnlyViews.MappingViewTests.Model.Create(1), Collections.ReadOnlyViews.MappingViewTests.Model.Create(2) };
                using var view = source.AsMappingView(
                    Collections.ReadOnlyViews.MappingViewTests.Vm.Create,
                    (x, i) => x.WithIndex(i));
                var old = removeAt == 0
                    ? view[1]
                    : view[0];

                source.RemoveAt(removeAt);

                Assert.AreEqual(1, view.Count);
                Assert.AreSame(source[0], view[0].Model);
                Assert.AreSame(old, view[0]);
                Assert.AreEqual(0, view[0].Index);
            }

            [TestCase(0)]
            [TestCase(1)]
            public static void UpdatesOnRemoveThreeItems(int removeAt)
            {
                var source = new ObservableCollection<Collections.ReadOnlyViews.MappingViewTests.Model<int>> { Collections.ReadOnlyViews.MappingViewTests.Model.Create(1), Collections.ReadOnlyViews.MappingViewTests.Model.Create(2), Collections.ReadOnlyViews.MappingViewTests.Model.Create(3) };
                using var view = source.AsMappingView(
                    Collections.ReadOnlyViews.MappingViewTests.Vm.Create,
                    (x, i) => x.WithIndex(i));
                var old = view[2];

                source.RemoveAt(removeAt);

                Assert.AreEqual(2, view.Count);
                Assert.AreSame(source[0], view[0].Model);
                Assert.AreSame(old, view[1]);
                Assert.AreEqual(0, view[0].Index);
                Assert.AreEqual(1, view[1].Index);
            }

            [TestCase(0)]
            [TestCase(1)]
            public static void NewsOnRemoveTwoItems(int removeAt)
            {
                var source = new ObservableCollection<Collections.ReadOnlyViews.MappingViewTests.Model<int>> { Collections.ReadOnlyViews.MappingViewTests.Model.Create(1), Collections.ReadOnlyViews.MappingViewTests.Model.Create(2) };
                using var view = source.AsMappingView(
                    Collections.ReadOnlyViews.MappingViewTests.Vm.Create,
                    (x, i) => Collections.ReadOnlyViews.MappingViewTests.Vm.Create(x.Model, i));
                source.RemoveAt(removeAt);

                Assert.AreEqual(1, view.Count);
                Assert.AreSame(source[0], view[0].Model);
                Assert.AreEqual(0, view[0].Index);
            }
        }
    }
}
