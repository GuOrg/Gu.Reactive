namespace Gu.Wpf.Reactive
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Windows;

    /// <summary>
    /// Helper for invoking on the dispatcher.
    /// </summary>
    [Obsolete("This will be removed in future version.")]
    public static class Schedulers
    {
        /// <summary>
        /// Gets observes on DispatcherScheduler.Current if not null
        /// Falls back to DispatcherScheduler.Current (for tests).
        /// </summary>
        /// <returns>The scheduler.</returns>
        public static IScheduler DispatcherOrCurrentThread
        {
            get
            {
                var dispatcher = Application.Current?.Dispatcher;
                if (dispatcher is null)
                {
                    return Scheduler.CurrentThread;
                }

                return WpfSchedulers.Dispatcher;
            }
        }

        /// <summary>
        /// Observes on DispatcherScheduler.Current id not null
        /// Falls back to DispatcherScheduler.Current (for tests).
        /// </summary>
        /// <typeparam name="T">The type of the items in the observable.</typeparam>
        /// <param name="observable">The <see cref="IObservable{T}"/>.</param>
        /// <returns>An <see cref="IObservable{T}"/>.</returns>
        public static IObservable<T> ObserveOnDispatcherOrCurrentThread<T>(this IObservable<T> observable)
        {
            return observable.ObserveOn(DispatcherOrCurrentThread);
        }
    }
}
