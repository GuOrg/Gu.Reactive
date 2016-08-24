namespace Gu.Wpf.Reactive
{
    using System.ComponentModel;
    using System.Windows;

    internal static class DesignMode
    {
        // referencing tooltips here so it gets copied
        private static readonly DependencyObject _dependencyObject = new DependencyObject();
        internal static bool? OverrideIsDesignTime = null;
        internal static bool IsDesignTime
        {
            get
            {
                if (OverrideIsDesignTime.HasValue)
                {
                    return OverrideIsDesignTime.Value;
                }
                return DesignerProperties.GetIsInDesignMode(_dependencyObject);
            }
        }
    }
}
