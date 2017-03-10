namespace Gu.Reactive
{
    using System;

    internal class SimpleFactory<TSource, TResult> : SimpleUpdatingFactory<TSource, TResult>
    {
        internal SimpleFactory(Func<TSource, TResult> selector)
            : base((o, _) => selector(o))
        {
        }

        public override bool CanUpdateIndex => true;
    }
}