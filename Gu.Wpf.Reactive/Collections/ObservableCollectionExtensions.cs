namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Extension methods for <see cref="ObservableCollection{T}"/>.
    /// </summary>
    public static class ObservableCollectionExtensions
    {
        private static readonly Task CompletedTask = Task.FromResult(default(VoidTypeStruct)); // Task.CompletedTask is internal
        private static readonly Task<bool> CompletedTrueTask = Task.FromResult(true);
        private static readonly Task<bool> CompletedFalseTask = Task.FromResult(false);

        /// <summary>
        /// Insert <paramref name="item"/> sorted in <paramref name="collection"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in <paramref name="collection"/>.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="item">The item to add.</param>
        /// <param name="comparison">The comparison.</param>
        public static void InvokeInsertSorted<T>(this ObservableCollection<T> collection, T item, Comparison<T>? comparison = null)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (comparison is null)
            {
                comparison = Comparer<T>.Default.Compare;
            }

            if (collection.Count == 0)
            {
                Invoke(() => collection.Add(item));
            }
            else
            {
                var last = true;
                for (var i = 0; i < collection.Count; i++)
                {
                    var result = comparison.Invoke(collection[i], item);
                    if (result >= 1)
                    {
                        Invoke(() => collection.Insert(i, item));
                        last = false;
                        break;
                    }
                }

                if (last)
                {
                    Invoke(() => collection.Add(item));
                }
            }
        }

        /// <summary>
        /// Add <paramref name="item"/> on the dispatcher.
        /// </summary>
        /// <typeparam name="T">The type of elements in <paramref name="collection"/>.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="item">The item to add.</param>
        public static void InvokeAdd<T>(this ObservableCollection<T> collection, T item)
        {
            Invoke(() => collection.Add(item));
        }

        /// <summary>
        /// Add <paramref name="items"/> on the dispatcher.
        /// </summary>
        /// <typeparam name="T">The type of elements in <paramref name="collection"/>.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="items">The items to add.</param>
        public static void InvokeAddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            Invoke(
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
        /// <typeparam name="T">The type of elements in <paramref name="collection"/>.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="item">The item to remove.</param>
        public static void InvokeRemove<T>(this ObservableCollection<T> collection, T item)
        {
            Invoke(() => collection.Remove(item));
        }

        /// <summary>
        /// Remove <paramref name="items"/> on the dispatcher.
        /// </summary>
        /// <typeparam name="T">The type of elements in <paramref name="collection"/>.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="items">The items to remove.</param>
        public static void InvokeRemoveRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            Invoke(
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
        /// <typeparam name="T">The type of elements in <paramref name="collection"/>.</typeparam>
        /// <param name="collection">The collection.</param>
        public static void InvokeClear<T>(this ObservableCollection<T> collection)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            Invoke(collection.Clear);
        }

        /// <summary>
        /// Add <paramref name="item"/> on the dispatcher.
        /// </summary>
        /// <typeparam name="T">The type of elements in <paramref name="collection"/>.</typeparam>
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
        /// <typeparam name="T">The type of elements in <paramref name="collection"/>.</typeparam>
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
        /// <typeparam name="T">The type of elements in <paramref name="collection"/>.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="item">The item to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task<bool> RemoveAsync<T>(this ObservableCollection<T> collection, T item)
        {
            return InvokeAsync(() => collection.Remove(item));
        }

        /// <summary>
        /// Clear <paramref name="collection"/> on the dispatcher.
        /// </summary>
        public static Task ClearAsync<T>(this ObservableCollection<T> collection)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            return InvokeAsync(collection.Clear);
        }

        private static void Invoke(Action action)
        {
            var application = Application.Current;
            if (application != null)
            {
                application.Dispatcher.Invoke(action);
                return;
            }

            action();
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

        private static Task<bool> InvokeAsync(Func<bool> action)
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
