namespace Gu.Wpf.Reactive
{
    using System;
    using System.Reactive.Concurrency;
    using System.Windows;

    using Gu.Reactive;

    // ReSharper disable once ClassNeverInstantiated.Global
    public class WpfSchedulers : Gu.Reactive.Schedulers, IWpfSchedulers
    {
        private static readonly Lazy<DispatcherScheduler> LazyDispatcher = new Lazy<DispatcherScheduler>(() => new DispatcherScheduler(Application.Current.Dispatcher));

        public static DispatcherScheduler Dispatcher => LazyDispatcher.Value;

        IScheduler IWpfSchedulers.Dispatcher => Dispatcher;
    }
}
