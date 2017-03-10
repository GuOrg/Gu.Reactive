namespace Gu.Reactive
{
    using System;
    using System.Reactive.Disposables;
    using System.Runtime.CompilerServices;

    internal sealed class MappingCache<TSource, TResult> : IMappingFactory<TSource, TResult>
    {
        private readonly Func<TSource, int, TResult> indexSelector;
        private readonly Func<TResult, int, TResult> indexUpdater;
        private readonly Func<TSource, TResult> selector;
        private readonly ConditionalWeakTable<object, object> cache;
        private readonly CompositeDisposable itemDisposables = new CompositeDisposable();
        private bool disposed;

        public MappingCache(Func<TSource, TResult> selector)
        {
            this.selector = selector;
            this.indexSelector = null;
            this.indexUpdater = null;
            this.cache = new ConditionalWeakTable<object, object>();
        }

        public MappingCache(Func<TSource, int, TResult> indexSelector, Func<TResult, int, TResult> indexUpdater)
        {
            this.selector = null;
            this.indexSelector = indexSelector;
            this.indexUpdater = indexUpdater;
            this.cache = new ConditionalWeakTable<object, object>();
        }

        public bool CanUpdateIndex => this.indexSelector != null && this.indexUpdater != null;

        public TResult GetOrCreateValue(TSource key, int index)
        {
            this.ThrowIfDisposed();
            object mapped;
            if (this.cache.TryGetValue(key, out mapped))
            {
                if (this.CanUpdateIndex)
                {
                    return this.UpdateIndex(key, index);
                }

                return (TResult)mapped;
            }

            mapped = this.indexSelector != null
                         ? this.indexSelector(key, index)
                         : this.selector(key);

            this.cache.Add(key, mapped);
            var disposable = mapped as IDisposable;
            if (disposable != null)
            {
                this.itemDisposables.Add(disposable);
            }

            return (TResult)mapped;
        }

        public TResult UpdateIndex(TSource key, int index)
        {
            if (this.indexUpdater == null)
            {
                return default(TResult);
            }

            object mapped;
            if (this.cache.TryGetValue(key, out mapped))
            {
                var updated = this.indexUpdater((TResult)mapped, index);
                if (!ReferenceEquals(mapped, updated))
                {
                    this.cache.Remove(key);
                    this.cache.Add(key, updated);
                }

                return updated;
            }

            return default(TResult);
        }

        /// <summary>
        /// Make the class sealed when using this.
        /// Call ThrowIfDisposed at the start of all public methods
        /// </summary>
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.itemDisposables.Dispose();
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}