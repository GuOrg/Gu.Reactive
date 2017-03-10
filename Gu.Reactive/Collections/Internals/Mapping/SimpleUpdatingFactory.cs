namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;

    using Gu.Reactive.Internals;

    internal class SimpleUpdatingFactory<TSource, TResult> : IMappingFactory<TSource, TResult>
    {
        private readonly Func<TSource, int, TResult> selector;
        private readonly Func<TResult, int, TResult> updater;

        internal SimpleUpdatingFactory(Func<TSource, int, TResult> selector, Func<TResult, int, TResult> updater)
        {
            Ensure.NotNull(selector, nameof(selector));

            this.selector = selector;
            this.updater = updater ?? Id;
        }

        protected SimpleUpdatingFactory(Func<TSource, int, TResult> selector)
            : this(selector, null)
        {
        }

        public virtual bool CanUpdateIndex => true;

        void IDisposable.Dispose()
        {
        }

        public TResult GetOrCreateValue(TSource key, int index) => this.selector(key, index);

        public virtual TResult UpdateIndex(TSource key, TResult oldResult, int index) => this.updater(oldResult, index);

        private static TResult Id(TResult olResult, int index) => olResult;

        public virtual void Refresh(IEnumerable<TSource> source, IReadOnlyList<TResult> mapped)
        {
            // nop
        }
    }
}