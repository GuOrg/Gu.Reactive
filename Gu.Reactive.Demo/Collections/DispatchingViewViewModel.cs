namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Gu.Wpf.Reactive;

    public sealed class DispatchingViewViewModel : IDisposable
    {
        private bool disposed;

        public DispatchingViewViewModel()
        {
            this.Add(3);
            this.ReadOnlyObservableCollection = new ReadOnlyObservableCollection<DummyItem>(this.ObservableCollection);
            this.DispatchingView = this.ObservableCollection.AsDispatchingView();
            this.AddOneCommand = new RelayCommand(this.AddOne, () => true);
            this.AddOneToViewCommand = new RelayCommand(this.AddOneToView, () => true);
            this.AddTenCommand = new RelayCommand(this.AddTen, () => true);
            this.AddOneOnOtherThreadCommand = new RelayCommand(() => Task.Run(() => this.AddOne()), () => true);
            this.ClearCommand = new RelayCommand(() => this.Clear(), () => true);
            this.ObservableCollection
                .ObserveCollectionChanged()
                .ObserveOnDispatcher()
                .Subscribe(x => this.ObservableCollectionChanges.Add(x.EventArgs));

            this.DispatchingView
                .ObserveCollectionChanged()
                .ObserveOnDispatcher()
                .Subscribe(x => this.DispatchingChanges.Add(x.EventArgs));
        }

        public ReadOnlyObservableCollection<DummyItem> ReadOnlyObservableCollection { get; }

        public IObservableCollection<DummyItem> DispatchingView { get; }

        public ICommand AddOneCommand { get; }

        public ICommand AddOneToViewCommand { get; }

        public ICommand AddTenCommand { get; }

        public ICommand AddOneOnOtherThreadCommand { get; }

        public RelayCommand ClearCommand { get; }

        public ObservableCollection<NotifyCollectionChangedEventArgs> ObservableCollectionChanges { get; } = new ObservableCollection<NotifyCollectionChangedEventArgs>();

        public ObservableCollection<NotifyCollectionChangedEventArgs> DispatchingChanges { get; } = new ObservableCollection<NotifyCollectionChangedEventArgs>();

        public ObservableCollection<DummyItem> ObservableCollection { get; } = new ObservableCollection<DummyItem>();

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            (this.DispatchingView as IDisposable)?.Dispose();
        }

        private void AddOne()
        {
            lock (((ICollection)this.ObservableCollection).SyncRoot)
            {
                this.ObservableCollection.Add(new DummyItem(this.ObservableCollection.Count + 1));
            }
        }

        private void AddOneToView()
        {
            this.DispatchingView.Add(new DummyItem(this.ObservableCollection.Count + 1));
        }

        private void AddTen()
        {
            this.Add(10);
        }

        private void Add(int n)
        {
            for (int i = 0; i < n; i++)
            {
                this.AddOne();
            }
        }

        private void Clear()
        {
            this.ObservableCollection.Clear();
            this.ObservableCollectionChanges.Clear();
            this.DispatchingChanges.Clear();
        }
    }
}
