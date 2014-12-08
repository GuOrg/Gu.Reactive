namespace Gu.Wpf.Reactive
{
    using System.Collections;
    using System.Collections.Generic;

    public struct CastEnumerator<T> : IEnumerator<T>
    {
        private readonly IEnumerator _enumerator;
        public CastEnumerator(IEnumerator enumerator)
        {
            _enumerator = enumerator;
        }

        public void Dispose()
        {
            // What goes here?
        }

        public bool MoveNext()
        {
            return _enumerator.MoveNext();
        }

        public void Reset()
        {
            _enumerator.Reset();
        }

        public T Current
        {
            get { return (T)_enumerator.Current; }
        }

        object IEnumerator.Current
        {
            get { return _enumerator.Current; }
        }
    }
}