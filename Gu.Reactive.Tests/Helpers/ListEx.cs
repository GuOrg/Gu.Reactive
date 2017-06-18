namespace Gu.Reactive.Tests.Helpers
{
    using System.Collections.Generic;

    public static class ListEx
    {
        public static void AddRange<T>(this List<T> list, params T[] items)
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
        }
    }
}