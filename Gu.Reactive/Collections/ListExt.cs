namespace Gu.Reactive
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

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
    }
}