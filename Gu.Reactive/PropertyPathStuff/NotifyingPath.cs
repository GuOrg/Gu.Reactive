namespace Gu.Reactive.PropertyPathStuff
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
        private readonly RootItem _root;
        private bool _disposed;

        internal NotifyingPath(RootItem root, IPropertyPath path)
        {
            _root = root;
            var items = new INotifyingPathItem[path.Count + 1];
            items[0] = root;
            INotifyingPathItem previous = root;
            for (int i = 0; i < path.Count; i++)
            {
                var item = new NotifyingPathItem(previous, path[i]);
                items[i + 1] = item;
                previous = item;
            }
            _parts = items;
        }

        public int Count => _parts.Count;

        public INotifyingPathItem this[int index]
        {
            get
            {
                VerifyDisposed();
                return _parts[index];
            }
        }

        internal INotifyPropertyChanged Source
        {
            get
            {
                VerifyDisposed();
                return (INotifyPropertyChanged)((RootItem)_parts[0]).Value;
            }
            set
            {
                VerifyDisposed();
                _root.Value = value;
            }
        }

        internal object LastSource => ((NotifyingPathItem)_parts.Last()).Previous.Value;

        public static NotifyingPath Create<TSource, TValue>(Expression<Func<TSource, TValue>> propertyExpression)
        {
            throw new NotImplementedException("Change to create from propertypath");

            //var path = PropertyPath.Create(propertyExpression);
            //var items = new INotifyingPathItem[path.Count];
            //items[0] = (RootItem)path[0];
            //INotifyingPathItem previous = items[0];
            //for (int i = 1; i < path.Count; i++)
            //{
            //    var item = new NotifyingPathItem(previous, (PathProperty)path[i]);
            //    items[i] = item;
            //    previous = item;
            //}
            //return new NotifyingPath(items);
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
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            foreach (var part in _parts)
            {
                part.Dispose();
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
    }
}