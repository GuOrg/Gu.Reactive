// ReSharper disable All
namespace Gu.Reactive.Tests.NotifyPropertyChangedExt
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Tests.Helpers;

    using Moq;
    using NUnit.Framework;

    public partial class NotifyPropertyChangedExtTests
    {
        public class ObservePropertyChangedSlimString : INotifyPropertyChanged
        {
            private int publicProperty;

            private int privateProperty;

            public event PropertyChangedEventHandler PropertyChanged;

            public int PublicProperty
            {
                get
                {
                    return this.publicProperty;
                }

                set
                {
                    if (value == this.publicProperty)
                    {
                        return;
                    }

                    this.publicProperty = value;
                    this.OnPropertyChanged();
                }
            }

            private int PrivateProperty
            {
                get
                {
                    return this.privateProperty;
                }

                set
                {
                    if (value == this.privateProperty)
                    {
                        return;
                    }

                    this.privateProperty = value;
                    this.OnPropertyChanged();
                }
            }

            [Test]
            public void Simple()
            {
                var source = new Fake();
                var changes = new List<PropertyChangedEventArgs>();
                using (source.ObservePropertyChangedSlim()
                             .Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);
                    source.IsTrue = !source.IsTrue;
                    CollectionAssert.AreEqual(new[] { "IsTrue" }, changes.Select(x => x.PropertyName));

                    source.Value++;
                    CollectionAssert.AreEqual(new[] { "IsTrue", "Value" }, changes.Select(x => x.PropertyName));
                }
            }

            [Test]
            public void ShadowingProperty()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var source = new WithShadowing<int>();
                using (source.ObservePropertyChangedSlim("Value", signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    source.Value = 1;
                    CollectionAssert.AreEqual(new[] { "Value" }, changes.Select(x => x.PropertyName));
                }
            }

            [TestCase(true, 1)]
            [TestCase(false, 0)]
            public void SignalInitial(bool signalInitial, int expected)
            {
                var count1 = 0;
                var count2 = 0;
                var observable = this.ObservePropertyChangedSlim(nameof(this.PublicProperty), signalInitial);
                using (observable.Subscribe(_ => count1++))
                {
                    using (observable.Subscribe(_ => count2++))
                    {
                        Assert.AreEqual(expected, count1);
                        Assert.AreEqual(expected, count2);

                        this.PublicProperty++;
                        Assert.AreEqual(expected + 1, count1);
                        Assert.AreEqual(expected + 1, count2);
                    }
                }
            }

            [Test]
            public void WhenPublicProperty()
            {
                var count = 0;
                using (this.ObservePropertyChangedSlim(nameof(this.PublicProperty))
                           .Subscribe(_ => count++))
                {
                    Assert.AreEqual(1, count);
                    this.PublicProperty++;
                    Assert.AreEqual(2, count);
                }
            }

            [Test]
            public void WhenPrivateProperty()
            {
                var count = 0;
                using (this.ObservePropertyChangedSlim(nameof(this.PrivateProperty))
                           .Subscribe(_ => count++))
                {
                    Assert.AreEqual(1, count);
                    this.PrivateProperty++;
                    Assert.AreEqual(2, count);
                }
            }

            [Test]
            public void MissingProperty()
            {
                var source = new Fake();
                _ = Assert.Throws<ArgumentException>(() => source.ObservePropertyChangedSlim("Missing"));
            }

            [Test]
            public void NamedNoSignalInitial()
            {
                var source = new Fake();
                var count = 0;
                using (source.ObservePropertyChangedSlim(nameof(source.IsTrue), signalInitial: false)
                             .Subscribe(_ => count++))
                {
                    Assert.AreEqual(0, count);
                    source.IsTrue = !source.IsTrue;
                    Assert.AreEqual(1, count);

                    source.Value++;
                    Assert.AreEqual(1, count);
                }
            }

            [Test]
            public void NamedSignalInitial()
            {
                var source = new Fake();
                var count = 0;
                using (source.ObservePropertyChangedSlim(nameof(source.IsTrue), signalInitial: true)
                           .Subscribe(_ => count++))
                {
                    Assert.AreEqual(1, count);
                    source.IsTrue = !source.IsTrue;
                    Assert.AreEqual(2, count);

                    source.Value++;
                    Assert.AreEqual(2, count);
                }
            }

            [Test]
            public void ReadOnlyObservableCollectionCount()
            {
                var ints = new ObservableCollection<int>();
                var source = new ReadOnlyObservableCollection<int>(ints);
                var values = new List<string>();
                using (source.ObservePropertyChangedSlim("Count", signalInitial: false)
                             .Subscribe(x => values.Add(x.PropertyName)))
                {
                    CollectionAssert.IsEmpty(values);

                    ints.Add(1);
                    CollectionAssert.AreEqual(new[] { "Count" }, values);

                    ints.Add(2);
                    CollectionAssert.AreEqual(new[] { "Count", "Count" }, values);
                }
            }

            [Test]
            public void ReactsOnMock()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var source = new Mock<IReadOnlyObservableCollection<int>>();
                using (source.Object.ObservePropertyChangedSlim("Count", signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);

                    source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs("Count"));
                    CollectionAssert.AreEqual(new[] { "Count" }, changes.Select(x => x.PropertyName));
                }
            }

            [TestCase("")]
            [TestCase(null)]
            [TestCase("Name")]
            public void ReactsOnStringEmptyOrNullWhenHasValue(string propertyName)
            {
                var changes = new List<PropertyChangedEventArgs>();
                var source = new Fake { Name = "Johan" };
                using (source.ObservePropertyChangedSlim("Name", signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);
                    source.OnPropertyChanged(propertyName);
                    //// This means all properties changed according to wpf convention
                    CollectionAssert.AreEqual(new[] { propertyName }, changes.Select(x => x.PropertyName));
                }
            }

            [TestCase("")]
            [TestCase(null)]
            [TestCase("Name")]
            public void ReactsOnStringEmptyOrNullWhenNull(string propertyName)
            {
                var changes = new List<PropertyChangedEventArgs>();
                var source = new Fake { Name = null };
                using (source.ObservePropertyChangedSlim("Name", signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);
                    source.OnPropertyChanged(propertyName);
                    //// This means all properties changed according to wpf convention
                    CollectionAssert.AreEqual(new[] { propertyName }, changes.Select(x => x.PropertyName));
                }
            }

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
