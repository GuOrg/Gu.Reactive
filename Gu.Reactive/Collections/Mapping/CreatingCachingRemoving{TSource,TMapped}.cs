namespace Gu.Reactive
{
    using System;

    internal class CreatingCachingRemoving<TSource, TResult> : CreatingCaching<TSource, TResult>
        where TSource : class
        where TResult : class
    {
        private readonly Action<TResult> onRemove;

        internal CreatingCachingRemoving(Func<TSource, TResult> selector, Action<TResult> onRemove)
            : base(selector)
        {
            this.onRemove = onRemove;
            this.Cache.OnRemove += onRemove;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Cache.Clear();
                this.Cache.OnRemove -= this.onRemove;
            }

            base.Dispose(disposing);
        }
    }
}