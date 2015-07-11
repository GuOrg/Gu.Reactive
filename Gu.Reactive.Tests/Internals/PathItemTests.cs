namespace Gu.Reactive.Tests.Internals
{
    using System;

    using Gu.Reactive.Internals;
    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;
    using PropertyPathStuff;

    public class PathItemTests
    {

        [Test]
        public void ThrowsOnWriteOnly()
        {
            var propertyInfo = typeof(Fake).GetProperty("WriteOnly");
            Assert.NotNull(propertyInfo);
            Assert.Throws<ArgumentException>(() =>  new PathProperty(null, propertyInfo));
        }

        [Test]
        public void ThrowsOnNullProp()
        {
            Assert.Throws<ArgumentNullException>(() => new PathProperty(null, null));
        }
    }
}
