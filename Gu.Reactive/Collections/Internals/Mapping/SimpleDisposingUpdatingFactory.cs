namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;

    internal class SimpleDisposingUpdatingFactory<TSource, TResult> : SimpleUpdatingFactory<TSource, TResult>
    {
        protected SimpleDisposingUpdatingFactory(Func<TSource, int, TResult> selector)
            : base(selector)
        {
        }

        internal SimpleDisposingUpdatingFactory(Func<TSource, int, TResult> selector, Func<TResult, int, TResult> updater)
            : base(selector, updater)
        {
        }

        public override TResult UpdateIndex(TSource key, TResult oldResult, int index)
        {
            var updated = base.UpdateIndex(key, oldResult, index);
            if (!ReferenceEquals(oldResult, updated))
            {
                (oldResult as IDisposable)?.Dispose();
            }

            return updated;
        }

        public override void Refresh(IEnumerable<TSource> source, IReadOnlyList<TResult> mapped)
        {
            base.Refresh(source, mapped);
        }
    }
}