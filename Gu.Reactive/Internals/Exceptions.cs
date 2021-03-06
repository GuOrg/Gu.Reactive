﻿namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections.Generic;

    internal static class Exceptions
    {
        private static InvalidOperationException? collectionWasModified;
        private static ArgumentException? destinationNotLongEnough;

        internal static InvalidOperationException CollectionWasModified => collectionWasModified ??= CreateCollectionWasModified();

        internal static ArgumentException DestinationNotLongEnough => destinationNotLongEnough ??= CreateDestinationNotLongEnough();

        private static InvalidOperationException CreateCollectionWasModified()
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            var ints = new List<int>(1);
            try
            {
                using var enumerator = ints.GetEnumerator();

                // this increments version of the list.
                ints.Clear();

                // this throws collection was modified.
                _ = enumerator.MoveNext();
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
