namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections.Generic;

    internal static class Exceptions
    {
        private static InvalidOperationException collectionWasModified;

        public static InvalidOperationException CollectionWasModified => collectionWasModified ?? (collectionWasModified = Create());

        private static InvalidOperationException Create()
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            var ints = new List<int>(1);
            try
            {
                using (var enumerator = ints.GetEnumerator())
                {
                    // this increments version of the list.
                    ints.Clear();

                    // this throws collection was modified.
                    enumerator.MoveNext();
                }
            }
            catch (InvalidOperationException e)
            {
                return e;
            }

            throw new NotImplementedException("Should never get here.");
        }
    }
}