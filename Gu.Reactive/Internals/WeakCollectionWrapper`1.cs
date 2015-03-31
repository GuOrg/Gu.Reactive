namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal sealed class WeakCollectionWrapper<T> : IEnumerable<T>, IDisposable where T : class
    {
        private static readonly ObjectIdentityComparer Comparer = new ObjectIdentityComparer();
        private readonly WeakReference _wr = new WeakReference(null);
        private readonly ConcurrentDictionary<T, IDisposable> _map = new ConcurrentDictionary<T, IDisposable>(Comparer);
        private readonly object _lock = new object();
        private bool _disposed;

        public WeakCollectionWrapper(ObservableCollection<T> collection)
        {
            _wr.Target = collection;
        }

        public IEnumerable<T> Collection
        {
            get
            {
                VerifyDisposed();
                return (IEnumerable<T>)_wr.Target;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            VerifyDisposed();
            var source = (ObservableCollection<T>)_wr.Target;
            if (source == null)
            {
                return Enumerable.Empty<T>()
                                 .GetEnumerator();
            }
            return source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            VerifyDisposed();
            return GetEnumerator();
        }

        /// <summary>
        /// Make the class sealed when using this. 
        /// Call VerifyDisposed at the start of all public methods
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            foreach (var disposable in _map.Values)
            {
                disposable.Dispose();
            }
            _map.Clear();
            _wr.Target = null;
        }

        public void Update(
            NotifyCollectionChangedEventArgs e,
            Func<T, IDisposable> subscribe)
        {
            VerifyDisposed();
            lock (_lock)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        AddRange(e.NewItems.Cast<T>(), subscribe);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        RemoveRange(e.OldItems.Cast<T>());
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        AddRange(e.NewItems.Cast<T>(), subscribe);
                        RemoveRange(e.OldItems.Cast<T>());
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        Reset(Collection, subscribe);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void Reset(IEnumerable<T> collection, Func<T, IDisposable> subscribe)
        {
            if (collection == null)
            {
                foreach (var disposable in _map.Values)
                {
                    disposable.Dispose();
                }
                _map.Clear();
                return;
            }
            var old = _map.Keys.Except(collection, Comparer);
            RemoveRange(old);
            var newItems = collection.Except(_map.Keys, Comparer);
            AddRange(newItems, subscribe);
        }

        private void RemoveRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                if (item == null)
                {
                    continue;
                }
                IDisposable subscription;
                if (_map.TryRemove(item, out subscription))
                {
                    subscription.Dispose();
                }
            }
        }

        private void AddRange(IEnumerable<T> items, Func<T, IDisposable> subscribe)
        {
            foreach (var item in items)
            {
                if (item == null)
                {
                    continue;
                }
                _map.GetOrAdd(item, subscribe);
            }
        }

        private void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(
                    GetType()
                        .FullName);
            }
        }

        private class ObjectIdentityComparer : IEqualityComparer<T>
        {
            public bool Equals(T x, T y)
            {
                return ReferenceEquals(x, y);
            }
            public int GetHashCode(T obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }
        }
    }
}