namespace Gu.Wpf.Reactive
{
    using System.Windows;
    using System.Windows.Controls;

    using Gu.Reactive;

    public class ConditionTypeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NegatedConditionTemplate { get; set; }

        public DataTemplate AndConditionTemplate { get; set; }

        public DataTemplate OrConditionTemplate { get; set; }

        public DataTemplate NodeConditionTemplate { get; set; }

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

            return base.SelectTemplate(item, container);
        }
    }
}