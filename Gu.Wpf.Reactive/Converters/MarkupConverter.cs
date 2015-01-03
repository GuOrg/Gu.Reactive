namespace Gu.Wpf.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;
    using System.Windows.Media;

    /// <summary>
    /// Class implements a base for a typed value converter used as a markup extension. Override the Convert method in the inheriting class
    /// </summary>
    /// <typeparam name="TInput">Type of the expected input - value to be converted</typeparam>
    /// <typeparam name="TResult">Type of the result of the conversion</typeparam>
    public abstract class MarkupConverter<TInput, TResult> : MarkupExtension, IValueConverter
    {
        private readonly ITypeConverter<TInput> _inputTypeConverter;
        private readonly ITypeConverter<TResult> _resultTypeConverter;
        private ITypeDescriptorContext _typeDescriptorContext;
        protected MarkupConverter()
        {
            _inputTypeConverter = new DefaultTypeConverter<TInput>();
            _resultTypeConverter = new DefaultTypeConverter<TResult>();
        }

        protected MarkupConverter(ITypeConverter<TInput> inputTypeConverter, ITypeConverter<TResult> resultTypeConverter)
        {
            _inputTypeConverter = inputTypeConverter;
            _resultTypeConverter = resultTypeConverter;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (DesignMode.IsDesignTime)
            {
                if (parameter != null)
                {
                    throw new ArgumentException("parameter makes no sense for markupextension");
                }
                if (!_inputTypeConverter.IsValid(value))
                {
                    throw new ArgumentException("{0}.Convert() value is not valid", "value");
                }
            }
            if (_inputTypeConverter.IsValid(value))
            {
                var convertTo = _inputTypeConverter.ConvertTo(value, culture);
                return Convert(convertTo, culture);
            }
            return ConvertDefault();
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (DesignMode.IsDesignTime)
            {
                if (parameter != null)
                {
                    throw new ArgumentException("parameter makes no sense for markupextension");
                }
                if (!_resultTypeConverter.IsValid(value))
                {
                    throw new ArgumentException("{0}.ConvertBack() value is not valid", "value");
                }
            }
            if (_resultTypeConverter.CanConvertTo(value, culture))
            {
                var convertTo = _resultTypeConverter.ConvertTo(value, culture);
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
    }
}
