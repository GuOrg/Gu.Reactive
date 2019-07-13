namespace Gu.Reactive.Internals
{
    using System.Collections.Generic;

    internal class IdentitySet<T> : HashSet<T>
        where T : class
    {
        internal IdentitySet()
            : base(ObjectIdentityComparer<T>.Default)
        {
        }
    }
}
