namespace Gu.Reactive.Tests.Internals
{
    using System;

    using Gu.Reactive.Internals;

    using NUnit.Framework;

    public partial class EnsureTests
    {
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
