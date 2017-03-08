namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq.Expressions;

    internal static class NotifyingPath
    {
        private static readonly ConcurrentDictionary<LambdaExpression, object> Cached = new ConcurrentDictionary<LambdaExpression, object>(PropertyPathComparer.Default);

        internal static NotifyingPath<TNotifier, TProperty> GetOrCreate<TNotifier, TProperty>(Expression<Func<TNotifier, TProperty>> property)
        {
            var notifyingPath = (NotifyingPath<TNotifier, TProperty>)Cached.GetOrAdd(property, p => NotifyingPath<TNotifier, TProperty>.Create((Expression<Func<TNotifier, TProperty>>)p));
            if (!string.IsNullOrEmpty(notifyingPath.ErrorMessage))
            {
                throw new ArgumentException($"Error found in {property}" + Environment.NewLine + notifyingPath.ErrorMessage, nameof(property));
            }

            return notifyingPath;
        }
    }
}