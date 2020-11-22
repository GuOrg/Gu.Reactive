namespace Gu.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Disposables;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// A generic version of <see cref="System.Reactive.Disposables.SerialDisposable"/>.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    public sealed class SerialDisposable<T> : ICancelable, IDisposable, INotifyPropertyChanged
        where T : class, IDisposable
    {
        private readonly object gate = new object();
        private T? disposable;
        private bool isDisposed;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets a value that indicates whether the object is disposed.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                lock (this.gate)
                {
                    return this.isDisposed;
                }
            }
        }

        /// <summary>Gets or sets the underlying disposable.</summary>
        /// <remarks>If the SerialDisposable has already been disposed, assignment to this property causes immediate disposal of the given disposable object. Assigning this property disposes the previous disposable object.</remarks>
        public T? Disposable
        {
            get => this.disposable;

            set
            {
                if (ReferenceEquals(value, this.disposable))
                {
                    return;
                }

                if (this.isDisposed)
                {
#pragma warning disable IDISP007 // Don't dispose injected.
                    value?.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
                    return;
                }

                IDisposable? toDispose;
                lock (this.gate)
                {
                    if (ReferenceEquals(value, this.disposable))
                    {
                        return;
                    }

                    if (this.isDisposed)
                    {
                        toDispose = value;
                    }
                    else
                    {
                        toDispose = this.disposable;
                        this.disposable = value;
                    }
                }

                this.OnPropertyChanged();
#pragma warning disable IDISP007 // Don't dispose injected.
                toDispose?.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
            }
        }

        /// <summary>
        /// Disposes the underlying disposable as well as all future replacements.
        /// </summary>
        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            IDisposable? toDispose;
            lock (this.gate)
            {
                if (this.isDisposed)
                {
                    return;
                }

                this.isDisposed = true;
                toDispose = this.disposable;
                this.disposable = null;
            }

            toDispose?.Dispose();
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
