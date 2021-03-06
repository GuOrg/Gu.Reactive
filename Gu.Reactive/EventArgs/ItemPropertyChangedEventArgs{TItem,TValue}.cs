﻿namespace Gu.Reactive
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// The property changed event args.
    /// </summary>
    /// <typeparam name="TItem">The type of the items in the collection.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public class ItemPropertyChangedEventArgs<TItem, TValue> : PropertyChangedEventArgs
    {
        internal ItemPropertyChangedEventArgs(TItem? item, SourceAndValue<INotifyPropertyChanged?, TValue> sourceAndValue, string propertyName)
            : base(propertyName)
        {
            this.SourceAndValue = sourceAndValue;
            this.Item = item;
        }

        /// <summary>
        /// Gets the item in the collection or null if it was removed.
        /// Note that the item can be in many places in the collection.
        /// </summary>
        public TItem? Item { get; }

        /// <summary>
        /// Gets the source of the last node in the property path that is not null.
        /// The value is the value of the end node in the property path or <see cref="Maybe{T}.None"/> if it is null.
        /// This is not guaranteed to be the value when the event was raised in a multi threaded scenario.
        /// </summary>
        public SourceAndValue<INotifyPropertyChanged?, TValue> SourceAndValue { get; }

        /// <summary>
        /// Gets SourceAndValue.Value.GetValueOrDefault().
        /// This is not guaranteed to be the value when the event was raised in a multi threaded scenario.
        /// </summary>
        [Obsolete("Use SourceAndValue")]
        public TValue? Value => this.SourceAndValue.Value.GetValueOrDefault();
    }
}
