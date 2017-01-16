namespace Gu.Reactive
{
    using System.ComponentModel;
    using System.Reactive;

    /// <summary>
    /// The property changed event args.
    /// </summary>
    public class ItemPropertyChangedEventArgs<TItem, TValue> : PropertyChangedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemPropertyChangedEventArgs{TItem, TValue}"/> class.
        /// </summary>
        public ItemPropertyChangedEventArgs(TItem item, EventPattern<PropertyChangedAndValueEventArgs<TValue>> e)
            : base(e.EventArgs.PropertyName)
        {
            this.Item = item;
            this.Value = e.EventArgs.Value;
            this.Sender = e.Sender;
        }

        /// <summary>
        /// Gets the sender.
        /// </summary>
        public TItem Item { get; }

        /// <summary>
        /// Gets the current value.
        /// This is not guaranteed to be the value when the event was raised in a multithreaded scenario.
        /// </summary>
        public TValue Value { get; }

        /// <summary>
        /// The original sender.
        /// </summary>
        public object Sender { get; }
    }
}