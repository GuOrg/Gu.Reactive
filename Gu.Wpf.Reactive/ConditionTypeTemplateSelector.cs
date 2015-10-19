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
                    return AndConditionTemplate;
                }

                if (item is OrCondition)
                {
                    return OrConditionTemplate;
                }

                if (item is NegatedCondition)
                {
                    return NegatedConditionTemplate;
                }

                return NodeConditionTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}