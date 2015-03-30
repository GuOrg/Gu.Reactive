namespace Gu.Reactive
{
    using System.ComponentModel;

    /// <summary>
    /// The property changed event args.
    /// </summary>
    /// <typeparam name="TSource">
    /// </typeparam>
    /// <typeparam name="TProperty">
    /// </typeparam>
    public class ChildPropertyChangedEventArgs<TSource, TProperty> : PropertyChangedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChildPropertyChangedEventArgs{TSource,TProperty}"/> class.
        /// </summary>
        /// <param name="originalSender">
        /// The sender.
        /// </param>
        /// <param name="currentValue">
        /// The current value.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        public ChildPropertyChangedEventArgs(TSource originalSender, TProperty currentValue, string propertyName)
            : base(propertyName)
        {
            OriginalSender = originalSender;
            CurrentValue = currentValue;
        }

        /// <summary>
        /// Gets the sender.
        /// </summary>
        public TSource OriginalSender { get; private set; }

        /// <summary>
        /// Gets the current value.
        /// </summary>
        public TProperty CurrentValue { get; private set; }
    }
}