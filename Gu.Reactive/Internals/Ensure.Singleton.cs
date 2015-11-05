namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections.Generic;

    internal static partial class Ensure
    {
        public static readonly HashSet<Type> Singletons = new HashSet<Type>();

        internal static void Singleton(object @this)
        {
            var type = @this.GetType();
            if (!Singletons.Add(type))
            {
                var message = $"Expected {type.Name} to be singleton";
                throw new InvalidOperationException(message);
            }
        }
    }
}
