namespace Gu.Reactive
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal class InstanceMap<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
        where TKey : class
    {
        private readonly Dictionary<Key, TValue> inner = new(KeyComparer.Default);

#pragma warning disable INPC017 // Backing field name must match.
        internal object Gate => this.inner;
#pragma warning restore INPC017 // Backing field name must match.

        internal IEnumerable<TKey> Keys
        {
            get
            {
                foreach (var kvp in this.inner)
                {
                    yield return kvp.Key.Item;
                }
            }
        }

        internal TValue this[TKey key]
        {
            get => this.inner[new Key(key)];
            set => this.inner[new Key(key)] = value;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var kvp in this.inner)
            {
                yield return new KeyValuePair<TKey, TValue>(kvp.Key.Item, kvp.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        internal void Add(TKey key, TValue value)
        {
            this.inner.Add(new Key(key), value);
        }

        internal void Remove(TKey key)
        {
            this.inner.Remove(new Key(key));
        }

        internal bool TryGetValue(TKey key, out TValue? result)
        {
            return this.inner.TryGetValue(new Key(key), out result);
        }

        internal void Clear()
        {
            this.inner.Clear();
        }

        internal bool ContainsKey(TKey key)
        {
            return this.inner.ContainsKey(new Key(key));
        }

        private readonly struct Key
        {
            internal readonly TKey Item;

            internal Key(TKey item)
            {
                this.Item = item;
            }
        }

        private sealed class KeyComparer : IEqualityComparer<Key>
        {
            internal static readonly KeyComparer Default = new KeyComparer();

            private KeyComparer()
            {
            }

            public bool Equals(Key x, Key y) => ReferenceEquals(x.Item, y.Item);

            public int GetHashCode(Key obj)
            {
                if (obj.Item is { } item)
                {
                    return RuntimeHelpers.GetHashCode(item);
                }

                return -1;
            }
        }
    }
}
