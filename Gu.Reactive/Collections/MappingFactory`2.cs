namespace Gu.Reactive
{
    using System;

    internal class MappingFactory<TSource, TResult> : IMappingFactory<TSource, TResult>
    {
        private readonly Func<TSource, int, TResult> _indexSelector;
        private readonly Func<TSource, TResult> _selector;
        private readonly WeakCompositeDisposable _itemDisposables = new WeakCompositeDisposable();
        private bool _disposed;

        internal MappingFactory(Func<TSource, TResult> selector)
        {
            _selector = selector;
        }

        internal MappingFactory(Func<TSource, int, TResult> indexSelector)
        {
            _indexSelector = indexSelector;
        }

        public bool CanUpdateIndex => _indexSelector != null;

        public TResult GetOrCreateValue(TSource key, int index)
        {
            VerifyDisposed();
            TResult mapped;
            if (_indexSelector != null)
            {
                mapped = _indexSelector(key, index);
            }
            else
            {
                mapped = _selector(key);
            }

            var disposable = mapped as IDisposable;
            if (disposable != null)
            {
                _itemDisposables.Add(disposable);
            }

            return mapped;
        }

        public TResult UpdateIndex(TSource key, int index)
        {
            return GetOrCreateValue(key, index);
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
