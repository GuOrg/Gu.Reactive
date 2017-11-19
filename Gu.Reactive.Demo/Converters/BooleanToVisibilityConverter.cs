namespace Gu.Reactive.Demo.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    [MarkupExtensionReturnType(typeof(BooleanToVisibilityConverter))]
    [ValueConversion(typeof(bool?), typeof(Visibility))]
    public class BooleanToVisibilityConverter : MarkupExtension, IValueConverter
    {
        public Visibility WhenTrue { get; set; } = Visibility.Visible;

        public Visibility WhenFalse { get; set; } = Visibility.Hidden;

        public Visibility WhenNull { get; set; } = Visibility.Hidden;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var meh = value as bool?;
            if (meh == null)
            {
                return this.WhenNull;
            }

            return meh == true
                       ? this.WhenTrue
                       : this.WhenFalse;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
