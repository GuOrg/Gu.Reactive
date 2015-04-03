namespace Gu.Reactive.Tests.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Gu.Reactive.Internals;
    using Gu.Reactive.Tests.Fakes;

    using NUnit.Framework;

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
