namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Concurrency;

    public class CollectionSynchronizer<T>
    {
        internal static readonly IReadOnlyList<NotifyCollectionChangedEventArgs> EmptyArgs = new NotifyCollectionChangedEventArgs[0];
        internal static readonly IReadOnlyList<NotifyCollectionChangedEventArgs> ResetArgs = new[] { Diff.NotifyCollectionResetEventArgs };
        private readonly List<T> _current = new List<T>();

        public CollectionSynchronizer(IEnumerable<T> source)
        {
            _current.AddRange(source);
        }

        internal IReadOnlyList<T> Current
        {
            get
            {
                lock (_current)
                {
                    return _current;
                }
            }
        }

        internal void Reset(
            object sender,
            IEnumerable<T> updated,
            IScheduler scheduler,
            PropertyChangedEventHandler propertyChanged,
            NotifyCollectionChangedEventHandler collectionChanged)
        {
            Refresh(sender, updated, EmptyArgs, scheduler, propertyChanged, collectionChanged);
        }

        internal void Refresh(
            object sender,
            IEnumerable<T> updated,
            IReadOnlyList<NotifyCollectionChangedEventArgs> collectionChanges,
            IScheduler scheduler,
            PropertyChangedEventHandler propertyChanged,
            NotifyCollectionChangedEventHandler collectionChanged)
        {
            lock (_current)
            {
                var changes = Update(updated, collectionChanges);
                Notify(sender, changes, scheduler, propertyChanged, collectionChanged);
            }
        }

        private IReadOnlyList<EventArgs> Update(IEnumerable<T> updated, IReadOnlyList<NotifyCollectionChangedEventArgs> collectionChanges)
        {
            var list = updated as IReadOnlyList<T> ?? updated.ToArray();
            IReadOnlyList<EventArgs> changes = Diff.Changes(_current, list);
            if (changes.Any())
            {
                _current.Clear();
                _current.AddRange(list);
            }
            return changes;
        }

        internal static void Notify(object sender,
                     IReadOnlyList<EventArgs> args,
                     IScheduler scheduler,
                     PropertyChangedEventHandler propertyChangedEventHandler,
                     NotifyCollectionChangedEventHandler collectionChangeEventHandler)
        {
            if (propertyChangedEventHandler == null && collectionChangeEventHandler == null)
            {
                return;
            }
            foreach (var e in args)
            {
                var propertyChangedEventArgs = e as PropertyChangedEventArgs;
                if (propertyChangedEventArgs != null && propertyChangedEventHandler != null)
                {
                    propertyChangedEventHandler(sender, propertyChangedEventArgs);
                }
                var notifyCollectionChangedEventArgs = e as NotifyCollectionChangedEventArgs;
                if (notifyCollectionChangedEventArgs != null && collectionChangeEventHandler != null)
                {
                    if (scheduler != null)
                    {
                        scheduler.Schedule(() => collectionChangeEventHandler(sender, notifyCollectionChangedEventArgs));
                    }
                    else
                    {
                        collectionChangeEventHandler(sender, notifyCollectionChangedEventArgs);
                    }
                }
            }
        }

        #region Ilist & IList<T>

        public int IndexOf(T value)
        {
            lock (_current)
            {
                return _current.IndexOf(value);
            }
        }

        public int IndexOf(object value)
        {
            lock (_current)
            {
                return ((IList)_current).IndexOf(value);
            }
        }

        public int LastIndexOf(T value)
        {
            lock (_current)
            {
                return _current.LastIndexOf(value);
            }
        }

        public bool Contains(T value)
        {
            lock (_current)
            {
                return _current.Contains(value);
            }
        }

        public bool Contains(object value)
        {
            lock (_current)
            {
                return ((IList)_current).Contains(value);
            }
        }

        public void CopyTo(Array array, int index)
        {
            lock (_current)
            {
                ((IList)_current).CopyTo(array, index);
            }
        }

        #endregion Ilist & IList<T>
    }
}