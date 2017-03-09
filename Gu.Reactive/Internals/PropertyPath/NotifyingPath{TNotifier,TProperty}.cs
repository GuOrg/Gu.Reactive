namespace Gu.Reactive.Internals
{
    using System.Collections.Generic;
    using System.ComponentModel;

    internal class NotifyingPath<TNotifier, TProperty> : PropertyPath<TNotifier, TProperty>
        where TNotifier : class, INotifyPropertyChanged
    {
        internal NotifyingPath(IReadOnlyList<INotifyingGetter> parts)
            : base(parts)
        {
        }

        internal PropertyPathTracker<TNotifier, TProperty> CreateTracker(TNotifier item) => new PropertyPathTracker<TNotifier, TProperty>(item, this);
    }
}