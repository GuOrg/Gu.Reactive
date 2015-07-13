namespace Gu.Reactive
{
    using System;

    internal interface IMappingFactory<in TSource, out TResult> : IDisposable
    {
        bool CanUpdateIndex { get; }

        TResult GetOrCreateValue(TSource key, int index);

        TResult UpdateIndex(TSource key, int index);
    }
}