//namespace Gu.Reactive
//{
//    using System;

//    internal class CreatingRemoving<TSource, TResult> : IMapper<TSource, TResult>
//        where TSource : class 
//        where TResult : class
//    {
//        private readonly Func<TSource, TResult> selector;
//        private readonly Action<TResult> onRemove;
//        private readonly Cache<TSource, TResult> cache = new Cache<TSource, TResult>();

//        private bool disposed;

//        internal CreatingRemoving(Func<TSource, TResult> selector, Action<TResult> onRemove)
//        {
//            this.selector = selector;
//            this.onRemove = onRemove;
//        }

//        public bool CanUpdateIndex => false;

//        public TResult GetOrCreate(TSource key, int index)
//        {
//            this.ThrowIfDisposed();
//            return this.cache.GetOrCreate(key, this.selector);
//        }

//        /// <inheritdoc />
//        TResult IMapper<TSource, TResult>.Update(TSource key, TResult oldResult, int index) => oldResult;

//        /// <inheritdoc />
//        public void Dispose()
//        {
//            this.Dispose(true);
//        }

//        protected virtual void Dispose(bool disposing)
//        {
//            if (this.disposed)
//            {
//                return;
//            }

//            this.disposed = true;
//            if (disposing)
//            {
//                this.cache.Clear();
//            }
//        }

//        protected void ThrowIfDisposed()
//        {
//            if (this.disposed)
//            {
//                throw new ObjectDisposedException(this.GetType().FullName);
//            }
//        }
//    }
//}