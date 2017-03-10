namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A weak <see cref="System.Reactive.Disposables.CompositeDisposable"/>
    /// </summary>
    public sealed class WeakCompositeDisposable : IDisposable
    {
        private readonly object gate = new object();
        private readonly List<WeakReference<IDisposable>> disposables = new List<WeakReference<IDisposable>>();
        private bool disposed;

        /// <summary>
        /// Adds a disposable to the CompositeDisposable or disposes the disposable if the CompositeDisposable is disposed.
        /// </summary>
        public void Add(IDisposable disposable)
        {
            if (this.disposed)
            {
#pragma warning disable GU0036 // Don't dispose injected.
                disposable.Dispose();
#pragma warning restore GU0036 // Don't dispose injected.
                return;
            }

            lock (this.gate)
            {
                if (this.disposed)
                {
#pragma warning disable GU0036 // Don't dispose injected.
                    disposable.Dispose();
#pragma warning restore GU0036 // Don't dispose injected.
                    return;
                }

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

        /// <summary>
        /// Delete all weakreferences where target is collected.
        /// </summary>
        public void Purge()
        {
            this.ThrowIfDisposed();
            lock (this.gate)
            {
                for (var i = 0; i < this.disposables.Count; i++)
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
        /// Call ThrowIfDisposed at the start of all public methods
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
                        disposable.Dispose();
                    }
                }

                this.disposables.Clear();
            }
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}
