namespace Gu.Reactive.Internals
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal class ObjectIdentityComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}