#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements must be documented
namespace Gu.Wpf.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Markup;
    using System.Xaml;

    [Obsolete("To be removed.")]
    public static class ServiceProviderExt
    {
        [Obsolete("To be removed.")]
        public static IXamlTypeResolver XamlTypeResolver(this IServiceProvider provider)
        {
            return provider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
        }

        [Obsolete("To be removed.")]
        public static IProvideValueTarget ProvideValueTarget(this IServiceProvider provider)
        {
            return provider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        }

        [Obsolete("To be removed.")]
        public static IRootObjectProvider RootObjectProvider(this IServiceProvider provider)
        {
            return provider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;
        }

        [Obsolete("To be removed.")]
        public static ITypeDescriptorContext TypeDescriptorContext(this IServiceProvider provider)
        {
            return provider.GetService(typeof(ITypeDescriptorContext)) as ITypeDescriptorContext;
        }

        [Obsolete("To be removed.")]
        public static IUriContext UriContext(this IServiceProvider provider)
        {
            return provider.GetService(typeof(IUriContext)) as IUriContext;
        }

        [Obsolete("To be removed.")]
        public static IXamlNameResolver XamlNameResolver(this IServiceProvider provider)
        {
            return provider.GetService(typeof(IXamlNameResolver)) as IXamlNameResolver;
        }

        [Obsolete("To be removed.")]
        public static IXamlNamespaceResolver XamlNamespaceResolver(this IServiceProvider provider)
        {
            return provider.GetService(typeof(IXamlNamespaceResolver)) as IXamlNamespaceResolver;
        }

        [Obsolete("To be removed.")]
        public static IXamlSchemaContextProvider XamlSchemaContextProvider(this IServiceProvider provider)
        {
            return provider.GetService(typeof(IXamlSchemaContextProvider)) as IXamlSchemaContextProvider;
        }

        [Obsolete("To be removed.")]
        public static bool IsInTemplate(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget target && !(target.TargetObject is DependencyObject);
        }
    }
}
