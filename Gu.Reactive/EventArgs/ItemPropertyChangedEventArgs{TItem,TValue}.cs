namespace Gu.Reactive
{
    using System.ComponentModel;

    using Gu.Reactive.Internals;

    /// <summary>
    /// The property changed event args.
    /// </summary>
    public class ItemPropertyChangedEventArgs<TItem, TValue> : PropertyChangedEventArgs
    {
        private readonly SourceAndValue<TValue> sourceAndValue;

        internal ItemPropertyChangedEventArgs(TItem item, SourceAndValue<TValue> sourceAndValue, string propertyName) 
            : base(propertyName)
        {
            this.sourceAndValue = sourceAndValue;
            this.Item = item;
        }

        /// <summary>
        /// Gets the item in the collection.
        /// Note that the item can be in many places in the collection.
        /// </summary>
        public TItem Item { get; }

        /// <summary>
        /// The source for <see cref="Value"/> or a the first non null item in the property path.
        /// </summary>
        public object ValueSource => this.sourceAndValue.Source;

        /// <summary>
        /// True if the last source in the property path was not null.
        /// Note that it can be true even if <see cref="Value"/> is null.
        /// </summary>
        public bool HasValue => this.sourceAndValue.Value.HasValue;

        /// <summary>
        /// Gets the current value.
        /// This is not guaranteed to be the value when the event was raised in a multithreaded scenario.
        /// Returns default(TValue) if <see cref="HasValue"/> is false.
        /// </summary>
        public TValue Value => this.sourceAndValue.Value.GetValueOrDefault();
    }
}