namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections;
    using System.Windows.Markup;

    using Gu.Reactive.Internals;

    [MarkupExtensionReturnType(typeof(IEnumerable))]
    public class EnumValuesForExtension : MarkupExtension
    {
        private Type type;

        public EnumValuesForExtension(Type type)
        {
            this.Type = type;
        }

        [ConstructorArgument("type")]
        public Type Type
        {
            get { return this.type; }

            set
            {
                Ensure.NotNull(value, nameof(value));
                Ensure.IsTrue(value.IsEnum, nameof(value),$"Expected {this.Type} to be an enum.");
                this.type = value;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Ensure.NotNull(this.Type, nameof(this.Type));
            return Enum.GetValues(this.Type);
        }
    }
}