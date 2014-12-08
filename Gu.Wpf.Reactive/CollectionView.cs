namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Windows.Data;
    using Gu.Reactive;

    /// <summary>
    /// Typed CollectionView
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CollectionView<T> : ICollectionView<T>
    {
        private readonly ICollectionView _view;

        private bool _disposed = false;
        private readonly IDisposable _subscription;

        /// <summary>
        /// For manual Refresh()
        /// </summary>
        /// <param name="collection"></param>
        private CollectionView(CollectionViewSource source, ICollectionView view, params IObservable<object>[] updateTrigger)
        {
            _view = view;
            Source = source;
            var observable = updateTrigger.Merge();
            _subscription = observable.ObserveOnDispatcherOrCurrentThread()
                                      .Subscribe(x => _view.Refresh());
        }

        /// <summary>
        /// Calls Refresh when observable signals
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="updateTrigger"></param>
        public static CollectionView<T> Create(IEnumerable<T> collection, params IObservable<object>[] updateTrigger)
        {
            var source = new CollectionViewSource { Source = collection };
            var view = source.View;
            return new CollectionView<T>(source, view, updateTrigger);
        }

        /// <summary>
        /// Creates a view for a property that is IEnumerable<typeparam name="T"></typeparam>
        /// Typical usage Create(this, x => x.Items)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="prop">The view is refreshed when propertychanged for this property is raised</param>
        public static CollectionView<T> Create<TSource>(TSource source, Expression<Func<TSource, IEnumerable<T>>> prop)
            where TSource : INotifyPropertyChanged
        {
            var enumerable = prop.Compile().Invoke(source);
            return Create(enumerable, source.ToObservable(prop));
        }
        public CollectionViewSource Source { get; private set; }

        public event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                _view.CollectionChanged += value;
            }
            remove
            {
                _view.CollectionChanged -= value;
            }
        }

        public event EventHandler CurrentChanged
        {
            add
            {
                _view.CurrentChanged += value;
            }
            remove
            {
                _view.CurrentChanged -= value;
            }
        }

        public event CurrentChangingEventHandler CurrentChanging
        {
            add
            {
                _view.CurrentChanging += value;
            }
            remove
            {
                _view.CurrentChanging -= value;
            }
        }

        public Predicate<T> Filter
        {
            get
            {
                return o => _view.Filter(o);
            }
            set
            {
                Schedulers.DispatcherOrCurrentThread.Schedule(
                    () =>
                        {
                            _view.Filter = value == null ? (Predicate<object>)null : (o => value((T)o));
                        });
            }
        }

        Predicate<T> ICollectionView<T>.Filter
        {
            get
            {
                return Filter;
            }
            set
            {
                Filter = value;
            }
        }

        Predicate<object> ICollectionView.Filter
        {
            get
            {
                return _view.Filter;
            }
            set
            {
                Schedulers.DispatcherOrCurrentThread.Schedule(
                    () =>
                        {
                            _view.Filter = value;
                        });
            }
        }

        public bool CanFilter
        {
            get { return _view.CanFilter; }
        }

        public bool CanGroup
        {
            get { return _view.CanGroup; }
        }

        public bool CanSort
        {
            get { return _view.CanSort; }
        }

        public bool Contains(object item)
        {
            return _view.Contains(item);
        }

        public System.Globalization.CultureInfo Culture
        {
            get
            {
                return _view.Culture;
            }
            set
            {
                _view.Culture = value;
            }
        }

        public object CurrentItem
        {
            get { return _view.CurrentItem; }
        }

        public int CurrentPosition
        {
            get { return _view.CurrentPosition; }
        }

        public IDisposable DeferRefresh()
        {
            return _view.DeferRefresh();
        }

        public System.Collections.ObjectModel.ObservableCollection<GroupDescription> GroupDescriptions
        {
            get { return _view.GroupDescriptions; }
        }

        public System.Collections.ObjectModel.ReadOnlyObservableCollection<object> Groups
        {
            get { return _view.Groups; }
        }

        public bool IsCurrentAfterLast
        {
            get { return _view.IsCurrentAfterLast; }
        }

        public bool IsCurrentBeforeFirst
        {
            get { return _view.IsCurrentBeforeFirst; }
        }

        public bool IsEmpty
        {
            get { return _view.IsEmpty; }
        }

        public bool MoveCurrentTo(object item)
        {
            return _view.MoveCurrentTo(item);
        }

        public bool MoveCurrentToFirst()
        {
            return _view.MoveCurrentToFirst();
        }

        public bool MoveCurrentToLast()
        {
            return _view.MoveCurrentToLast();
        }

        public bool MoveCurrentToNext()
        {
            return _view.MoveCurrentToNext();
        }

        public bool MoveCurrentToPosition(int position)
        {
            return _view.MoveCurrentToPosition(position);
        }

        public bool MoveCurrentToPrevious()
        {
            return _view.MoveCurrentToPrevious();
        }

        public void Refresh()
        {
            _view.Refresh();
        }

        public SortDescriptionCollection SortDescriptions
        {
            get { return _view.SortDescriptions; }
        }

        public IEnumerable SourceCollection
        {
            get { return _view.SourceCollection; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _view.GetEnumerator();
        }


        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new CastEnumerator<T>(_view.GetEnumerator());
        }

        /// <summary>
        /// Dispose(true); //I am calling you from Dispose, it's safe
        /// GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern. 
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_subscription != null)
                {
                    _subscription.Dispose();
                }
            }
            // Free any unmanaged objects here. 
            _disposed = true;
        }
    }
}
