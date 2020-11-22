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
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public EnumValuesForExtension(Type type)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            this.Type = type;
        }

        /// <summary>
        /// Gets or sets the enum type.
        /// </summary>
        [ConstructorArgument("type")]
        public Type Type
        {
            get => this.type;

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (!value.IsEnum)
                {
                    throw new ArgumentException("Expected type to be an enum");
                }

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
