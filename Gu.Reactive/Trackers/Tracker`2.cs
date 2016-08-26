namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Gu.Reactive.Internals;
    using JetBrains.Annotations;

    public abstract class Tracker<TValue> : ITracker<TValue?> where TValue : struct
    {
        protected readonly TValue? WhenEmpty;
        protected readonly IReadOnlyList<TValue> Source;
        private readonly IDisposable subscription;

        private TValue? value;
        private bool disposed;

        protected Tracker(
            IReadOnlyList<TValue> source,
                          IObservable<NotifyCollectionChangedEventArgs> onRefresh,
                          TValue? whenEmpty)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(onRefresh, nameof(onRefresh));
            this.Source = source;
            this.WhenEmpty = whenEmpty;
            this.subscription = onRefresh.Subscribe(this.Refresh);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Reset()
        {
            this.VerifyDisposed();
            if (this.Source.Count == 0)
            {
                this.Value = this.WhenEmpty;
                return;
            }

            this.Value = this.GetValue(this.Source);
        }

        public TValue? Value
        {
            get
            {
                this.VerifyDisposed();
                return this.value;
            }

            protected set
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
        /// Dispose(true); //I am calling you from Dispose, it's safe
        /// GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
                this.subscription.Dispose();
            }

            // Free any unmanaged objects here.
        }

        protected virtual void Refresh(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (this.Source.Count > 1 && e.IsSingleNewItem())
                        {
                            var value = e.NewItem<TValue>();
                            this.OnAdd(value);
                        }
                        else
                        {
                            this.Reset();
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        if (e.IsSingleOldItem())
                        {
                            var value = e.OldItem<TValue>();
                            this.OnRemove(value);
                        }
                        else
                        {
                            this.Reset();
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Replace:
                    {
                        if (e.IsSingleOldItem() && e.IsSingleOldItem())
                        {
                            var oldValue = e.OldItem<TValue>();
                            var newValue = e.NewItem<TValue>();
                            this.OnReplace(oldValue, newValue);
                        }
                        else
                        {
                            this.Reset();
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        this.Reset();
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
            if (this.disposed)
            {
                throw new ObjectDisposedException(
                    this.GetType()
                        .FullName);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}