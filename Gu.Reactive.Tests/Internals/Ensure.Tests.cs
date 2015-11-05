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
                Assert.AreEqual(message + "\r\nParameter name: isTrue", ex.Message);
            }
        }

        [TestCase(1, 2, "Expected value to be: 2, was: 1\r\nParameter name: value")]
        [TestCase(1, 1, null)]
        public void Equal(object value, object expected, string message)
        {
            if (message == null)
            {
                Ensure.Equal(value, expected, nameof(value));
            }
            else
            {
                var ex = Assert.Throws<ArgumentException>(() => Ensure.Equal(value, expected, nameof(value)));
                Assert.AreEqual(message, ex.Message);
            }
        }

        [TestCase(1, 1, "Expected value to not be: 1\r\nParameter name: value")]
        [TestCase(1, 2, null)]
        public void NotEqual(object value, object expected, string message)
        {
            if (message == null)
            {
                Ensure.NotEqual(value, expected, nameof(value));
            }
            else
            {
                var ex = Assert.Throws<ArgumentException>(() => Ensure.NotEqual(value, expected, nameof(value)));
                Assert.AreEqual(message, ex.Message);
            }
        }
    }
}
