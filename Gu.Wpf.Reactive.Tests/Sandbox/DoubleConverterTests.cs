namespace Gu.Wpf.Reactive.Tests.Sandbox
{
    using System.ComponentModel;
    using NUnit.Framework;

    public class DoubleConverterTests
    {
        [Test]
        public void TestNameTest()
        {
            var doubleConverter = new DoubleConverter();
            Assert.IsTrue(doubleConverter.IsValid(1));
            Assert.IsTrue(doubleConverter.IsValid("1"));
            //var convertTo = doubleConverter.ConvertTo(null, CultureInfo.InvariantCulture, 1, typeof (double));
        }
    }
}
