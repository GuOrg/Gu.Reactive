#pragma warning disable SA1600 // Elements must be documented, internal
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