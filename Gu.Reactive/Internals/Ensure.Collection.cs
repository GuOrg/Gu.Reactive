namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal static partial class Ensure
    {
        internal static void NotNullOrEmpty<T>(IReadOnlyCollection<T> value, string parameterName)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), $"{nameof(parameterName)} cannot be null");
            Ensure.NotNull(value, parameterName);

            if (value.Count == 0)
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }
}
