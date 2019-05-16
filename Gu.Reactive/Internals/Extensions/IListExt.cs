namespace Gu.Reactive
{
    using System;
    using System.Collections;

    // ReSharper disable once InconsistentNaming
    internal static class IListExt
    {
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
