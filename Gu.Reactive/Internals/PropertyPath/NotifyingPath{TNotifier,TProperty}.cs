namespace Gu.Reactive.Internals
{
    using System.Collections.Generic;
    using System.ComponentModel;

    internal class NotifyingPath<TNotifier, TProperty> : PropertyPath<TNotifier, TProperty, INotifyingProperty>
        where TNotifier : class, INotifyPropertyChanged
    {
        internal NotifyingPath(IReadOnlyList<INotifyingProperty> parts)
            : base(parts)
        {
        }

        internal PropertyPathTracker<TNotifier, TProperty> CreateTracker(TNotifier item) => new PropertyPathTracker<TNotifier, TProperty>(item, this);
    }
}