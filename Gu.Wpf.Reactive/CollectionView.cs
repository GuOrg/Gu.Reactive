﻿namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Windows.Data;
    using Gu.Reactive;

    /// <summary>
    /// Typed CollectionView
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CollectionView<T> : CollectionView, ICollectionView<T>
    {
        /// <summary>
        /// For manual Refresh()
        /// </summary>
        /// <param name="collection"></param>
        public CollectionView(IEnumerable<T> collection)
            : base(collection)
        {
        }

        /// <summary>
        /// Calls Refresh when observable signals
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="updateTrigger"></param>
        public CollectionView(IEnumerable<T> collection, params IObservable<object>[] updateTrigger)
            : base(collection)
        {
            var observable = updateTrigger.Merge();
            observable.ObserveOnSynchronizationContextOrImmediate()
                      .Subscribe(x => Refresh());
        }

        /// <summary>
        /// Creates a view for a property that is IEnumerable<typeparam name="T"></typeparam>
        /// Typical usage Create(this, x => x.Items)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="prop">The view is refreshed when propertychanged for this property is raised</param>
        public static CollectionView<TItem> Create<TSource, TItem>(TSource source, Expression<Func<TSource, IEnumerable<TItem>>> prop)
            where TSource : INotifyPropertyChanged
        {
            return new CollectionView<TItem>(prop.Compile().Invoke(source), source.ToObservable(prop));
        }

        public new Predicate<T> Filter
        {
            get
            {
                return o => base.Filter(o);
            }
            set
            {
                base.Filter = o => value((T)o);
            }
        }

        Predicate<T> ICollectionView<T>.Filter
        {
            get
            {
                return o => base.Filter(o);
            }
            set
            {
                base.Filter = o => value((T)o);
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.Cast<T>().GetEnumerator();
        }
    }
}