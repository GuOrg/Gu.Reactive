namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    internal static class Diff
    {
        internal static readonly NotifyCollectionChangedEventArgs NotifyCollectionResetEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);


        internal static readonly IReadOnlyList<EventArgs> ResetEventArgsCollection = new EventArgs[]
                                                                              { Notifier.CountPropertyChangedEventArgs, Notifier.IndexerPropertyChangedEventArgs,
                                                                                  NotifyCollectionResetEventArgs
                                                                              };

        public static NotifyCollectionChangedEventArgs CollectionChange<T>(IReadOnlyList<T> before, IReadOnlyList<T> after, IReadOnlyList<NotifyCollectionChangedEventArgs> collectionChanges)
        {
            if (collectionChanges != null && collectionChanges.Count == 1)
            {
                var change = collectionChanges[0];
                switch (change.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    case NotifyCollectionChangedAction.Remove:
                    case NotifyCollectionChangedAction.Replace:
                    case NotifyCollectionChangedAction.Move:
                        return change;
                    case NotifyCollectionChangedAction.Reset:
                        return CollectionChange(before, after);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return CollectionChange(before, after);
        }

        internal static NotifyCollectionChangedEventArgs CollectionChange<T>(IReadOnlyList<T> before, IReadOnlyList<T> after)
        {
            var diff = before.Count - after.Count;
            if (Math.Abs(diff) > 1)
            {
                return NotifyCollectionResetEventArgs;
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

        private static NotifyCollectionChangedEventArgs AddOrReset<T>(IReadOnlyList<T> before, IReadOnlyList<T> after, IComparer<T> comparer)
        {
            int newIndex = after.Count - 1;
            int offset = 0;
            for (int i = 0; i < before.Count; i++)
            {
                if (comparer.Compare(before[i], after[i + offset]) != 0)
                {
                    if (newIndex != after.Count - 1)
                    {
                        return NotifyCollectionResetEventArgs;
                    }

                    newIndex = i;
                    offset = 1;
                    i--;
                }
            }

            return CreateAddEventArgs(after[newIndex], newIndex);
        }

        private static NotifyCollectionChangedEventArgs RemoveOrReset<T>(IReadOnlyList<T> before, IReadOnlyList<T> after, IComparer<T> comparer)
        {
            int oldIndex = before.Count - 1;
            int offset = 0;
            for (int i = 0; i < after.Count; i++)
            {
                if (comparer.Compare(before[i - offset], after[i]) != 0)
                {
                    if (oldIndex != before.Count - 1)
                    {
                        return NotifyCollectionResetEventArgs;
                    }

                    oldIndex = i;
                    offset = -1;
                }
            }

            return CreateRemoveEventArgs(before[oldIndex], oldIndex);
        }

        private static NotifyCollectionChangedEventArgs MoveReplaceNoneOrReset<T>(IReadOnlyList<T> before, IReadOnlyList<T> after, IComparer<T> comparer)
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
                        return NotifyCollectionResetEventArgs;
                    }
                }
            }

            if (oldIndex == -1 && newIndex == -1)
            {
                return null;
            }

            if (oldIndex > -1 && newIndex > -1)
            {
                if (comparer.Compare(before[oldIndex], after[newIndex]) == 0 && comparer.Compare(before[newIndex], after[oldIndex]) == 0)
                {
                    return CreateMoveEventArgs(before[oldIndex], newIndex, oldIndex);
                }
            }

            if (oldIndex > -1 && newIndex == -1)
            {
                return CreateReplaceEventArgs(after[oldIndex], before[oldIndex], oldIndex);
            }

            return NotifyCollectionResetEventArgs; // Resetting here, throwing is an alternative.
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
