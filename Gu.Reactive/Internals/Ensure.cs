namespace Gu.Reactive.Internals
{
    using System;
    using System.Runtime.CompilerServices;

    internal static class Ensure
    {
        internal static void NotNull(object o, string paramName, [CallerMemberName] string caller = null)
        {
            NotNullOrEmpty(paramName, "paramName");
            if (o == null)
            {
                var message = string.Format("Expected parameter {0} in member {1} to not be null", paramName, caller);
                throw new ArgumentNullException(paramName, message);
            }
        }

        internal static void NotNull(object o, string paramName, string message, [CallerMemberName] string caller = null)
        {
            NotNullOrEmpty(paramName, "paramName");
            if (o == null)
            {
                if (message == null)
                {
                    throw new ArgumentNullException(paramName);
                }
                throw new ArgumentNullException(paramName, message);
            }
        }

        internal static void NotNullOrEmpty(string s, string paramName, string message = null)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (message == null)
                {
                    throw new ArgumentNullException(paramName);
                }
                throw new ArgumentNullException(paramName, message);
            }
        }

        public static void NotEqual<T>(T value, T other, string parameter)
        {
            if (Equals(value, other))
            {
                var message = string.Format("Expected {0} to not equal {1}", value, other);
                throw new ArgumentException(message, parameter);
            }
        }
    }
}
