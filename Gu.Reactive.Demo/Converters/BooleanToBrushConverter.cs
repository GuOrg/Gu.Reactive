namespace Gu.Reactive.Demo.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Markup;
    using System.Windows.Media;

    [MarkupExtensionReturnType(typeof(BooleanToBrushConverter))]
    [ValueConversion(typeof(bool?), typeof(Brush))]
    public class BooleanToBrushConverter : MarkupExtension, IValueConverter
    {
        public Brush WhenTrue { get; set; }

        public Brush WhenFalse { get; set; }

        public Brush WhenNull { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return b
                    ? this.WhenTrue
                    : this.WhenFalse;
            }

            return this.WhenNull;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}