#pragma warning disable SA1600 // Elements must be documented
#pragma warning disable SA1401 // Fields must be private
namespace Gu.Wpf.Reactive
{
    using System.ComponentModel;
    using System.Windows;

    internal static class DesignMode
    {
        internal static bool? OverrideIsDesignTime = null;

        private static readonly DependencyObject DependencyObject = new DependencyObject();

        internal static bool IsDesignTime
        {
            get
            {
                if (OverrideIsDesignTime.HasValue)
                {
                    return OverrideIsDesignTime.Value;
                }

                return DesignerProperties.GetIsInDesignMode(DependencyObject);
            }
        }
    }
}
