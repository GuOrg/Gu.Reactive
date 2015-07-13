namespace Gu.Reactive
{
    using System;

    internal class MappingFactory<TSource, TResult> : IMappingFactory<TSource, TResult>
    {
        private readonly Func<TSource, TResult> _selector;

        private readonly WeakCompositeDisposable _itemDisposables = new WeakCompositeDisposable();

        private bool _disposed;

        public MappingFactory(Func<TSource, TResult> selector)
        {
            _selector = selector;
        }

        public TResult GetOrCreateValue(TSource key)
        {
            VerifyDisposed();
            var mapped = _selector(key);
            var disposable = mapped as IDisposable;
            if (disposable != null)
            {
                _itemDisposables.Add(disposable);
            }
            return mapped;
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
