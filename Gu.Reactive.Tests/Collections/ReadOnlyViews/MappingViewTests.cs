// ReSharper disable All
#pragma warning disable WPF1011 // Implement INotifyPropertyChanged.
namespace Gu.Reactive.Tests.Collections.ReadOnlyViews
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Gu.Reactive.Internals;
    using NUnit.Framework;

    public static partial class MappingViewTests
    {
        [Test]
        public static void DoesNotDisposeInner()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using var mapped1 = source.AsMappingView(x => x, leaveOpen: true);
            using (var mapped2 = mapped1.AsMappingView(x => x, leaveOpen: true))
            {
                CollectionAssert.AreEqual(mapped1, source);
                CollectionAssert.AreEqual(mapped2, source);
            }

            CollectionAssert.AreEqual(mapped1, source);
        }

        [Test]
        public static void DoesNotDisposeInnerDisposeTwice()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using var mapped1 = source.AsMappingView(x => x, leaveOpen: true);
            using (var mapped2 = mapped1.AsMappingView(x => x, leaveOpen: true))
            {
                CollectionAssert.AreEqual(mapped1, source);
                CollectionAssert.AreEqual(mapped2, source);
#pragma warning disable IDISP016 // Don't use disposed instance.
                mapped2.Dispose();
#pragma warning restore IDISP016 // Don't use disposed instance.
                mapped2.Dispose();
            }

            CollectionAssert.AreEqual(mapped1, source);
        }

        [Test]
        public static void DisposesInnerByDefault()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using var mapped1 = source.AsMappingView(x => x, leaveOpen: true);
            using (var mapped2 = mapped1.AsMappingView(x => x))
            {
                CollectionAssert.AreEqual(mapped1, source);
                CollectionAssert.AreEqual(mapped2, source);
            }

            _ = Assert.Throws<ObjectDisposedException>(() => _ = mapped1.Count);
        }

        [Test]
        public static void DisposesInnerExplicit()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using var mapped1 = source.AsMappingView(x => x, leaveOpen: true);
            using (var mapped2 = mapped1.AsMappingView(x => x, leaveOpen: false))
            {
                CollectionAssert.AreEqual(mapped1, source);
                CollectionAssert.AreEqual(mapped2, source);
            }

            _ = Assert.Throws<ObjectDisposedException>(() => _ = mapped1.Count);
        }

        public static class Model
        {
            public static Model<T> Create<T>(T value) => new Model<T>(value);
        }

        public static class Vm
        {
            public static Vm<T> Create<T>(Model<T> model) => new Vm<T>(model, 0);

            public static Vm<T> Create<T>(Model<T>? model, int index) => new Vm<T>(model, index);
        }

        public class Model<T>
        {
            public Model(T value)
            {
                this.Value = value;
            }

            public T Value { get; }

            public override string ToString()
            {
                return $"{nameof(this.Value)}: {this.Value}";
            }
        }

        public class Vm<T> : System.ComponentModel.INotifyPropertyChanged
        {
            private Model<T>? model;
            private int index;

            public Vm()
            {
            }

            public Vm(Model<T>? model, int index)
            {
                this.model = model;
                this.index = index;
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            public Model<T>? Model
            {
                get => this.model;
                set
                {
                    if (ReferenceEquals(value, this.model))
                    {
                        return;
                    }

                    this.model = value;
                    this.OnPropertyChanged();
                }
            }

            public int Index
            {
                get => this.index;
                set
                {
                    if (value == this.index)
                    {
                        return;
                    }

                    this.index = value;
                    this.OnPropertyChanged();
                }
            }

            public Vm<T> WithIndex(int i)
            {
                this.Index = i;
                return this;
            }

            public override string ToString()
            {
                return $"{nameof(this.Index)}: {this.Index}, {nameof(this.Model)}: {this.Model}";
            }

            protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
