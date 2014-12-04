namespace Gu.Wpf.Reactive
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Windows;

    /// <summary>
    /// 
    /// </summary>
    public static class Schedulers
    {
        /// <summary>
        /// Observs on DispatcherScheduler.Current id not null
        /// Falls back to DispatcherScheduler.Current (for tests)
        /// </summary>
        /// <returns>The scheduler</returns>
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
        public static IObservable<T> ObserveOnDispatcherOrCurrentThread<T>(this IObservable<T> observable)
        {
            return observable.ObserveOn(DispatcherOrCurrentThread);
        }
    }
}
