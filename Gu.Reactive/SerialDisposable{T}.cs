namespace Gu.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Disposables;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// A generic version of <see cref="System.Reactive.Disposables.SerialDisposable"/>
    /// </summary>
    public sealed class SerialDisposable<T> : ICancelable, IDisposable, INotifyPropertyChanged
        where T : class, IDisposable
    {
        private readonly object gate = new object();
        private T current;
        private bool disposed;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets a value that indicates whether the object is disposed.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                lock (this.gate)
                {
                    return this.disposed;
                }
            }
        }

        /// <summary>Gets or sets the underlying disposable.</summary>
        /// <remarks>If the SerialDisposable has already been disposed, assignment to this property causes immediate disposal of the given disposable object. Assigning this property disposes the previous disposable object.</remarks>
        public T Disposable
        {
            get
            {
                return this.current;
            }

            set
            {
                if (ReferenceEquals(value, this.current))
                {
                    return;
                }

                if (this.disposed)
                {
#pragma warning disable GU0036 // Don't dispose injected.
                    value?.Dispose();
#pragma warning restore GU0036 // Don't dispose injected.
                    return;
                }

                IDisposable toDispose;
                lock (this.gate)
                {
                    if (ReferenceEquals(value, this.current))
                    {
                        return;
                    }

                    if (this.disposed)
                    {
                        toDispose = value;
                    }
                    else
                    {
                        toDispose = this.current;
                        this.current = value;
                    }
                }

                this.OnPropertyChanged();
                toDispose?.Dispose();
            }
        }

        /// <summary>
        /// Disposes the underlying disposable as well as all future replacements.
        /// </summary>
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            IDisposable toDispose;
            lock (this.gate)
            {
                if (this.disposed)
                {
                    return;
                }

                this.disposed = true;
                toDispose = this.current;
                this.current = null;
            }

            toDispose?.Dispose();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}