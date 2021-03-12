namespace Gu.Wpf.Reactive
{
    using System.Windows;
    using System.Windows.Controls;

    using Gu.Reactive;

    /// <summary>
    /// A <see cref="DataTemplateSelector"/> for <see cref="ICondition"/>.
    /// </summary>
    public class ConditionTypeTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the template for <see cref="NegatedCondition"/>.
        /// </summary>
        public DataTemplate? NegatedConditionTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for <see cref="AndCondition"/>.
        /// </summary>
        public DataTemplate? AndConditionTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for <see cref="OrCondition"/>.
        /// </summary>
        public DataTemplate? OrConditionTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for <see cref="ICondition"/>.
        /// </summary>
        public DataTemplate? NodeConditionTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for <see cref="ICondition"/>.
        /// </summary>
        public DataTemplate? DefaultTemplate { get; set; }

        /// <inheritdoc/>
        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            return item switch
            {
                AndCondition _ => this.AndConditionTemplate ?? this.DefaultTemplate,
                OrCondition _ => this.OrConditionTemplate ?? this.DefaultTemplate,
                NegatedCondition _ => this.NegatedConditionTemplate ?? this.DefaultTemplate,
                ICondition _ => this.NodeConditionTemplate ?? this.DefaultTemplate,
                _ => base.SelectTemplate(null, container),
            };
        }
    }
}
