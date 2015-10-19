namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using Gu.Reactive.Demo.Annotations;
    using Gu.Wpf.Reactive;

    public class FilteredViewViewModel : INotifyPropertyChanged
    {
        private static readonly IReadOnlyList<string> FirstNames = new[] { "Erik", "Johan", "Max", "Lynn", "Markus" };
        private static readonly IReadOnlyList<string> LastNames = new[] { "Larsson", "Svensson", "Skeet", "Andersson" };
        private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
        private readonly ObservableCollection<Person> _peopleRaw;
        private readonly Random _random = new Random();

        private string _searchText;
        private bool _hasSearchText;
        private IEnumerable<int> _selectedTags = Enumerable.Empty<int>();
        private int _numberOfItems = 100;

        public FilteredViewViewModel()
        {
            Tags = new HashSet<int>(Enumerable.Range(0, 10));
            _peopleRaw = new ObservableCollection<Person>();
            this.ObservePropertyChanged(x => x.NumberOfItems)
                .Subscribe(_ => UpdateRawCollection());
            Filtered = _peopleRaw.AsFilteredView(
                Filter,
                TimeSpan.FromMilliseconds(10),
                 WpfSchedulers.Dispatcher,
                this.ObservePropertyChanged(x => x.SearchText),
                this.ObservePropertyChanged(x => x.SelectedTags));


            ReadOnlyFiltered = _peopleRaw.AsReadOnlyFilteredView(
                Filter,
                TimeSpan.FromMilliseconds(10),
                 WpfSchedulers.Dispatcher,
                this.ObservePropertyChanged(x => x.SearchText),
                this.ObservePropertyChanged(x => x.SelectedTags));
            PeopleRaw = _peopleRaw.AsDispatchingView();
            this.ObservePropertyChanged(x => x.SearchText)
                .Subscribe(_ => HasSearchText = !string.IsNullOrEmpty(SearchText));
            AddOneOnOtherThread = new AsyncCommand(() => Task.Run(() => AddOne()));
        }

        public AsyncCommand AddOneOnOtherThread { get; }

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

        public IReadOnlyObservableCollection<Person> PeopleRaw { get; }

        public IEnumerable<int> Tags { get; private set; }

        public IEnumerable<int> SelectedTags
        {
            get { return _selectedTags; }
            set
            {
                if (Equals(value, _selectedTags)) return;
                _selectedTags = value ?? Enumerable.Empty<int>();
                OnPropertyChanged();
            }
        }

        public FilteredView<Person> Filtered { get; }

        public IReadOnlyObservableCollection<Person> ReadOnlyFiltered { get; }

        public int NumberOfItems
        {
            get { return _numberOfItems; }
            set
            {
                if (value == _numberOfItems)
                {
                    return;
                }
                _numberOfItems = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool Filter(Person person)
        {
            return IsTextMatch(person) && IsTagMatch(person);
        }

        private bool IsTextMatch(Person person)
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

        private bool IsTagMatch(Person person)
        {
            if (!_selectedTags.Any())
            {
                return true;
            }
            if (person.TagsValues == null || !person.TagsValues.Any())
            {
                return true;
            }
            return _selectedTags.Intersect(person.TagsValues).Any();
        }

        private IReadOnlyList<int> CreateTags()
        {
            var tags = Enumerable.Repeat(0, _random.Next(0, 3))
                                 .Select(_ => _random.Next(0, 10))
                                 .Distinct()
                                 .ToArray();
            return tags;
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

        private void UpdateRawCollection()
        {
            _peopleRaw.Clear();
            for (int i = 0; i < NumberOfItems; i++)
            {
                AddOne();
            }
        }

        private void AddOne()
        {
            _peopleRaw.Add(
                new Person
                {
                    FirstName = FirstNames[_peopleRaw.Count % FirstNames.Count],
                    LastName = LastNames[_peopleRaw.Count % LastNames.Count],
                    TagsValues = CreateTags()
                });
        }
    }
}
