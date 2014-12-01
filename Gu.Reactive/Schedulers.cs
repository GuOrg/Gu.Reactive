namespace Gu.Reactive
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading;

    public static class Schedulers
    {
        /// <summary>
        /// Observs on SynchronizationContext.Current if present.
        /// Falls back to Scheduler.Immediate (for tests)
        /// </summary>
        /// <returns>The scheduler</returns>
        public static IScheduler SynchronizationContextOrImmediate
        {
            get
            {
                var scheduler = SynchronizationContext.Current != null
                    ? (IScheduler)new SynchronizationContextScheduler(SynchronizationContext.Current)
                    : Scheduler.Immediate;
                return scheduler;
            }
        }

        /// <summary>
        /// Observs on SynchronizationContext.Current if present.
        /// Falls back to Scheduler.Immediate (for tests)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="observable"></param>
        /// <returns></returns>
        public static IObservable<T> ObserveOnSynchronizationContextOrImmediate<T>(this IObservable<T> observable)
        {
            return observable.ObserveOn(SynchronizationContextOrImmediate);
        }
    }
}
