// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyChangedTrackingEventArgs.cs" company="">
//   
// </copyright>
// <summary>
//   The property changed tracking event args.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gu.Reactive
{
    /// <summary>
    /// The property changed tracking event args.
    /// </summary>
    /// <typeparam name="TSource">
    /// </typeparam>
    /// <typeparam name="TProperty">
    /// </typeparam>
    public class PropertyChangedTrackingEventArgs<TSource, TProperty> : PropertyChangedEventArgs<TSource, TProperty>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedTrackingEventArgs{TSource,TProperty}"/> class.
        /// </summary>
        /// <param name="sender">
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
        public PropertyChangedTrackingEventArgs(TSource sender, TProperty currentValue, TProperty previousValue, string propertyName)
            : base(sender, currentValue, propertyName)
        {
            PreviousValue = previousValue;
        }

        /// <summary>
        /// Gets the previous value.
        /// </summary>
        public TProperty PreviousValue { get; private set; }
    }
}