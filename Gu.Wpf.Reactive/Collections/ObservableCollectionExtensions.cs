namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reactive.Concurrency;
    using System.Threading.Tasks;
    using System.Windows;

    using Gu.Reactive.Internals;

    /// <summary>
    /// Extension methods for <see cref="ObservableCollection{T}"/>
    /// </summary>
    public static class ObservableCollectionExtensions
    {
        private static readonly Task CompletedTask = Task.FromResult(default(VoidTypeStruct)); // Task.CompletedTask is internal
        private static readonly Task<bool> CompletedTrueTask = Task.FromResult(true);
        private static readonly Task<bool> CompletedFalseTask = Task.FromResult(false);

        /// <summary>
        /// Insert <paramref name="item"/> sorted in <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of elements in <paramref name="collection"/></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="item">The item to add.</param>
        /// <param name="comparison">The comparison</param>
        public static void InvokeInsertSorted<T>(this ObservableCollection<T> collection, T item, Comparison<T> comparison = null)
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
                var last = true;
                for (var i = 0; i < collection.Count; i++)
                {
                    var result = comparison.Invoke(collection[i], item);
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

        /// <summary>
        /// Add <paramref name="item"/> on the dispatcher.
        /// </summary>
        /// <typeparam name="T">The type of elements in <paramref name="collection"/></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="item">The item to add.</param>
        public static void InvokeAdd<T>(this ObservableCollection<T> collection, T item)
        {
            Shedule(() => collection.Add(item));
        }

        /// <summary>
        /// Add <paramref name="items"/> on the dispatcher.
        /// </summary>
        /// <typeparam name="T">The type of elements in <paramref name="collection"/></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="items">The items to add.</param>
        public static void InvokeAddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            Shedule(
                () =>
                {
                    foreach (var newItem in items)
                    {
                        collection.Add(newItem);
                    }
                });
        }

        /// <summary>
        /// Remove <paramref name="item"/> on the dispatcher.
        /// </summary>
        /// <typeparam name="T">The type of elements in <paramref name="collection"/></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="item">The item to remove.</param>
        public static void InvokeRemove<T>(this ObservableCollection<T> collection, T item)
        {
            Shedule(() => collection.Remove(item));
        }

        /// <summary>
        /// Remove <paramref name="items"/> on the dispatcher.
        /// </summary>
        /// <typeparam name="T">The type of elements in <paramref name="collection"/></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="items">The items to remove.</param>
        public static void InvokeRemoveRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            Shedule(
                () =>
                {
                    foreach (var oldItem in items)
                    {
                        collection.Remove(oldItem);
                    }
                });
        }

        /// <summary>
        /// Clear <paramref name="collection"/> on the dispatcher.
        /// </summary>
        /// <typeparam name="T">The type of elements in <paramref name="collection"/></typeparam>
        /// <param name="collection">The collection.</param>
        public static void InvokeClear<T>(this ObservableCollection<T> collection)
        {
            Shedule(collection.Clear);
        }

        /// <summary>
        /// Add <paramref name="item"/> on the dispatcher.
        /// </summary>
        /// <typeparam name="T">The type of elements in <paramref name="collection"/></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="item">The item to add.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task AddAsync<T>(this ObservableCollection<T> collection, T item)
        {
            return InvokeAsync(() => collection.Add(item));
        }

        /// <summary>
        /// Add <paramref name="items"/> on the dispatcher.
        /// </summary>
        /// <typeparam name="T">The type of elements in <paramref name="collection"/></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="items">The items to add.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task AddRangeAsync<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            return InvokeAsync(
                () =>
                {
                    foreach (var newItem in items)
                    {
                        collection.Add(newItem);
                    }
                });
        }

        /// <summary>
        /// Remove <paramref name="item"/> on the dispatcher.
        /// </summary>
        /// <typeparam name="T">The type of elements in <paramref name="collection"/></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="item">The item to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task<bool> RemoveAsync<T>(this ObservableCollection<T> collection, T item)
        {
            return InvokeAsyncResult(() => collection.Remove(item));
        }

        /// <summary>
        /// Clear <paramref name="collection"/> on the dispatcher.
        /// </summary>
        public static Task ClearAsync<T>(this ObservableCollection<T> collection)
        {
            return InvokeAsync(collection.Clear);
        }

        private static void Shedule(Action action)
        {
            Schedulers.DispatcherOrCurrentThread.Schedule(action);
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
