namespace Gu.Reactive
{
    /// <summary>
    /// The property changed tracking event args.
    /// </summary>
    /// <typeparam name="TSource">
    /// </typeparam>
    /// <typeparam name="TProperty">
    /// </typeparam>
    public class TrackingEventArgs<TSource, TProperty> : ChildPropertyChangedEventArgs<TSource, TProperty>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackingEventArgs{TSource,TProperty}"/> class.
        /// </summary>
        /// <param name="originalSender">
        /// The sender.
        /// </param>
        /// <param name="currentValue">
        /// The current value.
        /// </param>
        /// <param name="previousValue">
        /// The previous value.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        public TrackingEventArgs(TSource originalSender, TProperty currentValue, TProperty previousValue, string propertyName)
            : base(originalSender, currentValue, propertyName)
        {
            PreviousValue = previousValue;
        }

        /// <summary>
        /// Gets the previous value.
        /// </summary>
        public TProperty PreviousValue { get; private set; }
    }
}