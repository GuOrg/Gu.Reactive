#pragma warning disable 618
namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Gu.Wpf.Reactive;

    public sealed class ThrottledViewViewModel : IDisposable
    {
        private readonly ObservableCollection<DummyItem> observableCollection = new ObservableCollection<DummyItem>();
        private bool disposed;

        public ThrottledViewViewModel()
        {
            this.DeferTime = TimeSpan.FromMilliseconds(10);
            this.Add(3);
            this.ReadOnlyObservableCollection = new ReadOnlyObservableCollection<DummyItem>(this.observableCollection);
            this.ThrottledView = this.observableCollection.AsThrottledView(this.DeferTime);
            this.ReadOnlyThrottledView = this.observableCollection.AsReadOnlyThrottledView(this.DeferTime, WpfSchedulers.Dispatcher);
            this.ReadOnlyIListThrottledView = this.ReadOnlyThrottledView.AsReadonlyIListView();
            this.AddOneCommand = new RelayCommand(this.AddOne, () => true);
            this.AddOneToViewCommand = new RelayCommand(this.AddOneToView, () => true);
            this.AddTenCommand = new RelayCommand(this.AddTen, () => true);
            this.AddOneOnOtherThreadCommand = new RelayCommand(() => Task.Run(() => this.AddOne()), () => true);
        }

        public ReadOnlyObservableCollection<DummyItem> ReadOnlyObservableCollection { get; }

        public IThrottledView<DummyItem> ThrottledView { get; }

        public IReadOnlyObservableCollection<DummyItem> ReadOnlyThrottledView { get; }

        public IReadOnlyObservableCollection<DummyItem> ReadOnlyIListThrottledView { get; }

        public TimeSpan DeferTime { get; }

        public ICommand AddOneCommand { get; }

        public ICommand AddOneToViewCommand { get; }

        public ICommand AddTenCommand { get; }

        public ICommand AddOneOnOtherThreadCommand { get; }

        public ObservableCollection<DummyItem> ObservableCollection => this.observableCollection;

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.ThrottledView.Dispose();
            (this.ReadOnlyThrottledView as IDisposable)?.Dispose();
            (this.ReadOnlyIListThrottledView as IDisposable)?.Dispose();
        }

        private void AddOne()
        {
            this.observableCollection.Add(new DummyItem(this.observableCollection.Count + 1));
        }

        private void AddOneToView()
        {
            this.ThrottledView.Add(new DummyItem(this.observableCollection.Count + 1));
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
    }
}
