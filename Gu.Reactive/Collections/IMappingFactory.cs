namespace Gu.Reactive
{
    using System;

    internal interface IMappingFactory<in TSource, out TResult> : IDisposable
    {
        TResult GetOrCreateValue(TSource key);
    }
}