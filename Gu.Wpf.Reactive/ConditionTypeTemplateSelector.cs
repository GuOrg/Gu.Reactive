namespace Gu.Wpf.Reactive
{
    using System.Windows;
    using System.Windows.Controls;

    using Gu.Reactive;

    /// <summary>
    /// A <see cref="DataTemplateSelector"/> for <see cref="ICondition"/>
    /// </summary>
    public class ConditionTypeTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// The template for <see cref="NegatedCondition"/>
        /// </summary>
        public DataTemplate NegatedConditionTemplate { get; set; }

        /// <summary>
        /// The template for <see cref="AndCondition"/>
        /// </summary>
        public DataTemplate AndConditionTemplate { get; set; }

        /// <summary>
        /// The template for <see cref="OrCondition"/>
        /// </summary>
        public DataTemplate OrConditionTemplate { get; set; }

        /// <summary>
        /// The template for <see cref="ICondition"/>
        /// </summary>
        public DataTemplate NodeConditionTemplate { get; set; }

        /// <inheritdoc/>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null)
            {
                if (item is AndCondition)
                {
                    return this.AndConditionTemplate;
                }

                if (item is OrCondition)
                {
                    return this.OrConditionTemplate;
                }

                if (item is NegatedCondition)
                {
                    return this.NegatedConditionTemplate;
                }

                return this.NodeConditionTemplate;
            }

            return base.SelectTemplate(null, container);
        }
    }
}