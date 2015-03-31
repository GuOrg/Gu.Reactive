namespace Gu.Reactive.Internals
{
    using System.Collections;
    using System.Collections.Generic;

    internal class ValuePath<T> : IReadOnlyList<T> where T : PathItem
    {
        /// <summary>
        /// The _path.
        /// </summary>
        private readonly IReadOnlyList<T> _parts;

        public ValuePath(IReadOnlyList<T> parts)
        {
            _parts = parts;
        }

        public int Count
        {
            get { return _parts.Count; }
        }

        public T this[int index]
        {
            get { return _parts[index]; }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _parts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _parts.GetEnumerator();
        }
    }
}