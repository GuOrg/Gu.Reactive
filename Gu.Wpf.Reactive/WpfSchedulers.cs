namespace Gu.Wpf.Reactive
{
    using System;
    using System.Reactive.Concurrency;
    using System.Windows;

    using Gu.Reactive;

    /// <summary>
    /// <see cref="Gu.Reactive.Schedulers"/> with <see cref="Dispatcher"/>.
    /// </summary>
    //// ReSharper disable once ClassNeverInstantiated.Global
    public class WpfSchedulers : Gu.Reactive.Schedulers, IWpfSchedulers
    {
        private static readonly Lazy<DispatcherScheduler> LazyDispatcher = new Lazy<DispatcherScheduler>(() => new DispatcherScheduler(Application.Current.Dispatcher));

        /// <summary>
        /// The dispatcher scheduler for the current application.
        /// </summary>
        public static DispatcherScheduler Dispatcher => LazyDispatcher.Value;

        /// <inheritdoc/>
        IScheduler IWpfSchedulers.Dispatcher => Dispatcher;
    }
}
