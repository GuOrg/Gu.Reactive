namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal static class Ensure
    {
        internal static void NotNull(object o, string parameter, [CallerMemberName] string caller = null)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameter), "parameter cannot be null");

            if (o == null)
            {
                var message = string.Format("Expected parameter {0} in member {1} to not be null", parameter, caller);
                throw new ArgumentNullException(parameter, message);
            }
        }

        internal static void NotNullOrEmpty(string s, string parameter, string message = null)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameter), "parameter cannot be null");
            if (string.IsNullOrEmpty(s))
            {
                if (message == null)
                {
                    throw new ArgumentNullException(parameter);
                }
                throw new ArgumentNullException(parameter, message);
            }
        }

        internal static void NotNullOrEmpty<T>(IEnumerable<T> value, string parameter, string message = null)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameter),"parameter cannot be null");
            Ensure.NotNull(value, parameter);
            if (!value.Any())
            {
                if (message == null)
                {
                    message = string.Format("Expected {0} to not be empty", parameter);
                }
                throw new ArgumentException(message, parameter);
            }
        }

        public static void NotEqual<T>(T value, T other, string parameter)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameter), "parameter cannot be null");

            if (Equals(value, other))
            {
                var message = string.Format("Expected {0} to not equal {1}", value, other);
                throw new ArgumentException(message, parameter);
            }
        }
    }
}
