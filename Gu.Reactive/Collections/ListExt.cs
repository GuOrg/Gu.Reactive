namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Gu.Reactive.Internals;

    internal static class ListExt
    {
        internal static IReadOnlyList<T> AsReadOnly<T>(this IEnumerable<T> source)
        {
            var readOnlyList = source as IReadOnlyList<T>;
            if (readOnlyList != null)
            {
                return readOnlyList;
            }

            var list = source as IList<T>;
            if (list != null)
            {
                return new ReadOnlyCollection<T>(list);
            }

            return source.ToArray();
        }

        internal static bool Contains<T>(this IEnumerable<T> source, object value)
        {
            return source.Any(item => ReferenceEquals(value, item));
        }

        internal static int IndexOf<T>(this IReadOnlyList<T> source, object value)
        {
            for (int i = 0; i < source.Count; i++)
            {
                if (ReferenceEquals(value, source[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        internal static int IndexOf<T>(this IList<T> source, object value)
        {
            for (int i = 0; i < source.Count; i++)
            {
                if (ReferenceEquals(value, source[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        internal static void CopyTo<T>(this IReadOnlyList<T> source, Array array, int index)
        {
            Ensure.NotNull(array, nameof(array));
            Ensure.GreaterThanOrEqual(index, 0, nameof(index));
            Ensure.LessThan(index, array.Length, nameof(index));

            for (int i = 0; i < source.Count; i++)
            {
                array.SetValue(source[i], i + index);
            }
        }

        internal static void CopyTo<T>(this IList<T> source, Array array, int index)
        {
            Ensure.NotNull(array, nameof(array));
            Ensure.GreaterThanOrEqual(index, 0, nameof(index));
            Ensure.LessThan(index, array.Length, nameof(index));

            for (int i = 0; i < source.Count; i++)
            {
                array.SetValue(source[i], i + index);
            }
        }

        internal static object SyncRootOrDefault(this IEnumerable source, object @default)
        {
            return (source as ICollection)?.SyncRoot ?? @default;
        }

        internal static object SyncRoot<T>(this T source) where T : ICollection
        {
            return source.SyncRoot;
        }
    }
}
