namespace Gu.Reactive
{
    using System.ComponentModel;
    using System.Reactive;

    /// <summary>
    /// The property changed event args.
    /// </summary>
    /// <typeparam name="TItem">
    /// </typeparam>
    /// <typeparam name="TValue">
    /// </typeparam>
    public class ItemPropertyChangedEventArgs<TItem, TValue> : PropertyChangedEventArgs
    {
        public ItemPropertyChangedEventArgs(TItem item, EventPattern<PropertyChangedAndValueEventArgs<TValue>> e) 
            : base(e.EventArgs.PropertyName)
        {
            Item =  item;
            Value = e.EventArgs.Value;
            Sender = e.Sender;
        }

        /// <summary>
        /// Gets the sender.
        /// </summary>
        public TItem Item { get; }

        /// <summary>
        /// Gets the current value.
        /// </summary>
        public TValue Value { get; }

        public object Sender { get;  }
    }
}