namespace Gu.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Gu.Reactive.Internals;
    using JetBrains.Annotations;

    public sealed class Mapper<TSource, TResult> : ITracker<TResult>
    {
        private readonly ITracker<TSource> _source;
        private readonly Func<TSource, TResult> _selector;
        private readonly IDisposable _subscription;

        private bool _disposed;

        private TResult _value;


        public Mapper(ITracker<TSource> source, Func<TSource, TResult> selector)
        {
            _source = source;
            _selector = selector;
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            _subscription = source.ObservePropertyChangedSlim(nameof(source.Value))
                                  .Subscribe(_ => { Value = _selector(_source.Value); });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TResult Value
        {
            get { return _value; }
            private set
            {
                if (Equals(value, _value))
                {
                    return;
                }

                _value = value;
                OnPropertyChanged();
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

            _disposed = true;
            _subscription.Dispose();
        }

        private void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(
                    GetType()
                        .FullName);
            }
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
