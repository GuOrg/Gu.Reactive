// ReSharper disable All
namespace Gu.Reactive.Tests.NotifyPropertyChangedExt
{
    using System;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class ObservePropertyChangedSlimTests
    {
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
    }
}
