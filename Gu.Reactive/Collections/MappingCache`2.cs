namespace Gu.Reactive
{
    using System;
    using System.Runtime.CompilerServices;

    internal sealed class MappingCache<TSource, TResult> : IMappingFactory<TSource, TResult>
    {
        private readonly Func<TSource, int, TResult> _indexSelector;
        private readonly Func<TResult, int, TResult> _indexUpdater;
        private readonly Func<TSource, TResult> _selector;
        private readonly ConditionalWeakTable<object, object> _cache;
        private readonly WeakCompositeDisposable _itemDisposables = new WeakCompositeDisposable();
        private bool _disposed;

        public MappingCache(Func<TSource, TResult> selector)
        {
            _selector = selector;
            _cache = new ConditionalWeakTable<object, object>();
        }

        public MappingCache(Func<TSource, int, TResult> indexSelector, Func<TResult, int, TResult> indexUpdater)
        {
            _indexSelector = indexSelector;
            _indexUpdater = indexUpdater;
            _cache = new ConditionalWeakTable<object, object>();
        }

        public bool CanUpdateIndex => _indexSelector != null && _indexUpdater != null;

        public TResult GetOrCreateValue(TSource key, int index)
        {
            VerifyDisposed();
            object mapped;
            if (_cache.TryGetValue(key, out mapped))
            {
                if (CanUpdateIndex)
                {
                    return UpdateIndex(key, index);
                }

                return (TResult)mapped;
            }

            if (_indexSelector != null)
            {
                mapped = _indexSelector(key, index);
            }
            else
            {
                mapped = _selector(key);
            }

            _cache.Add(key, mapped);
            var disposable = mapped as IDisposable;
            if (disposable != null)
            {
                _itemDisposables.Add(disposable);
            }

            return (TResult)mapped;
        }

        public TResult UpdateIndex(TSource key, int index)
        {
            if (_indexUpdater == null)
            {
                return default(TResult);
            }

            object mapped;
            if (_cache.TryGetValue(key, out mapped))
            {
                var updated = _indexUpdater((TResult)mapped, index);
                if (!ReferenceEquals(mapped, updated))
                {
                    _cache.Remove(key);
                    _cache.Add(key, updated);
                }

                return updated;
            }

            return default(TResult);
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