namespace Gu.Reactive
{
    using System;

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
    }
}