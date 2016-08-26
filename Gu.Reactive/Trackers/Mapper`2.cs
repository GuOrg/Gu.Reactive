namespace Gu.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Gu.Reactive.Internals;
    using JetBrains.Annotations;

    public sealed class Mapper<TSource, TResult> : ITracker<TResult>
    {
        private readonly ITracker<TSource> source;
        private readonly Func<TSource, TResult> selector;
        private readonly IDisposable subscription;

        private bool disposed;

        private TResult value;


        public Mapper(ITracker<TSource> source, Func<TSource, TResult> selector)
        {
            this.source = source;
            this.selector = selector;
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            this.subscription = source.ObservePropertyChangedSlim(nameof(source.Value))
                                  .Subscribe(_ => { this.Value = this.selector(this.source.Value); });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TResult Value
        {
            get { return this.value; }

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

            this.disposed = true;
            this.subscription.Dispose();
        }

        private void VerifyDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(
                    this.GetType()
                        .FullName);
            }
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
