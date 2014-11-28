namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reactive.Linq;
    using System.Windows.Data;

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
            observable.Subscribe(x => this.Refresh());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="prop">The view is refreshed when propertychanged for this property is raised</param>
        public static CollectionView<TItem> Create<TSource, TItem>(TSource source, Expression<Func<TSource, IEnumerable<TItem>>> prop) where TSource : INotifyPropertyChanged
        {
            var view = new CollectionView<TItem>(
                prop.Compile()
                    .Invoke(source));

            PropertyChangedEventManager.AddListener(source, view, ((MemberExpression)prop.Body).Member.Name);
            return view;
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

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.Cast<T>().GetEnumerator();
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            this.Refresh();
            return true;
        }
    }
}
