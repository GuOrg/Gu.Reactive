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
                bool flag;
                var disposable = (IDisposable)null;
                lock (this.gate)
                {
                    if (ReferenceEquals(value, this.current))
                    {
                        return;
                    }

                    flag = this.disposed;
                    if (!flag)
                    {
                        disposable = this.current;
                        this.current = value;
                        this.OnPropertyChanged();
                    }
                }

                disposable?.Dispose();
                if (!flag || value == null)
                {
                    return;
                }

                value.Dispose();
            }
        }

        /// <summary>
        /// Disposes the underlying disposable as well as all future replacements.
        /// </summary>
        public void Dispose()
        {
            var disposable = (IDisposable)null;
            lock (this.gate)
            {
                if (!this.disposed)
                {
                    this.disposed = true;
                    disposable = this.current;
                    this.current = null;
                }
            }

            disposable?.Dispose();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}