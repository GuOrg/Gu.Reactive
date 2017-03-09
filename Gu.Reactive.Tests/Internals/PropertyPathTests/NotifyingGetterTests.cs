namespace Gu.Reactive.Tests.Internals.PropertyPathTests
{
    using System;

    using Gu.Reactive.Internals;
    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class NotifyingGetterTests
    {
        [Test]
        public void FakeIsTrue()
        {
            var pathProperty = Getter.GetOrCreate(typeof(Fake).GetProperty(nameof(Fake.IsTrue)));
            Assert.IsInstanceOf<NotifyingGetter<Fake, bool>>(pathProperty);
        }

        [Test]
        public void FakeOfIntNext()
        {
            var pathProperty = Getter.GetOrCreate(typeof(Fake<int>).GetProperty(nameof(Fake<int>.Next)));
            Assert.IsInstanceOf<NotifyingGetter<Fake<int>, Level<int>>>(pathProperty);
        }

        [Test]
        public void ThrowsOnWriteOnly()
        {
            var propertyInfo = typeof(Fake).GetProperty("WriteOnly");
            Assert.NotNull(propertyInfo);
            //// ReSharper disable once ObjectCreationAsStatement
            var exception = Assert.Throws<ArgumentException>(() => Getter.GetOrCreate(propertyInfo));
            var expected = "Property cannot be write only.\r\n" +
                           "The property Gu.Reactive.Tests.Helpers.Fake.WriteOnly does not have a getter.\r\n" +
                           "Parameter name: property";
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void ThrowsOnNullProp()
        {
            // ReSharper disable once ObjectCreationAsStatement
            var exception = Assert.Throws<ArgumentNullException>(() => Getter.GetOrCreate(null));
            Assert.AreEqual("Value cannot be null.\r\nParameter name: property", exception.Message);
        }
    }
}
