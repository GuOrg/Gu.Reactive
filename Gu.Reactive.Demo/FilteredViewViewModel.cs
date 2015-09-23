﻿namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    using Gu.Reactive.Demo.Annotations;
    using Gu.Wpf.Reactive;

    public class FilteredViewViewModel : INotifyPropertyChanged
    {
        private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
        private readonly ObservableCollection<Person> _peopleRaw;
        private readonly Random _random = new Random();
        private readonly ObservableCollection<NotifyCollectionChangedEventArgs> _rawEvents = new ObservableCollection<NotifyCollectionChangedEventArgs>();
        private readonly ObservableCollection<NotifyCollectionChangedEventArgs> _filteredEvents = new ObservableCollection<NotifyCollectionChangedEventArgs>();

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
                Schedulers.DispatcherOrCurrentThread,
                this.ObservePropertyChanged(x => x.SearchText),
                this.ObservePropertyChanged(x => x.SelectedTags));
            PeopleReadonly = new ReadOnlyObservableCollection<Person>(_peopleRaw);
            DummyReadonlyCollection = new DummyReadonlyCollection<Person>(_peopleRaw);
            this.ObservePropertyChanged(x => x.SearchText)
                .Subscribe(_ => HasSearchText = !string.IsNullOrEmpty(SearchText));

            PeopleRaw.ObserveCollectionChanged()
                    .ObserveOnDispatcherOrCurrentThread()
                    .Subscribe(x => _rawEvents.Add(x.EventArgs));

            Filtered.ObserveCollectionChanged()
                    .ObserveOnDispatcherOrCurrentThread()
                    .Subscribe(x => _filteredEvents.Add(x.EventArgs));
            ClearEventsCommand = new RelayCommand(
                () =>
                {
                    _filteredEvents.Clear();
                    _rawEvents.Clear();
                });
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

        public ICommand ClearEventsCommand { get; private set; }

        public ObservableCollection<Person> PeopleRaw
        {
            get { return _peopleRaw; }
        }

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

        public ReadOnlyObservableCollection<Person> PeopleReadonly { get; private set; }

        public FilteredView<Person> Filtered { get; private set; }

        public ObservableCollection<NotifyCollectionChangedEventArgs> RawEvents
        {
            get { return _rawEvents; }
        }

        public ObservableCollection<NotifyCollectionChangedEventArgs> FilteredEvents
        {
            get { return _filteredEvents; }
        }

        public DummyReadonlyCollection<Person> DummyReadonlyCollection { get; private set; }

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
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
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
                if (i % 3 == 0)
                {
                    _peopleRaw.Add(new Person { FirstName = "Johan", LastName = "Larsson", TagsValues = CreateTags() });
                }
                else if (i % 3 == 1)
                {
                    _peopleRaw.Add(new Person { FirstName = "Max", LastName = "Andersson", TagsValues = CreateTags() });
                }
                else
                {
                    _peopleRaw.Add(new Person { FirstName = "Erik", LastName = "Svensson", TagsValues = CreateTags() });
                }
            }
        }
    }
}