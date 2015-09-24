namespace Gu.Reactive
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    internal static class ListExt
    {
        internal static IReadOnlyList<T> AsReadOnly<T>(this IList<T> source)
        {
            return source as IReadOnlyList<T> ?? new ReadOnlyCollection<T>(source);
        }
    }
}