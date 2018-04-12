namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections.Generic;

    internal static class Exceptions
    {
        private static InvalidOperationException collectionWasModified;
        private static ArgumentException destinationNotLongEnough;

        public static InvalidOperationException CollectionWasModified => collectionWasModified ?? (collectionWasModified = CreateCollectionWasModified());

        public static ArgumentException DestinationNotLongEnough => destinationNotLongEnough ?? (destinationNotLongEnough = CreateDestinationNotLongEnough());

        private static InvalidOperationException CreateCollectionWasModified()
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
                    enumerator.MoveNext().IgnoreReturnValue();
                }
            }
            catch (InvalidOperationException e)
            {
                return e;
            }

            throw new InvalidOperationException("Should never get here.");
        }

        private static ArgumentException CreateDestinationNotLongEnough()
        {
            try
            {
                Array.Copy(new[] { 1, 2 }, 0, new int[1], 0, 2);
            }
            catch (ArgumentException e)
            {
                return e;
            }

            throw new InvalidOperationException("Should never get here.");
        }
    }
}
