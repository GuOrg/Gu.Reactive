// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyChangedEventArgs.cs" company="">
//   
// </copyright>
// <summary>
//   The property changed event args.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
    public class PropertyChangedEventArgs<TSource, TProperty> : PropertyChangedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedEventArgs{TSource,TProperty}"/> class.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="currentValue">
        /// The current value.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        public PropertyChangedEventArgs(TSource sender, TProperty currentValue, string propertyName)
            : base(propertyName)
        {
            Sender = sender;
            CurrentValue = currentValue;
        }

        /// <summary>
        /// Gets the sender.
        /// </summary>
        public TSource Sender { get; private set; }

        /// <summary>
        /// Gets the current value.
        /// </summary>
        public TProperty CurrentValue { get; private set; }
    }
}