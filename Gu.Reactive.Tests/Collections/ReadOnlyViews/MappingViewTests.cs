// ReSharper disable All
namespace Gu.Reactive.Tests.Collections.ReadOnlyViews
{
    using System;
    using System.Collections.ObjectModel;
    using Gu.Reactive.Internals;
    using NUnit.Framework;

    public partial class MappingViewTests
    {
        [Test]
        public void DoesNotDisposeInner()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using (var mapped1 = source.AsMappingView(x => x, leaveOpen: true))
            {
                using (var mapped2 = mapped1.AsMappingView(x => x, leaveOpen: true))
                {
                    CollectionAssert.AreEqual(mapped1, source);
                    CollectionAssert.AreEqual(mapped2, source);
                }

                CollectionAssert.AreEqual(mapped1, source);
            }
        }

        [Test]
        public void DoesNotDisposeInnerDisposeTwice()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using (var mapped1 = source.AsMappingView(x => x, leaveOpen: true))
            {
                using (var mapped2 = mapped1.AsMappingView(x => x, leaveOpen: true))
                {
                    CollectionAssert.AreEqual(mapped1, source);
                    CollectionAssert.AreEqual(mapped2, source);
                    mapped2.Dispose();
                    mapped2.Dispose();
                }

                CollectionAssert.AreEqual(mapped1, source);
            }
        }

        [Test]
        public void DisposesInnerByDefault()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using (var mapped1 = source.AsMappingView(x => x, leaveOpen: true))
            {
                using (var mapped2 = mapped1.AsMappingView(x => x))
                {
                    CollectionAssert.AreEqual(mapped1, source);
                    CollectionAssert.AreEqual(mapped2, source);
                }

                Assert.Throws<ObjectDisposedException>(() => mapped1.Count.IgnoreReturnValue());
            }
        }

        [Test]
        public void DisposesInnerExplicit()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using (var mapped1 = source.AsMappingView(x => x, leaveOpen: true))
            {
                using (var mapped2 = mapped1.AsMappingView(x => x, leaveOpen: false))
                {
                    CollectionAssert.AreEqual(mapped1, source);
                    CollectionAssert.AreEqual(mapped2, source);
                }

                Assert.Throws<ObjectDisposedException>(() => mapped1.Count.IgnoreReturnValue());
            }
        }

        public class Model
        {
            public Model(int value)
            {
                this.Value = value;
            }

            public int Value { get; }

            public override string ToString()
            {
                return $"{nameof(this.Value)}: {this.Value}";
            }
        }

        private class Vm
        {
            public Vm()
            {
            }

            public Vm(Model model,  int index)
            {
                this.Model = model;
                this.Index = index;
            }

            public Model Model { get; set; }

            public int Value { get; set; }

            public int Index { get; set; }

            public Vm WithIndex(int i)
            {
                this.Index = i;
                return this;
            }

            public override string ToString()
            {
                return $"{nameof(this.Value)}: {this.Value}, {nameof(this.Index)}: {this.Index}, {nameof(this.Model)}: {this.Model}";
            }
        }
    }
}
