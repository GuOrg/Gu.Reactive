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
        /// The template for <see cref="NegatedCondition"/>.
        /// </summary>
        public DataTemplate? NegatedConditionTemplate { get; set; }

        /// <summary>
        /// The template for <see cref="AndCondition"/>.
        /// </summary>
        public DataTemplate? AndConditionTemplate { get; set; }

        /// <summary>
        /// The template for <see cref="OrCondition"/>.
        /// </summary>
        public DataTemplate? OrConditionTemplate { get; set; }

        /// <summary>
        /// The template for <see cref="ICondition"/>.
        /// </summary>
        public DataTemplate? NodeConditionTemplate { get; set; }

        /// <inheritdoc/>
        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            return item switch
            {
                AndCondition _ => this.AndConditionTemplate,
                OrCondition _ => this.OrConditionTemplate,
                NegatedCondition _ => this.NegatedConditionTemplate,
                ICondition _ => this.NodeConditionTemplate,
                _ => base.SelectTemplate(null, container),
            };
        }
    }
}
