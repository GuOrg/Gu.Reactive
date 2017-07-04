namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections.Generic;

    internal static class SetExt
    {
        /// <summary>
        /// This is not nice but can't think of another way as we don't own <paramref name="other"/>
        /// </summary>
        internal static void UnionWithWithRetries<T>(this HashSet<T> set, IEnumerable<T> other)
        {
            var retry = 0;
            while (true)
            {
                try
                {
                    set.UnionWith(other);
                    break;
                }
                catch (InvalidOperationException e) when (e.Message == Exceptions.CollectionWasModified.Message &&
                                                          retry < 5)
                {
                    retry++;
                }
            }
        }
    }
}