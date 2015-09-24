namespace Gu.Reactive
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    public class NotifyCollectionChangedEventArgs<T>
    {
        private readonly NotifyCollectionChangedEventArgs _args;

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedEventArgs args)
        {
            _args = args;
        }

        public IEnumerable<T> NewItems => _args.NewItems?.Cast<T>() ?? Enumerable.Empty<T>();

        public int NewStartingIndex => _args.NewStartingIndex;

        public IEnumerable<T> OldItems => _args.OldItems?.Cast<T>() ?? Enumerable.Empty<T>();

        public int OldStartingIndex => _args.OldStartingIndex;

        public NotifyCollectionChangedAction Action => _args.Action;
    }
}