namespace Gu.Reactive
{
    using System;

    internal class CreatingCaching<TSource, TResult> : CreatingRemoving<TSource, TResult>
        where TSource : class
        where TResult : class
    {
        public CreatingCaching(Func<TSource, TResult> selector)
            : base(selector, result => { })
        {
        }
    }
}