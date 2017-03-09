namespace Gu.Reactive.Internals
{
    using System.ComponentModel;
    using System.Reflection;

    internal class NotifyingGetter<TNotifier, TValue> : ClassGetter<TNotifier, TValue>, INotifyingGetter
        where TNotifier : class, INotifyPropertyChanged
    {
        protected NotifyingGetter(PropertyInfo property)
            : base(property)
        {
        }

        IPropertyTracker INotifyingGetter.CreateTracker(IPropertyPathTracker tracker)
        {
            return new PropertyTracker<TNotifier, TValue>(tracker, this);
        }
    }
}