namespace Gu.Wpf.Reactive.Tests.Converters.TypeConverters
{
    using System;
    using NUnit.Framework;
    using Reactive.TypeConverters;

    public class DefaultTypeConverterTests
    {
        [TestCase(null, true)]
        [TestCase(true, true)]
        [TestCase(2, false)]
        [TestCase("", false)]
        public void IsValidNullable_Bool(object value, bool expected)
        {
            var converter = TypeConverterFactory.Create<bool?>();
            var actual = converter.IsValid(value);
            Assert.AreEqual(expected, actual);
        }

        [TestCase(null, true)]
        [TestCase(1.0, true)]
        [TestCase(2, true)]
        [TestCase("", false)]
        public void IsValidNullable_Double(object value, bool expected)
        {
            var converter = TypeConverterFactory.Create<double?>();
            var actual = converter.IsValid(value);
            Assert.AreEqual(expected, actual);
        }
    }
}
