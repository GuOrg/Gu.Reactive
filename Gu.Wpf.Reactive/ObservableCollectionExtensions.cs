namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reactive.Concurrency;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;

    public static class ObservableCollectionExtensions
    {
        public static void InsertSorted<T>(this ObservableCollection<T> collection, T item, Comparison<T> comparison)
        {
            if (collection.Count == 0)
            {
                collection.Add(item);
            }
            else
            {
                bool last = true;
                for (int i = 0; i < collection.Count; i++)
                {
                    int result = comparison.Invoke(collection[i], item);
                    if (result >= 1)
                    {
                        collection.Insert(i, item);
                        last = false;
                        break;
                    }
                }
                if (last)
                {
                    collection.Add(item);
                }
            }
        }

        public static void InvokeAdd<T>(this ObservableCollection<T> collection, T newItem)
        {
            collection.Shedule(() => collection.Add(newItem));
        }

        public static void InvokeAddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> newItems)
        {
            collection.Shedule(
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
            collection.Shedule(() => collection.Remove(oldItem));
        }

        public static void InvokeRemove<T>(this ObservableCollection<T> collection, IEnumerable<T> oldItems)
        {
            collection.Shedule(
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
            collection.Shedule(collection.Clear);
        }

        public static void Shedule<T>(this ObservableCollection<T> col, Action action)
        {
            Schedulers.DispatcherOrCurrentThread.Schedule(action);
        }

        public static DispatcherOperation AddAsync<T>(this ObservableCollection<T> collection, T newItem)
        {
            return collection.InvokeAsync(() => collection.Add(newItem));
        }

        public static DispatcherOperation AddRangeAsync<T>(
            this ObservableCollection<T> collection,
            IEnumerable<T> newItems)
        {
            return collection.InvokeAsync(
                () =>
                {
                    foreach (var newItem in newItems)
                    {
                        collection.Add(newItem);
                    }
                });
        }

        public static DispatcherOperation RemoveAsync<T>(this ObservableCollection<T> collection, T oldItem)
        {
            return collection.InvokeAsync(() => collection.Remove(oldItem));
        }

        public static Task ClearAsync<T>(this ObservableCollection<T> collection)
        {
            return collection.InvokeAsync(c => c.Clear());
        }

        public static async Task InvokeAsync<T>(this ObservableCollection<T> col, Action<ObservableCollection<T>> action)
        {
            var application = Application.Current;
            if (application != null)
            {
                await application.Dispatcher.InvokeAsync(() => action(col));
            }
            else
            {
                action(col);
            }
        }
    }
}
