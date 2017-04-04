namespace Gu.Reactive
{
    using System.Collections;
    using System.Collections.Generic;
    using Gu.Reactive.Internals;

    internal class InstanceMap<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
        where TKey : class
    {
        private readonly Dictionary<TKey, TValue> inner = new Dictionary<TKey, TValue>(ObjectIdentityComparer<TKey>.Default);

        internal object Gate => this.inner;

        internal IEnumerable<TKey> Keys => this.inner.Keys;

        internal TValue this[TKey key]
        {
            get { return this.inner[key]; }
            set { this.inner[key] = value; }
        }

        internal void Add(TKey key, TValue value)
        {
            this.inner.Add(key, value);
        }

        internal void Remove(TKey key)
        {
            this.inner.Remove(key);
        }

        internal bool TryGetValue(TKey key, out TValue result)
        {
            return this.inner.TryGetValue(key, out result);
        }

        internal void Clear()
        {
            this.inner.Clear();
        }

        internal bool ContainsKey(TKey key)
        {
            return this.inner.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => this.inner.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}