namespace Gu.Wpf.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;
    using System.Xaml;

    /// <summary>
    /// Binds to the datacontext of the current root object or elementname
    /// Us this for things that are not in the visual tree such as DataGridColumn
    /// </summary>
    [MarkupExtensionReturnType(typeof(object))]
    public class NinjaBinding : MarkupExtension
    {
        private static readonly DependencyObject DependencyObject = new DependencyObject();
        private static readonly string[] DoNotCopy = { "Path", "Source", "ElementName", "RelativeSource", "ValidationRules" };
        private static readonly PropertyInfo[] CopyProperties = typeof(Binding).GetProperties().Where(x => !DoNotCopy.Contains(x.Name)).ToArray();
        public NinjaBinding()
        {
        }

        public NinjaBinding(Binding binding)
        {
            Binding = binding;
        }

        [ConstructorArgument("binding")]
        public Binding Binding { get; set; }

        private bool IsInDesignMode
        {
            get { return DesignerProperties.GetIsInDesignMode(DependencyObject); }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Binding == null)
            {
                throw new ArgumentException("Binding == null");
            }
            if (IsInDesignMode)
            {
                if (Binding.RelativeSource != null)
                {
                    throw new NotSupportedException("NinjaBinding does not support Binding with RelativeSource, try using ElementName instead.");
                }
                return DefaultValue(serviceProvider);
            }
            Binding binding = null;
            if (Binding.ElementName != null)
            {
                var reference = new Reference(Binding.ElementName);
                var source = reference.ProvideValue(serviceProvider) as FrameworkElement;
                if (source == null)
                {
                    var rootObjectProvider = (IRootObjectProvider)serviceProvider.GetService(typeof(IRootObjectProvider));
                    if (rootObjectProvider == null)
                    {
                        throw new ArgumentException(string.Format("Could not resolve element: {0}", Binding.ElementName));
                    }
                    var root = rootObjectProvider.RootObject as FrameworkElement;
                    if (root != null && root.Name == Binding.ElementName)
                    {
                        source = root;
                    }
                    else
                    {
                        throw new ArgumentException(string.Format("Could not resolve element: {0}", Binding.ElementName));
                    }
                }
                binding = CreateElementNameBinding(Binding, source);
            }
            else if (Binding.RelativeSource != null)
            {
                return null;
            }
            else
            {
                var rootObjectProvider = (IRootObjectProvider)serviceProvider.GetService(typeof(IRootObjectProvider));
                if (rootObjectProvider == null)
                {
                    throw new ArgumentException("rootObjectProvider == null");
                }
                binding = CreateDataContextBinding((FrameworkElement)rootObjectProvider.RootObject, Binding);
            }

            var provideValue = binding.ProvideValue(serviceProvider);
            return provideValue;
        }

        private static Binding CreateElementNameBinding(Binding original, object source)
        {
            var binding = new Binding()
            {
                Path = original.Path,
                Source = source,
            };
            SyncProperties(original, binding);
            return binding;
        }

        private static Binding CreateDataContextBinding(FrameworkElement rootObject, Binding original)
        {
            string path = string.Format("{0}.{1}", FrameworkElement.DataContextProperty.Name, original.Path.Path);
            var binding = new Binding(path)
            {
                Source = rootObject,
            };
            SyncProperties(original, binding);
            return binding;
        }

        private static void SyncProperties(Binding source, Binding target)
        {
            foreach (var copyProperty in CopyProperties)
            {
                var value = copyProperty.GetValue(source);
                copyProperty.SetValue(target, value);
            }
            foreach (var rule in source.ValidationRules)
            {
                target.ValidationRules.Add(rule);
            }
        }

        private static object DefaultValue(IServiceProvider serviceProvider)
        {
            var provideValueTarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            if (provideValueTarget == null)
            {
                throw new ArgumentException("provideValueTarget == null");
            }
            var dependencyProperty = (DependencyProperty)provideValueTarget.TargetProperty;
            return dependencyProperty.DefaultMetadata.DefaultValue;
        }
    }
}
