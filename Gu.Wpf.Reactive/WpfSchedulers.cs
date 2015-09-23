namespace Gu.Wpf.Reactive
{
    using System.Reactive.Concurrency;
    using System.Windows;

    using Gu.Reactive;

    public class WpfSchedulers : Gu.Reactive.Schedulers, IWpfSchedulers
    {
        private DispatcherScheduler _dispatcherScheduler;

        public IScheduler Dispatcher => _dispatcherScheduler
                                        ?? (_dispatcherScheduler = new DispatcherScheduler(Application.Current.Dispatcher));
    }
}
