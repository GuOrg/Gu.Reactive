namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reactive.Concurrency;
    using System.Threading.Tasks;
    using System.Windows;

    public static class ObservableCollectionExtensions
    {
        private struct VoidTypeStruct { }
        private static readonly Task CompletedTask = Task.FromResult(new VoidTypeStruct()); // Task.CompletedTask is internal
        private static readonly Task<bool> CompletedTrueTask = Task.FromResult(true);
        private static readonly Task<bool> CompletedFalseTask = Task.FromResult(false);
        public static void InvokeInsertSorted<T>(this ObservableCollection<T> collection, T item, Comparison<T> comparison= null)
        {
            if (comparison == null)
            {
                comparison = Comparer<T>.Default.Compare;
            }
            if (collection.Count == 0)
            {
                Shedule(() => collection.Add(item));
            }
            else
            {
                bool last = true;
                for (int i = 0; i < collection.Count; i++)
                {
                    int result = comparison.Invoke(collection[i], item);
                    if (result >= 1)
                    {
                        Shedule(() => collection.Insert(i, item));
                        last = false;
                        break;
                    }
                }
                if (last)
                {
                    Shedule(() => collection.Add(item));
                }
            }
        }

        public static void InvokeAdd<T>(this ObservableCollection<T> collection, T newItem)
        {
            Shedule(() => collection.Add(newItem));
        }

        public static void InvokeAddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> newItems)
        {
            Shedule(
                () =>
                {
                    foreach (var newItem in newItems)
                    {
                        collection.Add(newItem);
                    }
                });
        }

        public static void InvokeRemove<T>(this ObservableCollection<T> collection, T oldItem)
        {
            Shedule(() => collection.Remove(oldItem));
        }

        public static void InvokeRemove<T>(this ObservableCollection<T> collection, IEnumerable<T> oldItems)
        {
            Shedule(
                () =>
                {
                    foreach (var oldItem in oldItems)
                    {
                        collection.Remove(oldItem);
                    }
                });
        }

        public static void InvokeClear<T>(this ObservableCollection<T> collection)
        {
            Shedule(collection.Clear);
        }

        private static void Shedule(Action action)
        {
            Schedulers.DispatcherOrCurrentThread.Schedule(action);
        }

        public static Task AddAsync<T>(this ObservableCollection<T> collection, T newItem)
        {
            return InvokeAsync(() => collection.Add(newItem));
        }

        public static Task AddRangeAsync<T>(
            this ObservableCollection<T> collection,
            IEnumerable<T> newItems)
        {
            return InvokeAsync(
                () =>
                {
                    foreach (var newItem in newItems)
                    {
                        collection.Add(newItem);
                    }
                });
        }

        public static Task<bool> RemoveAsync<T>(this ObservableCollection<T> collection, T oldItem)
        {
            return InvokeAsyncResult(() => collection.Remove(oldItem));
        }

        public static Task ClearAsync<T>(this ObservableCollection<T> collection)
        {
            return InvokeAsync(collection.Clear);
        }

        private static Task InvokeAsync(Action action)
        {
            var application = Application.Current;
            if (application != null)
            {
                return application.Dispatcher.InvokeAsync(action).Task;
            }
            action();
            return CompletedTask;
        }

        private static Task<bool> InvokeAsyncResult(Func<bool> action)
        {
            var application = Application.Current;
            if (application != null)
            {
                return application.Dispatcher.InvokeAsync(action).Task;
            }
            var result = action();
            return result ?
                CompletedTrueTask :
                CompletedFalseTask;
        }
    }
}
