namespace Gu.Wpf.Reactive
{
    using System;
    using System.Windows.Data;
    using System.Windows.Markup;

    /// <summary>
    /// Class implements a base for a typed value converter used as a markup extension. Override the Convert method in the inheriting class
    /// </summary>
    /// <typeparam name="TInput">Type of the expected input - value to be converted</typeparam>
    /// <typeparam name="TResult">Type of the result of the conversion</typeparam>
    public class MarkupConverter<TInput, TResult> : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value is TInput ? Convert((TInput)value) : ConvertDefault();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value is TResult ? ConvertBack((TResult)value) : ConvertBackDefault();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        protected virtual TResult Convert(TInput value)
        {
            return ConvertDefault();
        }

        protected virtual TResult ConvertDefault()
        {
            return default(TResult);
        }

        protected virtual TInput ConvertBack(TResult value)
        {
            return ConvertBackDefault();
        }

        protected virtual TInput ConvertBackDefault()
        {
            return default(TInput);
        }
    }
}
