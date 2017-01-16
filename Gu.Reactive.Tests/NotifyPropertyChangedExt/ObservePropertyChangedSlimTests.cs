// ReSharper disable All
namespace Gu.Reactive.Tests.NotifyPropertyChangedExt
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Tests.Helpers;

    using JetBrains.Annotations;

    using NUnit.Framework;

    public class ObservePropertyChangedSlimTests : INotifyPropertyChanged
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
            var fake = new Fake();
            int count = 0;
            fake.ObservePropertyChangedSlim()
                .Subscribe(_ => count++);
            Assert.AreEqual(0, count);
            fake.IsTrue = !fake.IsTrue;
            Assert.AreEqual(1, count);
        }

        [TestCase(true, 1)]
        [TestCase(false, 0)]
        public void SignalInitial(bool signalInitial, int expected)
        {
            int count = 0;
            this.ObservePropertyChangedSlim(nameof(this.PublicProperty), signalInitial)
                .Subscribe(_ => count++);
            Assert.AreEqual(expected, count);
            this.PublicProperty++;
            Assert.AreEqual(expected + 1, count);
        }

        [Test]
        public void WhenPublicProperty()
        {
            int count = 0;
            this.ObservePropertyChangedSlim(nameof(this.PublicProperty))
                .Subscribe(_ => count++);
            Assert.AreEqual(1, count);
            this.PublicProperty++;
            Assert.AreEqual(2, count);
        }

        [Test]
        public void WhenPrivateProperty()
        {
            int count = 0;
            this.ObservePropertyChangedSlim(nameof(this.PrivateProperty))
                .Subscribe(_ => count++);
            Assert.AreEqual(1, count);
            this.PrivateProperty++;
            Assert.AreEqual(2, count);
        }

        [Test]
        public void MissingProperty()
        {
            var fake = new Fake();
            Assert.Throws<ArgumentException>(() => fake.ObservePropertyChangedSlim("Missing"));
        }

        [Test]
        public void NamedNoSignalInitial()
        {
            var fake = new Fake();
            int count = 0;
            fake.ObservePropertyChangedSlim(nameof(fake.IsTrue), false)
                .Subscribe(_ => count++);
            Assert.AreEqual(0, count);
            fake.IsTrue = !fake.IsTrue;
            Assert.AreEqual(1, count);

            fake.Value++;
            Assert.AreEqual(1, count);
        }

        [Test]
        public void NamedSignalInitial()
        {
            var fake = new Fake();
            int count = 0;
            fake.ObservePropertyChangedSlim(nameof(fake.IsTrue), true)
                .Subscribe(_ => count++);
            Assert.AreEqual(1, count);
            fake.IsTrue = !fake.IsTrue;
            Assert.AreEqual(2, count);

            fake.Value++;
            Assert.AreEqual(2, count);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
