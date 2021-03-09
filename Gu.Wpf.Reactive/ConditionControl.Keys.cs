#pragma warning disable SA1600 // Elements must be documented
#pragma warning disable SA1601 // Elements must be documented
#pragma warning disable 1591
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

        public static ResourceKey TreeViewItemStyleKey { get; } = CreateKey();

        public static ResourceKey AllExpandedTreeViewItemStyleKey { get; } = CreateKey();

        public static ResourceKey CollapseSatisfiedTreeViewItemStyleKey { get; } = CreateKey();

        public static ICommand SetAllExpanded { get; } = new RelayCommand(() => SetExpandedStyle(AllExpandedTreeViewItemStyleKey));

        public static ICommand SetCollapseSatisfied { get; } = new RelayCommand(() => SetExpandedStyle(CollapseSatisfiedTreeViewItemStyleKey));

        private static ComponentResourceKey CreateKey([CallerMemberName] string? caller = null)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return new ComponentResourceKey(typeof(ConditionControl), caller);
        }

        private static void SetExpandedStyle(ResourceKey key)
        {
            if (Application.Current.TryFindResource(key) is Style style &&
                style.TargetType == typeof(TreeViewItem))
            {
                Application.Current.Resources[TreeViewItemStyleKey] = style;
            }
        }
    }
}
