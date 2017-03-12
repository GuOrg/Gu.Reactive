namespace Gu.Reactive.Internals
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal sealed class ObjectIdentityComparer<T> : IEqualityComparer<T>
        where T : class
    {
        internal static readonly ObjectIdentityComparer<T> Default = new ObjectIdentityComparer<T>();

        private ObjectIdentityComparer()
        {
        }

        bool IEqualityComparer<T>.Equals(T x, T y) => ReferenceEquals(x, y);

        int IEqualityComparer<T>.GetHashCode(T obj) => RuntimeHelpers.GetHashCode(obj);
    }
}