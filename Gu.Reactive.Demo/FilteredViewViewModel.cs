namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Demo.Annotations;
    using Gu.Wpf.Reactive;

    public class FilteredViewViewModel : INotifyPropertyChanged
    {
        private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
        private readonly ObservableCollection<Person> _peopleRaw;
        private string _searchText;

        private bool _hasSearchText;

        public FilteredViewViewModel()
        {
            _peopleRaw = new ObservableCollection<Person>
                             {
                                 new Person { FirstName = "Johan", LastName = "Larsson" },
                                 new Person { FirstName = "Max", LastName = "Andersson" },
                                 new Person { FirstName = "Erik", LastName = "Svensson" },
                             };
            Filtered = new FilteredView<Person>(_peopleRaw, Filter, this.ObservePropertyChanged(x => x.SearchText));
            PeopleReadonly = new ReadOnlyObservableCollection<Person>(_peopleRaw);
            DummyReadonlyCollection = new DummyReadonlyCollection<Person>(_peopleRaw);
            this.ObservePropertyChanged(x => x.SearchText)
                .Subscribe(_ => HasSearchText = !string.IsNullOrEmpty(SearchText));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (value == _searchText)
                {
                    return;
                }
                _searchText = value;
                OnPropertyChanged();
            }
        }

        public bool HasSearchText
        {
            get { return _hasSearchText; }
            private set
            {
                if (value == _hasSearchText)
                {
                    return;
                }
                _hasSearchText = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Person> PeopleRaw
        {
            get { return _peopleRaw; }
        }

        public ReadOnlyObservableCollection<Person> PeopleReadonly { get; private set; }

        public FilteredView<Person> Filtered { get; private set; }

        public DummyReadonlyCollection<Person> DummyReadonlyCollection { get; private set; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool Filter(Person person)
        {

            if (string.IsNullOrEmpty(SearchText))
            {
                return true;
            }

            if (IsMatch(person.FirstName, SearchText))
            {
                return true;
            }

            if (IsMatch(person.LastName, SearchText))
            {
                return true;
            }

            return false;
        }

        private static bool IsMatch(string value, string pattern)
        {
            if (value == null)
            {
                return false;
            }
            var indexOf = InvariantCulture.CompareInfo.IndexOf(value, pattern, CompareOptions.OrdinalIgnoreCase);
            if (pattern.Length == 1)
            {
                return indexOf == 0;
            }
            return indexOf >= 0;
        }
    }
}
