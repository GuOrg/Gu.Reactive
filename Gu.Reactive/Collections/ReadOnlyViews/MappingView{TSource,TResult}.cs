namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    using Gu.Reactive.Internals;

    /// <summary>
    /// A view of a collection that maps the values.
    /// </summary>
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {this.Count}")]
    public partial class MappingView<TSource, TResult> : ReadonlySerialViewBase<TSource, TResult>, IReadOnlyObservableCollection<TResult>, IUpdater
    {
        private readonly IDisposable refreshSubscription;
#pragma warning disable GU0037 // Don't assign member with injected and created disposables.
        private readonly IMapper<TSource, TResult> factory;
#pragma warning restore GU0037 // Don't assign member with injected and created disposables.

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        internal MappingView(IEnumerable<TSource> source, IMapper<TSource, TResult> factory, TimeSpan bufferTime, IScheduler scheduler, params IObservable<object>[] triggers)
            : base(source, s => s.Select(factory.GetOrCreate), true)
        {
            Ensure.NotNull(source as INotifyCollectionChanged, nameof(source));
            Ensure.NotNull(factory, nameof(factory));

            this.factory = factory;
            this.refreshSubscription = Observable.Merge(
                                                     source.ObserveCollectionChangedSlimOrDefault(false),
                                                     triggers.MergeOrNever()
                                                             .Select(x => CachedEventArgs.NotifyCollectionReset))
                                                 .Chunks(bufferTime, scheduler)
                                                 .ObserveOn(scheduler ?? ImmediateScheduler.Instance)
                                                 .StartWith(CachedEventArgs.SingleNotifyCollectionReset)
                                                 .Subscribe(this.Refresh);
        }

        /// <inheritdoc/>
        object IUpdater.CurrentlyUpdatingSourceItem => null;

        /// <inheritdoc/>
        public override void Refresh()
        {
            using (this.factory.RefreshTransaction())
            {
                base.Refresh();
            }
        }

        /// <summary>
        /// Called when the source collection changed.
        /// </summary>
        /// <param name="changes">The changes accumulated during the buffer time.</param>
        protected override void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes)
        {
            if (changes == null || changes.Count == 0)
            {
                return;
            }

            if (changes.Count > 1)
            {
                this.Refresh(changes);
                return;
            }

            var e = changes[0];
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (!e.TryGetSingleNewItem(out TSource newSource))
                        {
                            goto case NotifyCollectionChangedAction.Reset;
                        }

                        var index = e.NewStartingIndex;
                        var value = this.GetOrCreate(newSource, index);
                        this.Tracker.Insert(index, value);
                        var args = this.UpdateRange(index + 1, this.Count - 1);
                        args.Add(Diff.CreateAddEventArgs(value, index));
                        this.Notify(args);
                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        if (!e.TryGetSingleOldItem(out TSource oldSource))
                        {
                            goto case NotifyCollectionChangedAction.Reset;
                        }

                        var index = e.OldStartingIndex;
                        var value = this.Tracker[index];
                        this.Tracker.RemoveAt(index);
                        var argses = this.UpdateRange(index, this.Count - 1);
                        argses.Add(Diff.CreateRemoveEventArgs(value, index));
                        this.Notify(argses);
                        this.factory.Remove(oldSource, value);
                        break;
                    }

                case NotifyCollectionChangedAction.Replace:
                    {
                        if (!e.TryGetSingleNewItem(out TSource newSource) ||
                            !e.TryGetSingleOldItem(out TSource oldSource))
                        {
                            goto case NotifyCollectionChangedAction.Reset;
                        }

                        var index = e.NewStartingIndex;
                        var value = this.GetOrCreate(newSource, index);
                        var oldValue = this.Tracker[e.OldStartingIndex];
                        this.Tracker[index] = value;
                        var arg = Diff.CreateReplaceEventArgs(value, oldValue, index);
                        this.Notify(arg);
                        this.factory.Remove(oldSource, oldValue);
                        break;
                    }

                case NotifyCollectionChangedAction.Move:
                    {
                        var value = this.Tracker[e.OldStartingIndex];
                        this.Tracker.RemoveAt(e.OldStartingIndex);
                        this.Tracker.Insert(e.NewStartingIndex, value);
                        var args = this.UpdateRange(Math.Min(e.OldStartingIndex, e.NewStartingIndex), Math.Max(e.OldStartingIndex, e.NewStartingIndex));
                        args.Add(Diff.CreateMoveEventArgs(value, e.NewStartingIndex, e.OldStartingIndex));
                        this.Notify(args);
                        break;
                    }

                case NotifyCollectionChangedAction.Reset:
                    using (this.factory.RefreshTransaction())
                    {
                        base.Refresh(changes);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Delegates creation to mapping factory.
        /// </summary>
        protected virtual TResult GetOrCreate(TSource key, int index) => this.factory.GetOrCreate(key, index);

        /// <summary>
        /// Delegates updating of item at index to mapping factory.
        /// </summary>
        /// <param name="index">The index to update the item for.</param>
        /// <param name="createEventArgOnUpdate">If a <see cref="NotifyCollectionChangedEventArgs"/> for the update should be created.</param>
        /// <returns>
        /// The <see cref="NotifyCollectionChangedEventArgs"/> update causes or null.
        /// If the updated instance is the same reference null is returned.
        /// </returns>
        protected virtual NotifyCollectionChangedEventArgs UpdateAt(int index, bool createEventArgOnUpdate)
        {
            if (!this.factory.CanUpdateIndex)
            {
                return null;
            }

            var old = this.Tracker[index];
            var updated = this.factory.Update(this.Source.ElementAt(index), old, index);
            if (ReferenceEquals(updated, old))
            {
                return null;
            }

            this.Tracker[index] = updated;
            return createEventArgOnUpdate
                ? Diff.CreateReplaceEventArgs(updated, old, index)
                : null;
        }

        /// <summary>
        /// Delegates updating of items at and above index to mapping factory.
        /// This happens after an item is inserted, removed or moved.
        /// </summary>
        /// <param name="from">The index to start update of the item for.</param>
        /// <param name="to">The index to end update of the item for.</param>
        /// <returns>The collection changed args the update causes.</returns>
        protected virtual List<NotifyCollectionChangedEventArgs> UpdateRange(int from, int to)
        {
            if (!this.factory.CanUpdateIndex)
            {
                return new List<NotifyCollectionChangedEventArgs>();
            }

            var changes = new List<NotifyCollectionChangedEventArgs>();
            for (var i = from; i < Math.Min(this.Count, to + 1); i++)
            {
                var change = this.UpdateAt(i, changes.Count < 2);
                if (change != null)
                {
                    changes.Add(change);
                }
            }

            return changes;
        }

        /// <summary>
        /// Disposes of a <see cref="MappingView{TSource,TResult}"/>.
        /// </summary>
        /// <remarks>
        /// Called from Dispose() with disposing=true.
        /// Guidelines:
        /// 1. We may be called more than once: do nothing after the first call.
        /// 2. Avoid throwing exceptions if disposing is false, i.e. if we're being finalized.
        /// </remarks>
        /// <param name="disposing">True if called from Dispose(), false if called from the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.refreshSubscription.Dispose();
#pragma warning disable GU0036 // Don't dispose injected.
                this.factory.Dispose();
#pragma warning restore GU0036 // Don't dispose injected.
            }

            base.Dispose(disposing);
        }
    }
}
