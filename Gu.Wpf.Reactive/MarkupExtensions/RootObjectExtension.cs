namespace Gu.Wpf.Reactive
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Xaml;

    /// <summary>
    /// <see cref="MarkupExtension"/> for binding to the root object when not in the visual tree.
    /// </summary>
    [MarkupExtensionReturnType(typeof(ContentControl))]
    public class RootObjectExtension : MarkupExtension
    {
        /// <inheritdoc/>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var rootObjectProvider = (IRootObjectProvider)serviceProvider.GetService(typeof(IRootObjectProvider));
            return rootObjectProvider?.RootObject;
        }
    }
}
