namespace Gu.Reactive.Tests.Internals.PropertyPathTests
{
    using System;

    using Gu.Reactive.Internals;
    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class PathItemTests
    {
        [Test]
        public void ThrowsOnWriteOnly()
        {
            var propertyInfo = typeof(Fake).GetProperty("WriteOnly");
            Assert.NotNull(propertyInfo);
            //// ReSharper disable once ObjectCreationAsStatement
            var exception = Assert.Throws<ArgumentException>(() => new PathProperty(null, propertyInfo));
            var expected = "Property cannot be write only.\r\n" +
                           "The property Gu.Reactive.Tests.Helpers.Fake.WriteOnly does not have a getter.\r\n" +
                           "Parameter name: propertyInfo";
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void ThrowsOnNullProp()
        {
            // ReSharper disable once ObjectCreationAsStatement
            var exception = Assert.Throws<ArgumentNullException>(() => new PathProperty(null, null));
            Assert.AreEqual("Value cannot be null.\r\nParameter name: propertyInfo", exception.Message);
        }
    }
}
