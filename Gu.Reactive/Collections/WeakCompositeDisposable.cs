namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;

    public sealed class WeakCompositeDisposable : IDisposable
    {
        private readonly object gate = new object();
        private readonly List<WeakReference<IDisposable>> disposables = new List<WeakReference<IDisposable>>();
        private bool disposed;

        public void Add(IDisposable disposable)
        {
            this.VerifyDisposed();
            lock (this.gate)
            {
                foreach (var wr in this.disposables)
                {
                    IDisposable temp;
                    if (!wr.TryGetTarget(out temp))
                    {
                        wr.SetTarget(disposable);
                        return;
                    }
                }

                this.disposables.Add(new WeakReference<IDisposable>(disposable));
            }
        }

        public void Purge()
        {
            this.VerifyDisposed();
            lock (this.gate)
            {
                for (int i = 0; i < this.disposables.Count; i++)
                {
                    var wr = this.disposables[i];
                    IDisposable temp;
                    if (!wr.TryGetTarget(out temp))
                    {
                        this.disposables.RemoveAt(i);
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
            if (this.disposed)
            {
                return;
            }

            lock (this.gate)
            {
                if (this.disposed)
                {
                    return;
                }

                this.disposed = true;
                foreach (var weakReference in this.disposables)
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
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}
