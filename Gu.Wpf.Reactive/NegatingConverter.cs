namespace Gu.Wpf.Reactive
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Negates bools
    /// </summary>
    public class NegatingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool) value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}