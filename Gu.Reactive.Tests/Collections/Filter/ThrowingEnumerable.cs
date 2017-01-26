namespace Gu.Reactive.Tests.Collections.Filter
{
    using System.Collections;
    using System.Collections.Generic;

    public class ThrowingEnumerable<T> : IEnumerable<T>
    {
        private readonly Stack<bool> throws;
        private readonly List<T> inner = new List<T>();

        public ThrowingEnumerable(IEnumerable<T> items, Stack<bool> throws)
        {
            this.throws = throws;
            this.inner.AddRange(items);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ThrowingEnumerator(this.inner, this.throws.Count > 0 && this.throws.Pop());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.inner).GetEnumerator();
        }

        private class ThrowingEnumerator : IEnumerator<T>
        {
            private readonly List<T> source;
            private readonly IEnumerator<T> inner;

            private bool throws;

            public ThrowingEnumerator(List<T> source, bool throws)
            {
                this.source = source;
                this.throws = throws;
                this.inner = source.GetEnumerator();
            }

            public T Current => this.inner.Current;

            object IEnumerator.Current => ((IEnumerator)this.inner).Current;

            public void Dispose() => this.inner.Dispose();

            public bool MoveNext()
            {
                var moveNext = this.inner.MoveNext();
                if (this.throws)
                {
                    this.source.Add(default(T));
                    this.throws = false;
                }

                return moveNext;
            }

            public void Reset() => this.inner.Reset();
        }
    }
}