namespace Gu.Reactive
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    /// <summary>
    /// A generic decorator for <see cref="NotifyCollectionChangedEventArgs"/>
    /// </summary>
    public struct NotifyCollectionChangedEventArgs<T>
    {
        private static readonly IReadOnlyList<T> Empty = new T[0];

        private readonly NotifyCollectionChangedEventArgs args;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyCollectionChangedEventArgs{T}"/> struct.
        /// </summary>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedEventArgs args)
        {
            this.args = args;
            this.NewItems = GetItems(args.NewItems);
            this.OldItems = GetItems(args.OldItems);
        }

        /// <summary>Gets the list of new items involved in the change.</summary>
        public IReadOnlyList<T> NewItems { get; }

        /// <summary>Gets the list of items affected by a <see cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Replace" />, Remove, or Move action.</summary>
        public IReadOnlyList<T> OldItems { get; }

        /// <summary>Gets the action that caused the event. </summary>
        public NotifyCollectionChangedAction Action => this.args.Action;

        /// <summary>Gets the index at which the change occurred.</summary>
        public int NewStartingIndex => this.args.NewStartingIndex;

        /// <summary>Gets the index at which a <see cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Move" />, Remove, or Replace action occurred.</summary>
        public int OldStartingIndex => this.args.OldStartingIndex;

        private static IReadOnlyList<T> GetItems(IList items)
        {
            if (items == null || items.Count == 0)
            {
                return Empty;
            }

            if (items.Count == 1)
            {
                return new[] { (T)items[0] };
            }

            return items.Cast<T>()
                        .ToArray();
        }
    }
}
