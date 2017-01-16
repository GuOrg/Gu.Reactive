namespace Gu.Wpf.Reactive
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Windows;

    /// <summary>
    /// Helper for invoking on the dispatcher.
    /// </summary>
    public static class Schedulers
    {
        /// <summary>
        /// Observes on DispatcherScheduler.Current id not null
        /// Falls back to DispatcherScheduler.Current (for tests)
        /// </summary>
        /// <returns>The scheduler</returns>
        public static IScheduler DispatcherOrCurrentThread
        {
            get
            {
                var dispatcher = Application.Current?.Dispatcher;
                if (dispatcher == null)
                {
                    return Scheduler.CurrentThread;
                }

                return WpfSchedulers.Dispatcher;
            }
        }

        /// <summary>
        /// Observs on DispatcherScheduler.Current id not null
        /// Falls back to DispatcherScheduler.Current (for tests)
        /// </summary>
        public static IObservable<T> ObserveOnDispatcherOrCurrentThread<T>(this IObservable<T> observable)
        {
            return observable.ObserveOn(DispatcherOrCurrentThread);
        }
    }
}
