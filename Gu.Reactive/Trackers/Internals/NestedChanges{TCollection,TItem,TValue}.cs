namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using Gu.Reactive.Internals;

    internal sealed class NestedChanges<TCollection, TItem, TValue> : IChanges<TValue>
        where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
        where TItem : class, INotifyPropertyChanged
        where TValue : struct, IComparable<TValue>
    {
        private readonly IReadOnlyObservableCollection<ValueTracker> source;
        private readonly IDisposable subscription;
        private bool disposed;

        public NestedChanges(TCollection source, Expression<Func<TItem, TValue>> selector)
        {
            var path = NotifyingPath.GetOrCreate(selector);
            this.source = new MappingView<TItem, ValueTracker>(
                source,
                Mapper.Create<TItem, ValueTracker>(x => new ValueTracker(this, x, path), x => x.Dispose()),
                TimeSpan.Zero,
                null);
            this.subscription = source.ObserveCollectionChangedSlim(false)
                                      .Subscribe(this.OnSourceChanged);
        }

        public event Action<TValue> Add;

        public event Action<TValue> Remove;

        public event Action<IEnumerable<TValue>> Reset;

        public IEnumerable<TValue> Values => this.source.Where(m => m.Value.HasValue)
                                                 .Select(m => m.Value.Value);

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.subscription?.Dispose();
            (this.source as IDisposable)?.Dispose();
        }

        private void OnSourceChanged(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (e.TryGetSingleNewItem(out ValueTracker tracker))
                        {
                            var value = tracker.Value;
                            if (value.HasValue)
                            {
                                this.Add?.Invoke(value.Value);
                            }
                        }
                        else
                        {
                            this.Reset?.Invoke(this.Values);
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        if (e.TryGetSingleOldItem(out ValueTracker tracker))
                        {
                            var value = tracker.Value;
                            if (value.HasValue)
                            {
                                this.Remove?.Invoke(value.Value);
                            }
                        }
                        else
                        {
                            this.Reset?.Invoke(this.Values);
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Replace:
                    {
                        if (e.TryGetSingleNewItem(out ValueTracker newTracker) &&
                            e.TryGetSingleOldItem(out ValueTracker oldTracker))
                        {
                            var newValue = newTracker.Value;
                            if (newValue.HasValue)
                            {
                                this.Add?.Invoke(newValue.Value);
                            }

                            var oldValue = oldTracker.Value;
                            if (oldValue.HasValue)
                            {
                                this.Remove?.Invoke(oldValue.Value);
                            }
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

        private void OnAdd(TValue obj)
        {
            this.Add?.Invoke(obj);
        }

        private void OnRemove(TValue obj)
        {
            this.Remove?.Invoke(obj);
        }

        private sealed class ValueTracker : IDisposable
        {
            private readonly NestedChanges<TCollection, TItem, TValue> nestedChanges;
            private readonly PropertyPathTracker<TItem, TValue> inner;
            private bool disposed;
            private Maybe<TValue> value;

            public ValueTracker(NestedChanges<TCollection, TItem, TValue> nestedChanges, TItem item, NotifyingPath<TItem, TValue> path)
            {
                this.nestedChanges = nestedChanges;
                this.inner = path.CreateTracker(item);
                this.inner.TrackedPropertyChanged += this.TrackerOnTrackedPropertyChanged;
                this.value = this.inner.SourceAndValue().Value;
            }

            public Maybe<TValue> Value
            {
                get
                {
                    return this.value;
                }

                private set
                {
                    var before = this.value;
                    this.value = value;

                    if (this.value.HasValue)
                    {
                        this.nestedChanges.OnAdd(this.value.Value);
                    }

                    if (before.HasValue)
                    {
                        this.nestedChanges.OnRemove(before.Value);
                    }
                }
            }

            public void Dispose()
            {
                if (this.disposed)
                {
                    return;
                }

                this.disposed = true;
                this.inner.TrackedPropertyChanged -= this.TrackerOnTrackedPropertyChanged;
                this.inner?.Dispose();
            }

            private void TrackerOnTrackedPropertyChanged(IPropertyTracker tracker, object sender, PropertyChangedEventArgs propertyChangedEventArgs, SourceAndValue<INotifyPropertyChanged, TValue> sourceAndValue)
            {
                this.Value = sourceAndValue.Value;
            }
        }
    }
}