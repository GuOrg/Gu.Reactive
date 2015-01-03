namespace Gu.Wpf.Reactive
{
    using System.ComponentModel;
    using System.Windows;

    internal static class DesignMode
    {
        private static readonly DependencyObject _dependencyObject = new DependencyObject();
        internal static bool IsDesignTime { get { return DesignerProperties.GetIsInDesignMode(_dependencyObject); } }
    }
}
