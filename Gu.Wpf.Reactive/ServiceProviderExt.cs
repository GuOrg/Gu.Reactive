namespace Gu.Wpf.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Markup;
    using System.Xaml;

    public static class ServiceProviderExt
    {
        public static IXamlTypeResolver XamlTypeResolver(this IServiceProvider provider)
        {
            return provider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
        }

        public static IProvideValueTarget ProvideValueTarget(this IServiceProvider provider)
        {
            return provider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        }

        public static IRootObjectProvider RootObjectProvider(this IServiceProvider provider)
        {
            return provider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;
        }

        public static ITypeDescriptorContext TypeDescriptorContext(this IServiceProvider provider)
        {
            return provider.GetService(typeof(ITypeDescriptorContext)) as ITypeDescriptorContext;
        }

        public static IUriContext UriContext(this IServiceProvider provider)
        {
            return provider.GetService(typeof(IUriContext)) as IUriContext;
        }

        public static IXamlNameResolver XamlNameResolver(this IServiceProvider provider)
        {
            return provider.GetService(typeof(IXamlNameResolver)) as IXamlNameResolver;
        }

        public static IXamlNamespaceResolver XamlNamespaceResolver(this IServiceProvider provider)
        {
            return provider.GetService(typeof(IXamlNamespaceResolver)) as IXamlNamespaceResolver;
        }

        public static IXamlSchemaContextProvider XamlSchemaContextProvider(this IServiceProvider provider)
        {
            return provider.GetService(typeof(IXamlSchemaContextProvider)) as IXamlSchemaContextProvider;
        }

        public static bool IsInTemplate(this IServiceProvider serviceProvider)
        {
            var target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            return target != null && !(target.TargetObject is DependencyObject);
        }
    }
}
