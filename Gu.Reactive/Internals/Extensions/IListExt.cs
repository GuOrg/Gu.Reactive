namespace Gu.Reactive
{
    using System;
    using System.Collections;

    // ReSharper disable once InconsistentNaming
    internal static class IListExt
    {
        internal static bool TrySingle(this IList source, out object item)
        {
            if (source.Count == 1)
            {
                item = source[0];
                return true;
            }

            item = null;
            return false;
        }

        internal static bool All(this IList source, Func<object, bool> predicate)
        {
            foreach (var item in source)
            {
                if (!predicate(item))
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool Any(this IList source, Func<object, bool> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
