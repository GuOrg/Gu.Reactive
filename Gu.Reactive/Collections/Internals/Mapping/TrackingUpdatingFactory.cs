namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;

    internal class TrackingUpdatingFactory<TSource, TResult> : SimpleUpdatingFactory<TSource, TResult>
    {
        private readonly Action<TResult> onRemove;

        internal TrackingUpdatingFactory(Func<TSource, int, TResult> selector, Func<TResult, int, TResult> updater, Action<TResult> onRemove)
            : base(selector, updater)
        {
            this.onRemove = onRemove;
        }

        public override TResult GetOrCreateValue(TSource key, int index)
        {
            return base.GetOrCreateValue(key, index);
        }

        public override TResult UpdateIndex(TSource key, TResult oldResult, int index)
        {
            var updated = base.UpdateIndex(key, oldResult, index);
            if (!ReferenceEquals(oldResult, updated))
            {
                this.onRemove(oldResult);
            }

            return updated;
        }

        public override void Refresh(IEnumerable<TSource> source, IReadOnlyList<TResult> mapped)
        {
            base.Refresh(source, mapped);
        }
    }
}