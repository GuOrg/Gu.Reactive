namespace Gu.Wpf.Reactive
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Xaml;

#pragma warning disable WPF0081 // MarkupExtensionReturnType must use correct return type.
                               /// <summary>
                               /// Markupextension for binding to the root object when not in the visual tree.
                               /// </summary>
    [MarkupExtensionReturnType(typeof(ContentControl))]
#pragma warning restore WPF0081 // MarkupExtensionReturnType must use correct return type.
    public class RootObjectExtension : MarkupExtension
    {
        /// <inheritdoc/>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var rootObjectProvider = (IRootObjectProvider)serviceProvider.GetService(typeof(IRootObjectProvider));
            return rootObjectProvider?.RootObject;
        }
    }
}