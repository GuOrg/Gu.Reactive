// ReSharper disable UnusedParameter.Global
// ReSharper disable EmptyConstructor
// ReSharper disable VirtualMemberNeverOverridden.Global
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements must be documented
namespace Gu.Wpf.Reactive
{
    using System;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Windows.Data;
    using System.Windows.Markup;

    using Gu.Reactive;

    /// <summary>
    /// Class implements a base for a typed value converter used as a markup extension. Override the Convert method in the inheriting class
    /// </summary>
    /// <typeparam name="TInput">Type of the expected input - value to be converted</typeparam>
    /// <typeparam name="TResult">Type of the result of the conversion</typeparam>
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    [Obsolete("To be removed.")]
    public abstract class MarkupConverter<TInput, TResult> : MarkupExtension, IValueConverter
    {
        private static readonly ITypeConverter<TInput> InputTypeConverter = TypeConverterFactory.Create<TInput>();
        private static readonly ITypeConverter<TResult> ResultTypeConverter = TypeConverterFactory.Create<TResult>();

        protected MarkupConverter()
        {
        }

        /// <inheritdoc/>
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            this.VerifyValue(value, parameter);
            if (InputTypeConverter.IsValid(value))
            {
                var convertTo = InputTypeConverter.ConvertTo(value, culture);
                return this.Convert(convertTo, culture);
            }

            return this.ConvertDefault();
        }

        /// <inheritdoc/>
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            this.VerifyValue(value, parameter);
            if (ResultTypeConverter.CanConvertTo(value, culture))
            {
                var convertTo = ResultTypeConverter.ConvertTo(value, culture);
                return this.ConvertBack(convertTo, culture);
            }

            return this.ConvertBackDefault();
        }

        /// <inheritdoc/>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The converted value.</returns>
        protected abstract TResult Convert(TInput value, CultureInfo culture);

        protected virtual TResult ConvertDefault()
        {
            return default(TResult);
        }

        /// <summary>
        /// Convert <paramref name="value"/>
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The converted value.</returns>
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
                    var message = $"ConverterParameter makes no sense for MarkupConverter. Parameter was: {parameter} for converter of type {this.GetType().Name}";
                    throw new ArgumentException(message);
                }

                if (!InputTypeConverter.IsValid(value))
                {
                    var message = $"{caller} value: {value}{(value != null ? "of type: " + value.GetType().Name : string.Empty)} is not valid for converter of type: {this.GetType().Name} from: {typeof(TInput).PrettyName()} to {typeof(TResult).PrettyName()}";
                    throw new ArgumentException(message, nameof(value));
                }
            }
        }
    }
}
