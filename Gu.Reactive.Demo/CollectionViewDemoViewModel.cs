namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Data;

    using Gu.Reactive.Demo.Annotations;
    using Gu.Wpf.Reactive;

    public class CollectionViewDemoViewModel : INotifyPropertyChanged
    {
        private Predicate<int> _filter;

        public CollectionViewDemoViewModel()
        {
            Enumerable = new[] { 1, 2, 3, 4, 5 };
            CollectionView = new CollectionView(new[] { 1, 2, 3, 4, 5 });
            TypedCollectionView = CollectionView<int>.Create(new[] { 1, 2, 3, 4, 5 });
            TypedCollectionViewAsInterface = CollectionView<int>.Create(new[] { 1, 2, 3, 4, 5 });
            ObservableCollection = new ObservableCollection<int>(new[] { 1, 2, 3, 4, 5 });
            ObservableDefaultView = CollectionViewSource.GetDefaultView(ObservableCollection);
            TypedObservableCollectionView = CollectionView<int>.Create(ObservableCollection);
            this.ToObservable(x => x.Filter, false)
                .Subscribe(
                    x =>
                    {
                        TypedCollectionView.Filter = Filter;
                        //CollectionView.Filter = o => Filter((int)o);
                        //ObservableDefaultView.Filter = o => Filter((int)o);
                        //TypedObservableCollectionView.Filter = Filter;
                    });
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<int> Enumerable { get; private set; }

        public CollectionView CollectionView { get; private set; }

        public CollectionView<int> TypedCollectionView { get; private set; }
        
        public ICollectionView TypedCollectionViewAsInterface { get; private set; }
        
        public ObservableCollection<int> ObservableCollection { get; private set; }

        public ICollectionView ObservableDefaultView { get; private set; }

        public CollectionView<int> TypedObservableCollectionView { get; private set; }

        public Predicate<int> Filter
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
