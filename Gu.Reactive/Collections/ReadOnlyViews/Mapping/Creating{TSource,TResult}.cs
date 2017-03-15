namespace Gu.Reactive
{
    using System;

    internal sealed class Creating<TSource, TResult> : Updating<TSource, TResult>
    {
        internal Creating(Func<TSource, TResult> selector)
            : base((o, _) => selector(o))
        {
        }

        public override bool CanUpdateIndex => false;
    }
}