namespace Gu.Reactive.Tests.Internals
{
    using System;

    using Gu.Reactive.Internals;

    using NUnit.Framework;

    public partial class EnsureTests
    {
        [TestCase(null, true)]
        [TestCase("", false)]
        [TestCase(1, false)]
        public void NotNull(object value, bool expectException)
        {
            if (expectException)
            {
                var ex = Assert.Throws<ArgumentNullException>(() => Ensure.NotNull(value, nameof(value)));
                Assert.AreEqual("Value cannot be null.\r\nParameter name: value", ex.Message);
            }
            else
            {
                Ensure.NotNull(value, nameof(value));
            }
        }

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

        [TestCase(false, "Message")]
        [TestCase(true, null)]
        public void IsTrue(bool isTrue, string message)
        {
            if (isTrue)
            {
                Ensure.IsTrue(isTrue, nameof(isTrue), message);
            }
            else
            {
                var ex = Assert.Throws<ArgumentException>(() => Ensure.IsTrue(isTrue, nameof(isTrue), message));
                Assert.AreEqual(message+ "\r\nParameter name: isTrue", ex.Message);
            }
        }
    }
}
