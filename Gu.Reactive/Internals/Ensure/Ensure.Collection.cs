namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal static partial class Ensure
    {
        internal static void NotNullOrEmpty<T>(IReadOnlyCollection<T> collection, string parameterName)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), $"{nameof(parameterName)} cannot be null");
            Ensure.NotNull(collection, parameterName);

            if (collection.Count == 0)
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }
}
