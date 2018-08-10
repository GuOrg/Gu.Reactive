#pragma warning disable 618
namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Data;

    using Gu.Wpf.Reactive;

    public sealed class CollectionViewDemoViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable disposable;
        private Func<int, bool> filter = x => true;
        private bool disposed;

        public CollectionViewDemoViewModel()
        {
            this.Enumerable = new[] { 1, 2, 3, 4, 5 };
            this.FilteredView1 = this.Enumerable.AsReadOnlyFilteredView(this.FilterMethod, TimeSpan.FromMilliseconds(10), WpfSchedulers.Dispatcher, this.ObservePropertyChanged(x => x.Filter));
            this.FilteredView2 = this.Enumerable.AsReadOnlyFilteredView(this.FilterMethod, TimeSpan.FromMilliseconds(10), WpfSchedulers.Dispatcher, this.ObservePropertyChanged(x => x.Filter));

            this.ObservableCollection = new ObservableCollection<int>(new[] { 1, 2, 3, 4, 5 });
            this.ObservableDefaultView = CollectionViewSource.GetDefaultView(this.ObservableCollection);
            this.ObservableFilteredView = this.ObservableCollection.AsFilteredView(this.Filter, TimeSpan.Zero);
            this.ThrottledFilteredView = this.ObservableCollection.AsFilteredView(this.Filter, TimeSpan.FromMilliseconds(10));
            this.disposable = this.ObservePropertyChanged(x => x.Filter, signalInitial: false)
                                  .Subscribe(
                                      x =>
                                      {
                                          _ = Application.Current.Dispatcher.Invoke(() => this.ObservableDefaultView.Filter = o => this.Filter((int)o));
                                          this.ObservableFilteredView.Filter = this.Filter;
                                          this.ThrottledFilteredView.Filter = this.Filter;
                                      });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<int> Enumerable { get; }

        public ReadOnlyFilteredView<int> FilteredView1 { get; }

        public ReadOnlyFilteredView<int> FilteredView2 { get; }

        public ObservableCollection<int> ObservableCollection { get; }

        public ICollectionView ObservableDefaultView { get; }

        public IFilteredView<int> ObservableFilteredView { get; }

        public IFilteredView<int> ThrottledFilteredView { get; }

        public Func<int, bool> Filter
        {
            get => this.filter;

            set
            {
                if (Equals(value, this.filter))
                {
                    return;
                }

                this.filter = value;
                this.OnPropertyChanged();
            }
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.FilteredView1.Dispose();
            this.FilteredView2.Dispose();
            this.ObservableFilteredView.Dispose();
            this.ThrottledFilteredView.Dispose();
            this.disposable?.Dispose();
        }

        private bool FilterMethod(int value)
        {
            return this.Filter(value);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
