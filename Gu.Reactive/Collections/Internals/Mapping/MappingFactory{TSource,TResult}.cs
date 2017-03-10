namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Disposables;

    internal class MappingFactory<TSource, TResult> : IMappingFactory<TSource, TResult>
    {
        private readonly Func<TSource, int, TResult> indexSelector;
        private readonly Func<TSource, TResult> selector;
        private readonly CompositeDisposable itemDisposables = new CompositeDisposable();
        private bool disposed;

        internal MappingFactory(Func<TSource, TResult> selector)
        {
            this.selector = selector;
            this.indexSelector = null;
        }

        internal MappingFactory(Func<TSource, int, TResult> indexSelector)
        {
            this.selector = null;
            this.indexSelector = indexSelector;
        }

        public bool CanUpdateIndex => this.indexSelector != null;

        public TResult GetOrCreateValue(TSource key, int index)
        {
            this.ThrowIfDisposed();
            var mapped = this.indexSelector != null
                                 ? this.indexSelector(key, index)
                                 : this.selector(key);

            var disposable = mapped as IDisposable;
            if (disposable != null)
            {
                this.itemDisposables.Add(disposable);
            }

            return mapped;
        }

        public TResult UpdateIndex(TSource key, TResult oldResult, int index)
        {
            return this.GetOrCreateValue(key, index);
        }

        /// <inheritdoc/>
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

        public void Refresh(IEnumerable<TSource> source, IReadOnlyList<TResult> mapped)
        {
        }
    }
}
