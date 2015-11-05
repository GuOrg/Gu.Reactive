namespace Gu.Wpf.Reactive
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Xaml;

    [MarkupExtensionReturnType(typeof(ContentControl))]
    public class RootObjectExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var rootObjectProvider = (IRootObjectProvider)serviceProvider.GetService(typeof(IRootObjectProvider));
            return rootObjectProvider?.RootObject;
        }
    }
}