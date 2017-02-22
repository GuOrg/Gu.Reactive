namespace Gu.Reactive
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    internal static class CachedEventArgs
    {
        internal static readonly ConcurrentDictionary<string, PropertyChangedEventArgs> Cache = new ConcurrentDictionary<string, PropertyChangedEventArgs>();
        internal static readonly PropertyChangedEventArgs CountPropertyChanged = GetOrCreatePropertyChangedEventArgs("Count");
        internal static readonly PropertyChangedEventArgs IndexerPropertyChanged = GetOrCreatePropertyChangedEventArgs("Item[]");
        internal static readonly PropertyChangedEventArgs IsSatisfiedChanged = new PropertyChangedEventArgs(nameof(ICondition.IsSatisfied));

        internal static readonly NotifyCollectionChangedEventArgs NotifyCollectionReset = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

        internal static readonly IReadOnlyList<NotifyCollectionChangedEventArgs> SingleNotifyCollectionReset = new[] { NotifyCollectionReset };

        internal static readonly IReadOnlyList<EventArgs> ResetEventArgsCollection = new EventArgs[]
                                                                                         {
                                                                                             CountPropertyChanged,
                                                                                             IndexerPropertyChanged,
                                                                                             NotifyCollectionReset
                                                                                         };

        internal static readonly IReadOnlyList<NotifyCollectionChangedEventArgs> EmptyArgs = new NotifyCollectionChangedEventArgs[0];

        internal static PropertyChangedEventArgs GetOrCreatePropertyChangedEventArgs(string propertyName)
        {
            return Cache.GetOrAdd(propertyName, name => new PropertyChangedEventArgs(name));
        }
    }
}