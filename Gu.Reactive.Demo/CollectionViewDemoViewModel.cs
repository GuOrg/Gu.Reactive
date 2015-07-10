namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Reactive.Concurrency;
    using System.Runtime.CompilerServices;
    using System.Windows.Data;

    using Annotations;

    using Wpf.Reactive;

    public class CollectionViewDemoViewModel : INotifyPropertyChanged
    {
        private Func<int, bool> _filter = x => true;

        public CollectionViewDemoViewModel()
        {
            Enumerable = new[] { 1, 2, 3, 4, 5 };
            FilteredView1 = Enumerable.AsFilteredView(FilterMethod, TimeSpan.FromMilliseconds(10), Schedulers.DispatcherOrCurrentThread, this.ObservePropertyChanged(x => x.Filter));
            FilteredView2 = Enumerable.AsFilteredView(FilterMethod, TimeSpan.FromMilliseconds(10), Schedulers.DispatcherOrCurrentThread, this.ObservePropertyChanged(x => x.Filter));

            ObservableCollection = new ObservableCollection<int>(new[] { 1, 2, 3, 4, 5 });
            ObservableDefaultView = CollectionViewSource.GetDefaultView(ObservableCollection);
            ObservableFilteredView = ObservableCollection.AsFilteredView(Filter, TimeSpan.Zero, Schedulers.DispatcherOrCurrentThread);
            DeferredObservableFilteredView = ObservableCollection.AsFilteredView(Filter, TimeSpan.FromMilliseconds(10), Schedulers.DispatcherOrCurrentThread);

            this.ObservePropertyChanged(x => x.Filter, false)
                .Subscribe(
                    x =>
                    {
                        Schedulers.DispatcherOrCurrentThread.Schedule(() => ObservableDefaultView.Filter = o => Filter((int)o));
                        ObservableFilteredView.Filter = Filter;
                        DeferredObservableFilteredView.Filter = Filter;
                    });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<int> Enumerable { get; private set; }

        public ReadOnlyFilteredView<int> FilteredView1 { get; private set; }

        public ReadOnlyFilteredView<int> FilteredView2 { get; private set; }

        public ObservableCollection<int> ObservableCollection { get; private set; }

        public ICollectionView ObservableDefaultView { get; private set; }

        public IFilteredView<int> ObservableFilteredView { get; private set; }

        public IFilteredView<int> DeferredObservableFilteredView { get; private set; }

        public Func<int, bool> Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                if (Equals(value, _filter))
                {
                    return;
                }
                _filter = value;
                OnPropertyChanged();
            }
        }

        private bool FilterMethod(int value)
        {
            return Filter(value);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
