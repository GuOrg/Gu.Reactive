﻿namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Demo.Annotations;
    using Gu.Wpf.Reactive;

    public class FilteredViewViewModel : INotifyPropertyChanged
    {
        private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
        private readonly ObservableCollection<Person> _peopleRaw;
        private readonly Random _random = new Random();
        
        private string _searchText;
        private bool _hasSearchText;
        private IEnumerable<int> _selectedTags = Enumerable.Empty<int>();

        public FilteredViewViewModel()
        {
            Tags = new HashSet<int>(Enumerable.Range(0, 10));
            _peopleRaw = new ObservableCollection<Person>();
            int n = 1000;
            for (int i = 0; i < n / 3; i++)
            {
                _peopleRaw.Add(new Person { FirstName = "Johan", LastName = "Larsson", TagsValues = CreateTags() });
                _peopleRaw.Add(new Person { FirstName = "Max", LastName = "Andersson", TagsValues = CreateTags() });
                _peopleRaw.Add(new Person { FirstName = "Erik", LastName = "Svensson", TagsValues = CreateTags() });
            }
            Filtered = new FilteredView<Person>(
                _peopleRaw,
                Filter,
                this.ObservePropertyChanged(x => x.SearchText),
                this.ObservePropertyChanged(x => x.SelectedTags));
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
    }
}
