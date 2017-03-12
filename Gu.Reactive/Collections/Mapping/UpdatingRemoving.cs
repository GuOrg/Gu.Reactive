//namespace Gu.Reactive
//{
//    using System;

//    using Gu.Reactive.Internals;

//    internal class UpdatingRemoving<TSource, TResult> : Updating<TSource, TResult>
//        where TSource : class
//        where TResult : class
//    {
//        private readonly Action<TResult> onRemove;
//        private readonly Cache<TSource, TResult> cache = new Cache<TSource, TResult>();

//        private bool disposed;

//        internal UpdatingRemoving(Func<TSource, int, TResult> selector, Func<TResult, int, TResult> updater, Action<TResult> onRemove)
//            : base(selector, updater)
//        {
//            this.onRemove = onRemove;
//            this.cache.OnRemove += onRemove;
//        }

//        public override TResult GetOrCreate(TSource key, int index)
//        {
//            var result = base.GetOrCreate(key, index);
//            return this.cache.GetOrCreate(key, k => );
//        }

//        public override TResult Update(TSource key, TResult oldResult, int index)
//        {
//            var updated = base.Update(key, oldResult, index);
//            if (ReferenceEquals(oldResult, updated))
//            {
//                return updated;
//            }

//            if (oldResult != null)
//            {
//                throw new NotImplementedException("We don't know that this was the last instance.");
//                this.onRemove(oldResult);
//                this.items.Remove(oldResult);
//            }

//            return this.items.AddAndReturn(updated);
//        }

//        /// <inheritdoc/>
//        protected override void Dispose(bool disposing)
//        {
//            if (this.disposed)
//            {
//                return;
//            }

//            this.disposed = true;
//            if (disposing)
//            {
//                this.cache.Clear();
//                this.cache.OnRemove -= this.onRemove;
//            }
//        }
//    }
//}