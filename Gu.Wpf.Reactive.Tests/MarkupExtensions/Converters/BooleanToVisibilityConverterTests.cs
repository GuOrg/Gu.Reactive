namespace Gu.Wpf.Reactive.Tests.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using NUnit.Framework;

    public class BooleanToVisibilityConverterTests
    {
        [TestCase(true, Visibility.Visible)]
        [TestCase(false, Visibility.Hidden)]
        [TestCase(null, Visibility.Collapsed)]
        public void Roundtrip(bool? visible, Visibility visibility)
        {
            var converter = (IValueConverter)new BooleanToVisibilityConverter
            {
                WhenTrue = Visibility.Visible,
                WhenFalse = Visibility.Hidden,
                WhenNull = Visibility.Collapsed
            };
            var convert = converter.Convert(visible, null, null, null);
            Assert.AreEqual(visibility, convert);
            var convertBack = converter.ConvertBack(convert, null, null, null);
            Assert.AreEqual(visible, convertBack);
            convertBack = converter.ConvertBack(convert.ToString(), null, null, null);
            Assert.AreEqual(visible, convertBack);
        }

        [Test]
        public void ThrowsInDesigntime()
        {
            var converter = (IValueConverter)new BooleanToVisibilityConverter();
            DesignMode.OverrideIsDesignTime = true;
            Assert.Throws<ArgumentException>(() => converter.Convert(1.2, null, new object(), null));
            Assert.Throws<ArgumentException>(() => converter.ConvertBack(1.2, null, new object(), null));

            DesignMode.OverrideIsDesignTime = false;
            Assert.DoesNotThrow(() => converter.Convert(1.2, null, new object(), null));
            Assert.DoesNotThrow(() => converter.ConvertBack(1.2, null, new object(), null));
        }

        [TestCase(true)]
        [TestCase(false)]
        [TestCase(null)]
        public void DefaultValues(bool? visible)
        {
            var converter = (IValueConverter)new BooleanToVisibilityConverter();
            var systemConverter = new System.Windows.Controls.BooleanToVisibilityConverter();
            var convert = converter.Convert(visible, null, null, null);
            var sysConvert = systemConverter.Convert(visible, null, null, null);
            Assert.AreEqual(convert, sysConvert);
            var convertBack = converter.ConvertBack(convert, null, null, null);
            var sysConvertBack = systemConverter.ConvertBack(sysConvert, null, null, null);
            Assert.AreEqual(convertBack, sysConvertBack);
        }

        [TestCase(Visibility.Collapsed, false)]
        [TestCase("Collapsed", false)]
        [TestCase(Visibility.Visible, true)]
        [TestCase("Visible", true)]
        public void ConvertBack(object o, bool expected)
        {
            var converter = (IValueConverter)new BooleanToVisibilityConverter();
            var convertBack = converter.ConvertBack(o, null, null, null);
            Assert.AreEqual(expected, convertBack);
        }
    }
}