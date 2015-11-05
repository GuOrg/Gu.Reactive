namespace Gu.Wpf.Reactive
{
    using System;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Windows.Data;
    using System.Windows.Markup;

    using Gu.Reactive;
    using Gu.Wpf.Reactive.TypeConverters;

    /// <summary>
    /// Class implements a base for a typed value converter used as a markup extension. Override the Convert method in the inheriting class
    /// </summary>
    /// <typeparam name="TInput">Type of the expected input - value to be converted</typeparam>
    /// <typeparam name="TResult">Type of the result of the conversion</typeparam>
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public abstract class MarkupConverter<TInput, TResult> : MarkupExtension, IValueConverter
    {
        private static readonly ITypeConverter<TInput> InputTypeConverter = TypeConverterFactory.Create<TInput>();
        private static readonly ITypeConverter<TResult> ResultTypeConverter = TypeConverterFactory.Create<TResult>();

        protected MarkupConverter()
        {
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            VerifyValue(value, parameter);
            if (InputTypeConverter.IsValid(value))
            {
                var convertTo = InputTypeConverter.ConvertTo(value, culture);
                return Convert(convertTo, culture);
            }
            return ConvertDefault();
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            VerifyValue(value, parameter);
            if (ResultTypeConverter.CanConvertTo(value, culture))
            {
                var convertTo = ResultTypeConverter.ConvertTo(value, culture);
                return ConvertBack(convertTo, culture);
            }
            return ConvertBackDefault();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        protected abstract TResult Convert(TInput value, CultureInfo culture);

        protected virtual TResult ConvertDefault()
        {
            return default(TResult);
        }

        protected abstract TInput ConvertBack(TResult value, CultureInfo culture);

        protected virtual TInput ConvertBackDefault()
        {
            return default(TInput);
        }

        private void VerifyValue(object value, object parameter, [CallerMemberName] string caller = null)
        {
            if (DesignMode.IsDesignTime)
            {
                if (parameter != null)
                {
                    throw new ArgumentException(string.Format("ConverterParameter makes no sense for MarkupConverter. Parameter was: {0} for converter of type {1}", parameter, GetType().Name));
                }
                if (!InputTypeConverter.IsValid(value))
                {
                    var message = string.Format(
                            "{0} value: {1}{2} is not valid for converter of type: {3} from: {4} to {5}",
                            caller,
                            value,
                            value != null ? "of type: " + value.GetType().Name : "",
                            GetType().Name,
                            typeof(TInput).PrettyName(),
                            typeof(TResult).PrettyName());
                    throw new ArgumentException(message, nameof(value));
                }
            }
        }
    }
}
