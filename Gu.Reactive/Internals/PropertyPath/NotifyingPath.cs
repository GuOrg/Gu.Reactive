namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    internal static class NotifyingPath
    {
        private static readonly ConcurrentDictionary<LambdaExpression, object> Cached = new ConcurrentDictionary<LambdaExpression, object>(PropertyPathComparer.Default);

        internal static NotifyingPath<TNotifier, TProperty> GetOrCreate<TNotifier, TProperty>(Expression<Func<TNotifier, TProperty>> property)
            where TNotifier : class?, INotifyPropertyChanged?
        {
            var notifyingPath = Cached.GetOrAdd(property, p => Create((Expression<Func<TNotifier, TProperty>>)p));
            var errorMessage = notifyingPath as string;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new ArgumentException($"Error found in {property}" + Environment.NewLine + errorMessage, nameof(property));
            }

            return (NotifyingPath<TNotifier, TProperty>)notifyingPath;
        }

        private static object Create<TNotifier, TProperty>(Expression<Func<TNotifier, TProperty>> propertyPath)
            where TNotifier : class?, INotifyPropertyChanged?
        {
            var properties = PropertyPathParser.GetPath(propertyPath);
            var errorBuilder = new StringBuilder();
            var parts = new INotifyingGetter[properties.Count];
            for (var i = 0; i < properties.Count; i++)
            {
                if (TryGetError(properties, i, out var errorMessage))
                {
                    errorBuilder.Append(errorMessage);
                    errorBuilder.AppendLine();
                }

                if (errorBuilder.Length == 0)
                {
                    var property = properties[i];
                    var item = Getter.GetOrCreate(property);
                    parts[i] = (INotifyingGetter)item;
                }
            }

            if (errorBuilder.Length != 0)
            {
                return errorBuilder.ToString();
            }

            return new NotifyingPath<TNotifier, TProperty>(parts);
        }

        private static bool TryGetError(IReadOnlyList<PropertyInfo> path, int i, [NotNullWhen(true)] out string? errorMessage)
        {
            var propertyInfo = path[i];
            var reflectedType = propertyInfo.ReflectedType;
            if (reflectedType?.IsValueType == true)
            {
                errorMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    "Property path cannot have structs in it. Copy by value will make subscribing error prone. Also mutable struct much?" + Environment.NewLine +
                    "The type {0} is a value type not so {1}.{2} will not notify when it changes." + Environment.NewLine +
                    "The path is: {3}",
                    reflectedType.PrettyName(),
                    i == 0 ? "x" : path[i - 1].Name,
                    propertyInfo.Name,
                    path.ToPathString());
                return true;
            }

            if (reflectedType?.IsClass == true &&
                !typeof(INotifyPropertyChanged).IsAssignableFrom(reflectedType))
            {
                errorMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    "All levels in the path must implement INotifyPropertyChanged." + Environment.NewLine +
                    "The type {0} does not so {1}.{2} will not notify when it changes." + Environment.NewLine +
                    "The path is: {3}",
                    reflectedType.PrettyName(),
                    i == 0 ? "x" : path[i - 1].Name,
                    propertyInfo.Name,
                    path.ToPathString());
                return true;
            }

            errorMessage = null;
            return false;
        }

        private static string ToPathString(this IReadOnlyList<PropertyInfo> path) => $"x => x.{string.Join(".", path.Select(x => x.Name))}";
    }
}
