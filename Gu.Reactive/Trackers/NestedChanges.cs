namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    using Gu.Reactive.Internals;

    internal sealed class NestedChanges<TItem, TValue> : IDisposable
        where TItem : class, INotifyPropertyChanged
        where TValue : struct, IComparable<TValue>
    {
        private readonly IReadOnlyObservableCollection<PropertyPathTracker<TItem, TValue>> source;
        private readonly IDisposable subscription;
        private bool disposed;

        public NestedChanges(IReadOnlyObservableCollection<PropertyPathTracker<TItem, TValue>> source)
        {
            this.source = source;
            this.subscription = source.ObserveCollectionChangedSlim(false)
                                      .Subscribe(this.OnSourceChanged);
        }

        internal event Action<TValue> Add;

        internal event Action<TValue> Remove;

        internal event Action<IEnumerable<TValue>> Reset;

        internal IEnumerable<TValue> Values => this.source.Select(x => x.SourceAndValue())
                                                   .Where(m => m.Value.HasValue)
                                                   .Select(m => m.Value.Value);

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.subscription?.Dispose();
        }

        private void OnSourceChanged(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (e.TryGetSingleNewItem(out PropertyPathTracker<TItem, TValue> item))
                        {
                            this.Add?.Invoke(item);
                        }
                        else
                        {
                            this.Reset?.Invoke(this.Values);
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        if (e.TryGetSingleOldItem(out PropertyPathTracker<TItem, TValue> item))
                        {
                            this.Remove?.Invoke(item);
                        }
                        else
                        {
                            this.Reset?.Invoke(this.Values);
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Replace:
                    {
                        if (e.TryGetSingleNewItem(out PropertyPathTracker<TItem, TValue> newValue) &&
                            e.TryGetSingleOldItem(out PropertyPathTracker<TItem, TValue> oldValue))
                        {
                            this.Add?.Invoke(newValue);
                            this.Remove?.Invoke(oldValue);
                        }
                        else
                        {
                            this.Reset?.Invoke(this.Values);
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        this.Reset?.Invoke(this.Values);
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}