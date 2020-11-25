namespace Gu.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reactive;
    using Gu.Reactive.Internals;

    /// <summary>
    /// Obsolete overloads.
    /// </summary>
    public static partial class NotifyPropertyChangedExt
    {
        /// <summary>
        /// Observe property changes with values.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="property">An expression specifying the property path.</param>
        /// <param name="signalInitial">If true OnNext is called immediately on subscribe.</param>
        /// <typeparam name="TNotifier">The type of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the last property in the path.</typeparam>
        /// <returns>The <see cref="IObservable{T}"/> of type of type <see cref="EventPattern{TEventArgs}"/> of type <see cref="PropertyChangedAndValueEventArgs{TProperty}"/>.</returns>
        [Obsolete("Removing this as it is messy and hard to use. Use ObserveValue instead.")]
        public static IObservable<EventPattern<PropertyChangedAndValueEventArgs<TProperty>>> ObservePropertyChangedWithValue<TNotifier, TProperty>(
            this TNotifier source,
            Expression<Func<TNotifier, TProperty>> property,
            bool signalInitial = true)
            where TNotifier : class, INotifyPropertyChanged
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            var notifyingPath = NotifyingPath.GetOrCreate(property);
            return source.ObservePropertyChangedWithValue(notifyingPath, signalInitial);
        }

        internal static IObservable<EventPattern<PropertyChangedAndValueEventArgs<TProperty>>> ObservePropertyChangedWithValue<TNotifier, TProperty>(
            this TNotifier source,
            NotifyingPath<TNotifier, TProperty> propertyPath,
            bool signalInitial = true)
            where TNotifier : class, INotifyPropertyChanged
        {
            return source.ObserveValueCore(
                propertyPath,
                (sender, e, value) => new EventPattern<PropertyChangedAndValueEventArgs<TProperty>>(
                    sender,
                    new PropertyChangedAndValueEventArgs<TProperty>(e.PropertyName, value)),
                signalInitial);
        }
    }
}
