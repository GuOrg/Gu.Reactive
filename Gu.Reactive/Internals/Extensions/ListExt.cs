namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal static class ListExt
    {
        internal static bool Contains<T>(this IEnumerable<T> source, object value)
        {
            return source.Any(item => Equals(value, item));
        }

        internal static int IndexOf<T>(this IReadOnlyList<T> source, object value)
        {
            for (var i = 0; i < source.Count; i++)
            {
                if (Equals(value, source[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        internal static void CopyTo<T>(this IReadOnlyList<T> source, Array array, int index)
        {
            Ensure.GreaterThanOrEqual(index, 0, nameof(index));
            Ensure.LessThan(index, array.Length, nameof(index));

            for (var i = 0; i < source.Count; i++)
            {
                array.SetValue(source[i], i + index);
            }
        }

        internal static object SyncRootOrDefault(this IEnumerable source, object @default)
        {
            return (source as ICollection)?.SyncRoot ?? @default;
        }

        internal static object SyncRoot<T>(this T source)
            where T : ICollection
        {
            return source.SyncRoot;
        }
    }
}
