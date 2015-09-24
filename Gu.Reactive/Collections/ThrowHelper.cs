namespace Gu.Reactive
{
    using System;

    internal static class ThrowHelper
    {
        private const string CollectionIsReadonly = "Collection is readonly";

        internal static void ThrowCollectionIsReadonly()
        {
            throw new InvalidOperationException(CollectionIsReadonly);
        }

        internal static TResult ThrowCollectionIsReadonly<TResult>()
        {
            throw new InvalidOperationException(CollectionIsReadonly);
        }

        internal static void ThrowNotSupportedException()
        {
            throw new NotSupportedException();
        }

        internal static TResult ThrowNotSupportedException<TResult>()
        {
            throw new NotSupportedException();
        }
    }
}
