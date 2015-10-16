namespace Gu.Wpf.Reactive
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Data;

    internal static class BindingHelper
    {
        private static readonly Dictionary<string, PropertyPath> PropertyPaths = new Dictionary<string, PropertyPath>();

        internal static Binding CreateOneWayBinding(this DependencyObject source, DependencyProperty sourceProperty)
        {
            var propertyPath = AsPropertyPath(sourceProperty);
            return new Binding { Path = propertyPath, Source = source, Mode = BindingMode.OneWay };
        }

        internal static Binding CreateOneWayBinding(this DependencyObject source, DependencyProperty sourceProperty1, DependencyProperty sourceProperty2)
        {
            var propertyPath = GetPath(sourceProperty1, sourceProperty2);
            return new Binding
            {
                Path = propertyPath,
                Source = source,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
        }

        internal static PropertyPath AsPropertyPath(this DependencyProperty property)
        {
            return GetPath(property.Name);
        }

        internal static PropertyPath GetPath(DependencyProperty property1, DependencyProperty property2)
        {
            return GetPath($"{property1.Name}.{property2.Name}");
        }

        internal static PropertyPath GetPath(string path)
        {
            PropertyPath propertyPath;
            if (!PropertyPaths.TryGetValue(path, out propertyPath))
            {
                propertyPath = new PropertyPath(path);
                PropertyPaths[path] = propertyPath;
            }
            return propertyPath;
        }
    }
}
