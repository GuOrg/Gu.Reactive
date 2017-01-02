namespace Gu.Reactive
{
    using System;

    internal class MappingFactory<TSource, TResult> : IMappingFactory<TSource, TResult>
    {
        private readonly Func<TSource, int, TResult> indexSelector;
        private readonly Func<TSource, TResult> selector;
        private readonly WeakCompositeDisposable itemDisposables = new WeakCompositeDisposable();
        private bool disposed;

        internal MappingFactory(Func<TSource, TResult> selector)
        {
            this.selector = selector;
        }

        internal MappingFactory(Func<TSource, int, TResult> indexSelector)
        {
            this.indexSelector = indexSelector;
        }

        public bool CanUpdateIndex => this.indexSelector != null;

        public TResult GetOrCreateValue(TSource key, int index)
        {
            this.VerifyDisposed();
            TResult mapped;
            if (this.indexSelector != null)
            {
                mapped = this.indexSelector(key, index);
            }
            else
            {
                mapped = this.selector(key);
            }

            var disposable = mapped as IDisposable;
            if (disposable != null)
            {
                this.itemDisposables.Add(disposable);
            }

            return mapped;
        }

        public TResult UpdateIndex(TSource key, int index)
        {
            return this.GetOrCreateValue(key, index);
        }

        /// <summary>
        /// Make the class sealed when using this.
        /// Call VerifyDisposed at the start of all public methods
        /// </summary>
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            // Dispose some stuff now
        }

        private void VerifyDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(
                    this.GetType()
                        .FullName);
            }
        }
    }
}
