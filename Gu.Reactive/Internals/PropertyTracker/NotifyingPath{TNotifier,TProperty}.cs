namespace Gu.Reactive.Internals
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Text;

    internal class NotifyingPath<TNotifier, TProperty>
    {
        private readonly PropertyPath<TNotifier, TProperty> path;

        private NotifyingPath(PropertyPath<TNotifier, TProperty> path, string errorMessage)
        {
            this.path = path;
            this.ErrorMessage = errorMessage;
        }

        internal string ErrorMessage { get; }

        internal PropertyPath<TNotifier, TProperty> Path
        {
            get
            {
                if (!string.IsNullOrEmpty(this.ErrorMessage))
                {
                    throw new InvalidOperationException($"Error found in {this.path}" + Environment.NewLine + this.ErrorMessage);
                }

                return this.path;
            }
        }

        internal int Count => this.path.Count;

        internal static NotifyingPath<TNotifier, TProperty> Create(Expression<Func<TNotifier, TProperty>> property)
        {
            var path = PropertyPath.GetOrCreate(property);
            var errorBuilder = new StringBuilder();
            for (var i = 0; i < path.Count; i++)
            {
                string errorMessage;
                if (TryGetError(path, i, out errorMessage))
                {
                    errorBuilder.Append(errorMessage);
                    errorBuilder.AppendLine();
                }
            }

            return new NotifyingPath<TNotifier, TProperty>(path, errorBuilder.ToString());
        }

        internal SourceAndValue<TProperty> SourceAndValue(INotifyPropertyChanged root) => this.path.SourceAndValue((TNotifier)root);

        private static bool TryGetError(IPropertyPath path, int i, out string errorMessage)
        {
            var propertyInfo = path[i].PropertyInfo;
            var reflectedType = propertyInfo.ReflectedType;
            if (reflectedType?.IsValueType == true)
            {
                errorMessage = string.Format(
                    "Property path cannot have structs in it. Copy by value will make subscribing error prone. Also mutable struct much?" + Environment.NewLine +
                    "The type {0} is a value type not so {1}.{2} will not notify when it changes." + Environment.NewLine +
                    "The path is: {3}",
                    reflectedType.PrettyName(),
                    i == 0 ? "x" : path[i - 1].PropertyInfo.Name,
                    propertyInfo.Name,
                    path);
                return true;
            }

            if (reflectedType?.IsClass == true &&
                !typeof(INotifyPropertyChanged).IsAssignableFrom(reflectedType))
            {
                errorMessage = string.Format(
                    "All levels in the path must implement INotifyPropertyChanged." + Environment.NewLine +
                    "The type {0} does not so {1}.{2} will not notify when it changes." + Environment.NewLine +
                    "The path is: {3}",
                    reflectedType.PrettyName(),
                    i == 0 ? "x" : path[i - 1].PropertyInfo.Name,
                    propertyInfo.Name,
                    path);
                return true;
            }

            errorMessage = null;
            return false;
        }
    }
}