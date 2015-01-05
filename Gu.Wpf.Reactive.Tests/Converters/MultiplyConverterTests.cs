namespace Gu.Wpf.Reactive.Tests.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using NUnit.Framework;

    public class MultiplyConverterTests
    {
        [Test]
        public void Roundtrip()
        {
            var converter = (IValueConverter)new MultiplyConverter { Factor = 2 };
            var convert = converter.Convert(1.2, null, null, null);
            Assert.AreEqual(2.4, convert);
            var convertBack = converter.ConvertBack(convert, null, null, null);
            Assert.AreEqual(1.2, convertBack);
        }

        [TestCase("2.4", "en")]
        [TestCase("2,4", "sv")]
        public void ConvertBack(object o, string culture)
        {
            var converter = (IValueConverter)new MultiplyConverter { Factor = 2 };
           var convertBack = converter.ConvertBack(o, null, null, new CultureInfo(culture));
            Assert.AreEqual(1.2, convertBack);
        }

        [Test]
        public void ThrowsInDesigntime()
        {
            var converter = (IValueConverter)new MultiplyConverter { Factor = 2 };
            DesignMode.OverrideIsDesignTime = true;
            Assert.Throws<ArgumentException>(() => converter.Convert(1.2, null, new object(), null));
            Assert.Throws<ArgumentException>(() => converter.ConvertBack(1.2, null, new object(), null));

            DesignMode.OverrideIsDesignTime = false;
            Assert.DoesNotThrow(() => converter.Convert(1.2, null, new object(), null));
            Assert.DoesNotThrow(() => converter.ConvertBack(1.2, null, new object(), null));
        }
    }
}
