namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal static partial class Ensure
    {
        //internal static void NotNullOrEmpty(ICollection value, string parameterName)
        //{
        //    Debug.Assert(!string.IsNullOrEmpty(parameterName), $"{nameof(parameterName)} cannot be null");
        //    Ensure.NotNull(value, parameterName);

        //    if (value.Count == 0)
        //    {
        //        throw new ArgumentNullException(parameterName);
        //    }
        //}

        internal static void NotNullOrEmpty<T>(IReadOnlyCollection<T> value, string parameterName)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), $"{nameof(parameterName)} cannot be null");
            Ensure.NotNull(value, parameterName);

            if (value.Count == 0)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        //internal static void MinCount(ICollection value, int min, string parameterName)
        //{
        //    Debug.Assert(!string.IsNullOrEmpty(parameterName), $"{nameof(parameterName)} cannot be null");
        //    Ensure.NotNull(value, parameterName);

        //    if (value.Count < min)
        //    {
        //        var message = $"Expected {nameof(value)}.{nameof(value.Count)} to be at least {min}";
        //        throw new ArgumentNullException(parameterName, message);
        //    }
        //}

        internal static void MinCount<T>(IReadOnlyCollection<T> value, int min, string parameterName)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), $"{nameof(parameterName)} cannot be null");
            Ensure.NotNull(value, parameterName);

            if (value.Count < min)
            {
                var message = $"Expected {nameof(value)}.{nameof(value.Count)} to be at least {min}";
                throw new ArgumentNullException(parameterName, message);
            }
        }

        //internal static void MaxCount(ICollection value, int max, string parameterName)
        //{
        //    Debug.Assert(!string.IsNullOrEmpty(parameterName), $"{nameof(parameterName)} cannot be null");
        //    Ensure.NotNull(value, parameterName);

        //    if (value.Count > max)
        //    {
        //        var message = $"Expected {nameof(value)}.{nameof(value.Count)} to be less than {max}";
        //        throw new ArgumentNullException(parameterName, message);
        //    }
        //}

        internal static void MaxCount<T>(IReadOnlyCollection<T> value, int max, string parameterName)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), $"{nameof(parameterName)} cannot be null");
            Ensure.NotNull(value, parameterName);

            if (value.Count > max)
            {
                var message = $"Expected {nameof(value)}.{nameof(value.Count)} to be less than {max}";
                throw new ArgumentNullException(parameterName, message);
            }
        }
    }
}
