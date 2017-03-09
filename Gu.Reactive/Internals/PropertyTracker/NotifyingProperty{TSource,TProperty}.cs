namespace Gu.Reactive.Internals
{
    using System.ComponentModel;

    internal class NotifyingProperty<TSource, TValue> : PathProperty<TSource, TValue>, INotifyingProperty
        where TSource : class, INotifyPropertyChanged
    {
        public NotifyingProperty(INotifyingProperty previous, Getter<TSource, TValue> getter)
            : base(previous, getter)
        {
        }

        public PathPropertyTracker<TSource, TValue> CreateTracker(IPropertyPathTracker tracker) => new PathPropertyTracker<TSource, TValue>(tracker, this);

        IPathPropertyTracker INotifyingProperty.CreateTracker(IPropertyPathTracker tracker) => this.CreateTracker(tracker);
    }
}