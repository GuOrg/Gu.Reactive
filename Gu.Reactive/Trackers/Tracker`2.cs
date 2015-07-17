namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Gu.Reactive.Annotations;
    using Gu.Reactive.Internals;

    public abstract class Tracker<TValue> : ITracker<TValue?> where TValue : struct 
    {
        protected readonly TValue? WhenEmpty;
        protected readonly IReadOnlyList<TValue> Source;
        private readonly IDisposable _subscription;

        private TValue? _value;
        private bool _disposed;

        protected Tracker(IReadOnlyList<TValue> source,
                          IObservable<NotifyCollectionChangedEventArgs> onRefresh,
                          TValue? whenEmpty)
        {
            Ensure.NotNull(source, "source");
            Ensure.NotNull(onRefresh, "resetObservable");
            Source = source;
            WhenEmpty = whenEmpty;
            _subscription = onRefresh.Subscribe(Refresh);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Reset()
        {
            VerifyDisposed();
            if (Source.Count == 0)
            {
                Value = WhenEmpty;
                return;
            }
            Value = GetValue(Source);
        }

        public TValue? Value
        {
            get
            {
                VerifyDisposed();
                return _value;
            }
            protected set
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
        /// Dispose(true); //I am calling you from Dispose, it's safe
        /// GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern. 
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            if (disposing)
            {
                _subscription.Dispose();
            }

            // Free any unmanaged objects here. 
        }

        protected virtual void Refresh(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (Source.Count > 1 && e.IsSingleNewItem())
                        {
                            var value = e.NewItem<TValue>();
                            OnAdd(value);
                        }
                        else
                        {
                            Reset();
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        if (e.IsSingleOldItem())
                        {
                            var value = e.OldItem<TValue>();
                            OnRemove(value);
                        }
                        else
                        {
                            Reset();
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Replace:
                    {
                        if (e.IsSingleOldItem() && e.IsSingleOldItem())
                        {
                            var oldValue = e.OldItem<TValue>();
                            var newValue = e.NewItem<TValue>();
                            OnReplace(oldValue, newValue);
                        }
                        else
                        {
                            Reset();
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        Reset();
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected abstract void OnAdd(TValue value);

        protected abstract void OnRemove(TValue value);

        protected abstract void OnReplace(TValue oldValue, TValue newValue);

        protected abstract TValue GetValue(IReadOnlyList<TValue> source);

        protected void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(
                    GetType()
                        .FullName);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}