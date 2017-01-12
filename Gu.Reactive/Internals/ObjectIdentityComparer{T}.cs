#pragma warning disable SA1600 // Elements must be documented, internal
namespace Gu.Reactive.Internals
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal sealed class ObjectIdentityComparer<T> : IEqualityComparer<T>
    {
        internal static readonly ObjectIdentityComparer<T> Default = new ObjectIdentityComparer<T>();

        private ObjectIdentityComparer()
        {
        }

        bool IEqualityComparer<T>.Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        int IEqualityComparer<T>.GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}