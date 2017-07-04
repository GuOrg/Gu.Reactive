namespace Gu.Reactive.Internals
{
    using System.Collections.Generic;

    internal class IdentitySet<T> : HashSet<T>
        where T : class
    {
        public IdentitySet()
            : base(ObjectIdentityComparer<T>.Default)
        {
        }
    }
}