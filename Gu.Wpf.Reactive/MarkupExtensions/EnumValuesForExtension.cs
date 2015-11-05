namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections;
    using System.Windows.Markup;

    using Gu.Reactive.Internals;

    [MarkupExtensionReturnType(typeof(IEnumerable))]
    public class EnumValuesForExtension : MarkupExtension
    {
        private Type _type;

        public EnumValuesForExtension(Type type)
        {
            Type = type;
        }

        [ConstructorArgument("type")]
        public Type Type
        {
            get { return _type; }
            set
            {
                Ensure.NotNull(value, nameof(value));
                Ensure.IsTrue(value.IsEnum, nameof(value),$"Expected {Type} to be an enum.");
                _type = value;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Ensure.NotNull(Type, nameof(Type));
            return Enum.GetValues(Type);
        }
    }
}