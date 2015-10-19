namespace Gu.Wpf.Reactive
{
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class ConditionControl
    {
        public static ResourceKey ConditionTemplateKey { get; } = CreateKey();

        public static ResourceKey SingleConditionControlStyleKey { get; } = CreateKey();

        public static ResourceKey TreeviewItemStyleKey { get; } = CreateKey();

        public static ResourceKey AllExpandedTreeViewItemStyleKey { get; } = CreateKey();

        public static ResourceKey CollapseSatisfiedTreeViewItemStyleKey { get; } = CreateKey();

        public static ICommand SetAllExpanded { get; } = new RelayCommand(() => SetExpandedStyle(AllExpandedTreeViewItemStyleKey));

        public static ICommand SetCollapseSatisfied { get; } = new RelayCommand(() => SetExpandedStyle(CollapseSatisfiedTreeViewItemStyleKey));

        private static ComponentResourceKey CreateKey([CallerMemberName] string caller = null)
        {
            return new ComponentResourceKey(typeof(ConditionControl), caller);
        }

        private static void SetExpandedStyle(ResourceKey key)
        {
            var style = Application.Current.TryFindResource(key) as Style;
            if (style != null && style.TargetType == typeof(TreeViewItem))
            {
                Application.Current.Resources[TreeviewItemStyleKey] = style;
            }
        }
    }
}
