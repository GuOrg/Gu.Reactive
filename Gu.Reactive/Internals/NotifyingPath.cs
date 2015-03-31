namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;

    internal sealed class NotifyingPath : IReadOnlyList<INotifyingPathItem>, IDisposable
    {
        private readonly IReadOnlyList<INotifyingPathItem> _parts;

        private INotifyPropertyChanged _source;

        private bool _disposed;

        private NotifyingPath(IReadOnlyList<INotifyingPathItem> parts)
        {
            _parts = parts;
        }

        public int Count
        {
            get { return _parts.Count; }
        }

        public INotifyPropertyChanged Source
        {
            get
            {
                VerifyDisposed();
                return (INotifyPropertyChanged)((RootItem)_parts[0]).Value;
            }
            set
            {
                VerifyDisposed();
                ((RootItem)_parts[0]).Value = value;
                ((NotifyingPathItem)_parts[1]).Source = value;
            }
        }

        public object LastSource
        {
            get
            {
                return ((NotifyingPathItem)_parts.Last()).Previous.Value;
            }
        }

        public INotifyingPathItem this[int index]
        {
            get
            {
                VerifyDisposed();
                return _parts[index];
            }
        }

        public static NotifyingPath Create<TSource, TValue>(Expression<Func<TSource, TValue>> propertyExpression)
        {
            var path = ValuePath.Create(propertyExpression);
            var items = new INotifyingPathItem[path.Count];
            items[0] = (RootItem)path[0];
            INotifyingPathItem previous = items[0];
            for (int i = 1; i < path.Count; i++)
            {
                var item = new NotifyingPathItem(previous, (PathItem)path[i]);
                items[i] = item;
                previous = item;
            }
            return new NotifyingPath(items);
        }

        public IEnumerator<INotifyingPathItem> GetEnumerator()
        {
            VerifyDisposed();
            return _parts.GetEnumerator();
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
            _disposed = true;
            // Dispose some stuff now
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
    }
}