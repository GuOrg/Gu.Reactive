namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;

    public sealed class WeakCompositeDisposable : IDisposable
    {
        private readonly object _gate = new object();
        private readonly List<WeakReference<IDisposable>> _disposables = new List<WeakReference<IDisposable>>();
        private bool _disposed;

        public void Add(IDisposable disposable)
        {
            VerifyDisposed();
            lock (_gate)
            {
                foreach (var wr in _disposables)
                {
                    IDisposable temp;
                    if (!wr.TryGetTarget(out temp))
                    {
                        wr.SetTarget(disposable);
                        return;
                    }
                }

                _disposables.Add(new WeakReference<IDisposable>(disposable));
            }
        }

        public void Purge()
        {
            VerifyDisposed();
            lock (_gate)
            {
                for (int i = 0; i < _disposables.Count; i++)
                {
                    var wr = _disposables[i];
                    IDisposable temp;
                    if (!wr.TryGetTarget(out temp))
                    {
                        _disposables.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// Make the class sealed when using this. 
        /// Call VerifyDisposed at the start of all public methods
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            lock (_gate)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                foreach (var weakReference in _disposables)
                {
                    IDisposable disposable;
                    if (weakReference.TryGetTarget(out disposable))
                    {
                        if (disposable != null)
                        {
                            disposable.Dispose();
                        }
                    }
                }
            }
        }

        private void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}
