namespace Gu.Reactive.Tests.Internals
{
    using System;

    using Gu.Reactive.Internals;

    using NUnit.Framework;

    public partial class EnsureTests
    {
        [TestCase(null, true)]
        [TestCase("", true)]
        [TestCase("Yeah", false)]
        public void NotNullOrEmpty(string value, bool expectException)
        {
            if (expectException)
            {
                var ex = Assert.Throws<ArgumentNullException>(() => Ensure.NotNullOrEmpty(value, nameof(value)));
                Assert.AreEqual("Value cannot be null.\r\nParameter name: value", ex.Message);
            }
            else
            {
                Ensure.NotNullOrEmpty(value, nameof(value));
            }
        }
    }
}
