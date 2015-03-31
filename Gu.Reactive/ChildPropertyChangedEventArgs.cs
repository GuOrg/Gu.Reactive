namespace Gu.Reactive
{
    using System.ComponentModel;

    /// <summary>
    /// The property changed event args.
    /// </summary>
    /// <typeparam name="TSource">
    /// </typeparam>
    /// <typeparam name="TValue">
    /// </typeparam>
    public class ChildPropertyChangedEventArgs<TSource, TValue> : PropertyChangedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChildPropertyChangedEventArgs{TSource,TProperty}"/> class.
        /// </summary>
        /// <param name="originalSender">
        /// The sender.
        /// </param>
        /// <param name="value">
        /// The current value.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        public ChildPropertyChangedEventArgs(TSource originalSender, TValue value, string propertyName)
            : base(propertyName)
        {
            OriginalSender = originalSender;
            Value = value;
        }

        public ChildPropertyChangedEventArgs(INotifyPropertyChanged s, PropertyChangedAndValueEventArgs<TValue> e) 
            : base(e.PropertyName)
        {
            OriginalSender = (TSource) s;
            Value = e.Value;
        }

        /// <summary>
        /// Gets the sender.
        /// </summary>
        public TSource OriginalSender { get; private set; }

        /// <summary>
        /// Gets the current value.
        /// </summary>
        public TValue Value { get; private set; }
    }
}