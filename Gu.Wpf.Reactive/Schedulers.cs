namespace Gu.Wpf.Reactive
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Windows;

    public static class Schedulers
    {
        /// <summary>
        /// Observs on DispatcherScheduler.Current id not null
        /// Falls back to DispatcherScheduler.Current (for tests)
        /// </summary>
        /// <returns>The scheduler</returns>
        [Obsolete("Use IWpfScedulers instead")]
        public static IScheduler DispatcherOrCurrentThread
        {
            get
            {
                var application = Application.Current;
                if (application == null || application.Dispatcher == null)
                {
                    return Scheduler.CurrentThread;
                }
                return new DispatcherScheduler(application.Dispatcher);
            }
        }

        /// <summary>
        /// Observs on DispatcherScheduler.Current id not null
        /// Falls back to DispatcherScheduler.Current (for tests)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="observable"></param>
        /// <returns></returns>
        [Obsolete("Use IWpfScedulers instead")]
        public static IObservable<T> ObserveOnDispatcherOrCurrentThread<T>(this IObservable<T> observable)
        {
            return observable.ObserveOn(DispatcherOrCurrentThread);
        }
    }
}
