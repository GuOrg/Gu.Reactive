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
                Ensure.IsTrue(condition: true, parameterName: nameof(isTrue), message: message);
            }
            else
            {
                var ex = Assert.Throws<ArgumentException>(() => Ensure.IsTrue(condition: false, parameterName: nameof(isTrue), message: message));
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

        [TestCase(0, 1, null)]
        [TestCase(0, 0, "Expected x to be less than 0, x was 0\r\nParameter name: x")]
        [TestCase(1, 0, "Expected x to be less than 0, x was 1\r\nParameter name: x")]
        public void LessThan(int x, int max, string message)
        {
            if (message == null)
            {
                Ensure.LessThan(x, max, nameof(x));
            }
            else
            {
                var ex = Assert.Throws<ArgumentException>(() => Ensure.LessThan(x, max, nameof(x)));
                Assert.AreEqual(message, ex.Message);
            }
        }

        [TestCase(0, 1, null)]
        [TestCase(0, 0, null)]
        [TestCase(1, 0, "Expected x to be less than or equal to 0, x was 1\r\nParameter name: x")]
        public void LessThanOrEqualTo(int x, int max, string message)
        {
            if (message == null)
            {
                Ensure.LessThanOrEqual(x, max, nameof(x));
            }
            else
            {
                var ex = Assert.Throws<ArgumentException>(() => Ensure.LessThanOrEqual(x, max, nameof(x)));
                Assert.AreEqual(message, ex.Message);
            }
        }

        [TestCase(1, 0, null)]
        [TestCase(0, 0, "Expected x to be greater than 0, x was 0\r\nParameter name: x")]
        [TestCase(0, 1, "Expected x to be greater than 1, x was 0\r\nParameter name: x")]
        public void GreaterThan(int x, int min, string message)
        {
            if (message == null)
            {
                Ensure.GreaterThan(x, min, nameof(x));
            }
            else
            {
                var ex = Assert.Throws<ArgumentException>(() => Ensure.GreaterThan(x, min, nameof(x)));
                Assert.AreEqual(message, ex.Message);
            }
        }

        [TestCase(1, 0, null)]
        [TestCase(0, 0, null)]
        [TestCase(0, 1, "Expected x to be greater than or equal to 1, x was 0\r\nParameter name: x")]
        public void GreaterThanOrEqualTo(int x, int min, string message)
        {
            if (message == null)
            {
                Ensure.GreaterThanOrEqual(x, min, nameof(x));
            }
            else
            {
                var ex = Assert.Throws<ArgumentException>(() => Ensure.GreaterThanOrEqual(x, min, nameof(x)));
                Assert.AreEqual(message, ex.Message);
            }
        }
    }
}
