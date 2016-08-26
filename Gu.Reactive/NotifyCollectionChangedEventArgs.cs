namespace Gu.Reactive
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    public class NotifyCollectionChangedEventArgs<T>
    {
        private readonly NotifyCollectionChangedEventArgs args;

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedEventArgs args)
        {
            this.args = args;
        }

        public IEnumerable<T> NewItems => this.args.NewItems?.Cast<T>() ?? Enumerable.Empty<T>();

        public int NewStartingIndex => this.args.NewStartingIndex;

        public IEnumerable<T> OldItems => this.args.OldItems?.Cast<T>() ?? Enumerable.Empty<T>();

        public int OldStartingIndex => this.args.OldStartingIndex;

        public NotifyCollectionChangedAction Action => this.args.Action;
    }
}