namespace Gu.Reactive
{
    using System;
    using System.Runtime.CompilerServices;

    internal sealed class MappingCache<TSource, TResult> : IMappingFactory<TSource, TResult>
    {
        private readonly Func<TSource, TResult> _selector;
        private readonly ConditionalWeakTable<object, object> _cache = new ConditionalWeakTable<object, object>();
        private readonly WeakCompositeDisposable _itemDisposables = new WeakCompositeDisposable();
        private bool _disposed;

        public MappingCache(Func<TSource, TResult> selector)
        {
            _selector = selector;
            _cache = new ConditionalWeakTable<object, object>();
        }

        public TResult GetOrCreateValue(TSource key)
        {
            VerifyDisposed();
            object mapped;
            if (_cache.TryGetValue(key, out mapped))
            {
                return (TResult)mapped;
            }
            mapped = _selector(key);
            _cache.Add(key, mapped);
            var disposable = mapped as IDisposable;
            if (disposable != null)
            {
                _itemDisposables.Add(disposable);
            }
            return (TResult)mapped;
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
            _itemDisposables.Dispose();
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