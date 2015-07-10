namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    internal static class Diff
    {
        private const string CountName = "Count";
        private const string IndexerName = "Item[]";

        internal static readonly NotifyCollectionChangedEventArgs NotifyCollectionResetEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

        internal static readonly PropertyChangedEventArgs CountPropertyChangedEventArgs = new PropertyChangedEventArgs(CountName);

        internal static readonly PropertyChangedEventArgs IndexerPropertyChangedEventArgs = new PropertyChangedEventArgs(IndexerName);

        internal static readonly IReadOnlyList<EventArgs> ResetEventArgsCollection = new EventArgs[]
                                                                              {
                                                                                  CountPropertyChangedEventArgs,
                                                                                  IndexerPropertyChangedEventArgs,
                                                                                  NotifyCollectionResetEventArgs
                                                                              };

        internal static readonly IReadOnlyList<EventArgs> EmptyEventArgsCollection = new EventArgs[0];

        internal static IReadOnlyList<EventArgs> Changes<T>(IReadOnlyList<T> before, IReadOnlyList<T> after)
        {
            var diff = before.Count - after.Count;
            if (Math.Abs(diff) > 1)
            {
                return ResetEventArgsCollection;
            }
            IComparer<T> comparer;
            if (typeof(T).IsValueType)
            {
                comparer = Comparer<T>.Default;
            }
            else
            {
                comparer = RefComparer<T>.Default;
            }
            if (diff == -1)
            {
                return AddOrReset(before, after, comparer);
            }

            if (diff == 1)
            {
                return RemoveOrReset(before, after, comparer);
            }

            return MoveReplaceNoneOrReset(before, after, comparer);
        }

        internal static NotifyCollectionChangedEventArgs CreateAddEventArgs(object newItem, int newIndex)
        {
            return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem, newIndex);
        }

        internal static NotifyCollectionChangedEventArgs CreateRemoveEventArgs(object oldItem, int index)
        {
            return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index);
        }

        internal static NotifyCollectionChangedEventArgs CreateReplaceEventArgs(object newItem, object oldItem, int index)
        {
            return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index);
        }

        internal static NotifyCollectionChangedEventArgs CreateMoveEventArgs(object item, int newIndex, int oldIndex)
        {
            return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex);
        }

        private static IReadOnlyList<EventArgs> AddOrReset<T>(IReadOnlyList<T> before, IReadOnlyList<T> after, IComparer<T> comparer)
        {
            int newIndex = after.Count - 1;
            int offset = 0;
            for (int i = 0; i < before.Count; i++)
            {
                if (comparer.Compare(before[i], after[i + offset]) != 0)
                {
                    if (newIndex != after.Count - 1)
                    {
                        return ResetEventArgsCollection;
                    }
                    newIndex = i;
                    offset = 1;
                    i--;
                }
            }
            return new EventArgs[]
                       {
                           CountPropertyChangedEventArgs,
                           IndexerPropertyChangedEventArgs,
                           CreateAddEventArgs(after[newIndex], newIndex),
                       };
        }

        private static IReadOnlyList<EventArgs> RemoveOrReset<T>(IReadOnlyList<T> before, IReadOnlyList<T> after, IComparer<T> comparer)
        {
            int oldIndex = before.Count - 1;
            int offset = 0;
            for (int i = 0; i < after.Count; i++)
            {
                if (comparer.Compare(before[i - offset], after[i]) != 0)
                {
                    if (oldIndex != before.Count - 1)
                    {
                        return ResetEventArgsCollection;
                    }
                    oldIndex = i;
                    offset = -1;
                }
            }
            return new EventArgs[]
                       {
                           CountPropertyChangedEventArgs,
                           IndexerPropertyChangedEventArgs,
                           CreateRemoveEventArgs(before[oldIndex], oldIndex)
                       };
        }

        private static IReadOnlyList<EventArgs> MoveReplaceNoneOrReset<T>(IReadOnlyList<T> before, IReadOnlyList<T> after, IComparer<T> comparer)
        {
            int oldIndex = -1;
            int newIndex = -1;
            for (int i = 0; i < after.Count; i++)
            {
                if (comparer.Compare(before[i], after[i]) != 0)
                {
                    if (oldIndex == -1)
                    {
                        oldIndex = i;
                    }
                    else if (newIndex == -1)
                    {
                        newIndex = i;
                    }
                    else
                    {
                        return ResetEventArgsCollection;
                    }
                }
            }
            if (oldIndex == -1 && newIndex == -1)
            {
                return EmptyEventArgsCollection;
            }
            if (oldIndex > -1 && newIndex > -1)
            {
                if (comparer.Compare(before[oldIndex], after[newIndex]) == 0 && comparer.Compare(before[newIndex], after[oldIndex]) == 0)
                {
                    return new EventArgs[]
                       {
                           IndexerPropertyChangedEventArgs,
                           CreateMoveEventArgs(before[oldIndex],newIndex, oldIndex)
                       };
                }
            }
            if (oldIndex > -1 && newIndex == -1)
            {
                return new EventArgs[]
                           {
                               IndexerPropertyChangedEventArgs,
                               CreateReplaceEventArgs(after[oldIndex], before[oldIndex], oldIndex)
                           };
            }
            return ResetEventArgsCollection; // Resetting here, throwing is an alternative.
        }

        private class RefComparer<T> : IComparer<T>
        {
            internal static readonly RefComparer<T> Default = new RefComparer<T>();
            public int Compare(T x, T y)
            {
                return ReferenceEquals(x, y)
                           ? 0
                           : 1;
            }
        }
    }
}
