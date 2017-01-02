namespace Gu.Wpf.Reactive.Tests.Converters.TypeConverters
{
    using NUnit.Framework;

    public class DefaultTypeConverterTests
    {
        [TestCase(null, true)]
        [TestCase(true, true)]
        [TestCase(2, false)]
        [TestCase("", false)]
        public void IsValidNullableBool(object value, bool expected)
        {
            var converter = TypeConverterFactory.Create<bool?>();
            var actual = converter.IsValid(value);
            Assert.AreEqual(expected, actual);
        }

        [TestCase(null, true)]
        [TestCase(1.0, true)]
        [TestCase(2, true)]
        [TestCase("", false)]
        public void IsValidNullableDouble(object value, bool expected)
        {
            var converter = TypeConverterFactory.Create<double?>();
            var actual = converter.IsValid(value);
            Assert.AreEqual(expected, actual);
        }
    }
}
