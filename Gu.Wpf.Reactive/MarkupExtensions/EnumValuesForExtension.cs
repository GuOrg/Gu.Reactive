namespace Gu.Wpf.Reactive
{
    using System;
    using System.Windows.Markup;

    using Gu.Reactive.Internals;

    /// <summary>
    /// Markup extension for getting Enum.GetValues(this.Type).
    /// </summary>
    [MarkupExtensionReturnType(typeof(Array))]
    public class EnumValuesForExtension : MarkupExtension
    {
        private Type type;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumValuesForExtension"/> class.
        /// </summary>
        /// <param name="type">The enum type.</param>
        public EnumValuesForExtension(Type type)
        {
            Ensure.IsTrue(type?.IsEnum == true, nameof(type), "Expected type to be an enum");
            this.type = type;
        }

        /// <summary>
        /// The enum type.
        /// </summary>
        [ConstructorArgument("type")]
        public Type Type
        {
            get => this.type;

            set
            {
                Ensure.NotNull(value, nameof(value));
                Ensure.IsTrue(value.IsEnum, nameof(value), $"Expected {this.Type} to be an enum.");
                this.type = value;
            }
        }

        /// <inheritdoc/>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Ensure.NotNull(this.Type, nameof(this.Type));
            return Enum.GetValues(this.Type);
        }
    }
}
