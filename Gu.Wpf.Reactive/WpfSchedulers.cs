namespace Gu.Wpf.Reactive
{
    using System.Reactive.Concurrency;
    using System.Windows;

    using Gu.Reactive;

    // ReSharper disable once ClassNeverInstantiated.Global
    public class WpfSchedulers : Gu.Reactive.Schedulers, IWpfSchedulers
    {
        public static readonly DispatcherScheduler Dispatcher = new DispatcherScheduler(Application.Current.Dispatcher);

        IScheduler IWpfSchedulers.Dispatcher => Dispatcher;
    }
}
