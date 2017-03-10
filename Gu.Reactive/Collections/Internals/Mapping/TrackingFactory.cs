namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;

    internal sealed class TrackingFactory<TSource, TResult> : SimpleUpdatingFactory<TSource, TResult>
    {
        private readonly Action<TResult> onRemove;

        internal TrackingFactory(Func<TSource, int, TResult> selector, Action<TResult> onRemove)
            : base(selector)
        {
            this.onRemove = onRemove;
        }

        public override void Refresh(IEnumerable<TSource> source, IReadOnlyList<TResult> mapped)
        {
            base.Refresh(source, mapped);
        }
    }
}