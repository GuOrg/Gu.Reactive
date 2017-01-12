// ReSharper disable UnusedParameter.Global
#pragma warning disable SA1600 // Elements must be documented, internal
namespace Gu.Reactive.Internals
{
    using System;
    using System.Diagnostics;
    using System.Text.RegularExpressions;

    internal static partial class Ensure
    {
        internal static void NotNullOrEmpty(string value, string parameterName)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), $"{nameof(parameterName)} cannot be null");
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        public static void IsMatch(string text, string pattern, string parameterName)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), $"{nameof(parameterName)} cannot be null");
            if (!Regex.IsMatch(text, pattern))
            {
                throw new ArgumentException(parameterName);
            }
        }
    }
}
