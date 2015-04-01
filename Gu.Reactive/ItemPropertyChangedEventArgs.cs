namespace Gu.Reactive
{
    using System.ComponentModel;

    /// <summary>
    /// The property changed event args.
    /// </summary>
    /// <typeparam name="TItem">
    /// </typeparam>
    /// <typeparam name="TValue">
    /// </typeparam>
    public class ItemPropertyChangedEventArgs<TItem, TValue> : PropertyChangedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemPropertyChangedEventArgs{TSource,TValue}"/> class.
        /// </summary>
        /// <param name="item">
        /// The sender.
        /// </param>
        /// <param name="value">
        /// The current value.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        public ItemPropertyChangedEventArgs(TItem item, TValue value, string propertyName)
            : base(propertyName)
        {
            Item = item;
            Value = value;
        }

        public ItemPropertyChangedEventArgs(TItem item, PropertyChangedAndValueEventArgs<TValue> e) 
            : base(e.PropertyName)
        {
            Item =  item;
            Value = e.Value;
        }

        /// <summary>
        /// Gets the sender.
        /// </summary>
        public TItem Item { get; private set; }

        /// <summary>
        /// Gets the current value.
        /// </summary>
        public TValue Value { get; private set; }
    }
}