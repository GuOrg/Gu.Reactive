namespace Gu.Reactive
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Gu.Reactive.Internals;

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