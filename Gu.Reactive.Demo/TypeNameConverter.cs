namespace Gu.Reactive.Demo
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public sealed class TypeNameConverter : IValueConverter
    {
        public static readonly TypeNameConverter Default = new TypeNameConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.GetType().Name ?? "null";
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
