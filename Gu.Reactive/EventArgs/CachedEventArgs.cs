namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    internal static class CachedEventArgs
    {
        internal static readonly PropertyChangedEventArgs CountPropertyChanged = new PropertyChangedEventArgs("Count");
        internal static readonly PropertyChangedEventArgs IndexerPropertyChanged = new PropertyChangedEventArgs("Item[]");

        internal static readonly NotifyCollectionChangedEventArgs NotifyCollectionReset = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

        internal static readonly IReadOnlyList<NotifyCollectionChangedEventArgs> SingleNotifyCollectionReset = new[] { NotifyCollectionReset };

        internal static readonly IReadOnlyList<EventArgs> ResetEventArgsCollection = new EventArgs[]
                                                                                         {
                                                                                             CountPropertyChanged,
                                                                                             IndexerPropertyChanged,
                                                                                             NotifyCollectionReset
                                                                                         };

        internal static readonly IReadOnlyList<NotifyCollectionChangedEventArgs> EmptyArgs = new NotifyCollectionChangedEventArgs[0];
    }
}