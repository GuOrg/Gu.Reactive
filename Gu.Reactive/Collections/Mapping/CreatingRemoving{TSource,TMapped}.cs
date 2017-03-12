namespace Gu.Reactive
{
    using System;

    internal class CreatingRemoving<TSource, TResult> : CreatingCaching<TSource, TResult>
        where TSource : class
        where TResult : class
    {
        private readonly Action<TResult> onRemove;

        internal CreatingRemoving(Func<TSource, TResult> selector, Action<TResult> onRemove)
            : base(selector)
        {
            this.Cache.OnRemove += onRemove;
            this.onRemove = onRemove;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this.Cache.OnRemove -= this.onRemove;
            }
        }
    }
}