namespace Gu.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Gu.Reactive.Internals;

    /// <summary>
    /// A reactive mapper from <typeparamref name="TSource"/> to <typeparamref name="TResult"/>
    /// </summary>
    public sealed class Mapper<TSource, TResult> : ITracker<TResult>
    {
        private readonly IDisposable subscription;

        private bool disposed;
        private TResult value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mapper{TSource, TResult}"/> class.
        /// </summary>
        public Mapper(ITracker<TSource> source, Func<TSource, TResult> selector)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            this.subscription = source.ObservePropertyChangedSlim(nameof(source.Value))
                                      .Subscribe(_ => { this.Value = selector(source.Value); });
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The mapped value.
        /// </summary>
        public TResult Value
        {
            get
            {
                this.ThrowIfDisposed();
                return this.value;
            }

            private set
            {
                if (Equals(value, this.value))
                {
                    return;
                }

                this.value = value;
                this.OnPropertyChanged();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.subscription.Dispose();
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
