namespace Gu.Reactive.Internals
{
    using System;

    internal static class LazyExt
    {
        // ReSharper disable once UnusedParameter.Global
        internal static void ForceCreate<T>(this Lazy<T> lazy)
        {
            if (lazy.Value == null)
            {
                throw new InvalidOperationException();
            }
        }
    }
}