namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using Gu.Reactive.Internals;

    internal class UpdatingRemoving<TSource, TResult> : Updating<TSource, TResult>
        where TSource : class
        where TResult : class
    {
        private readonly Action<TResult> onRemove;
        private readonly SetPool.IdentitySet<TResult> items = SetPool.Borrow<TResult>();

        private bool disposed;

        internal UpdatingRemoving(Func<TSource, int, TResult> selector, Func<TResult, int, TResult> updater, Action<TResult> onRemove)
            : base(selector, updater)
        {
            this.onRemove = onRemove;
        }

        public override TResult GetOrCreateValue(TSource key, int index)
        {
            return this.items.AddAndReturn(base.GetOrCreateValue(key, index));
        }

        public override TResult UpdateIndex(TSource key, TResult oldResult, int index)
        {
            var updated = base.UpdateIndex(key, oldResult, index);
            if (ReferenceEquals(oldResult, updated))
            {
                return updated;
            }

            if (oldResult != null)
            {
                this.onRemove(oldResult);
                this.items.Remove(oldResult);
            }

            return this.items.AddAndReturn(updated);
        }

        /// <inheritdoc/>
        public override void Refresh(IEnumerable<TSource> source, IReadOnlyList<TResult> mapped, NotifyCollectionChangedEventArgs e)
        {
            this.ThrowIfDisposed();
            var set = SetPool.Borrow<TResult>();
            set.UnionWith(this.items);
            set.ExceptWith(mapped);
            foreach (var item in set)
            {
                this.onRemove(item);
            }

            SetPool.Return(set);
            base.Refresh(source, mapped, e);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
                foreach (var item in this.items)
                {
                    this.onRemove(item);
                }

                this.items.Clear();
                SetPool.Return(this.items);
            }
        }
    }
}