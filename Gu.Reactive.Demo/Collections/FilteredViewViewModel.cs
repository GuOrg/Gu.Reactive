#pragma warning disable 618
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
    using Gu.Wpf.Reactive;

    public sealed class FilteredViewViewModel : INotifyPropertyChanged, IDisposable
    {
        private static readonly IReadOnlyList<string> FirstNames = new[] { "Erik", "Johan", "Max", "Lynn", "Markus" };
        private static readonly IReadOnlyList<string> LastNames = new[] { "Larsson", "Svensson", "Skeet", "Andersson" };
        private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
        private readonly ObservableCollection<Person> peopleRaw;
        private readonly Random random = new Random();
        private readonly System.Reactive.Disposables.CompositeDisposable disposable;
        private string searchText;
        private bool hasSearchText;
        private IEnumerable<int> selectedTags = Enumerable.Empty<int>();
        private int numberOfItems = 100;
        private bool disposed;

        public FilteredViewViewModel()
        {
            this.Tags = new HashSet<int>(Enumerable.Range(0, 10));
            this.peopleRaw = new ObservableCollection<Person>();

            this.Filtered = this.peopleRaw.AsFilteredView(
                this.Filter,
                TimeSpan.FromMilliseconds(10),
                this.ObservePropertyChanged(x => x.SearchText),
                this.ObservePropertyChanged(x => x.SelectedTags));

            this.ReadOnlyFiltered = this.peopleRaw.AsReadOnlyFilteredView(
                this.Filter,
                TimeSpan.FromMilliseconds(10),
                WpfSchedulers.Dispatcher,
                this.ObservePropertyChanged(x => x.SearchText),
                this.ObservePropertyChanged(x => x.SelectedTags));
            this.PeopleRaw = this.peopleRaw.AsDispatchingView();
            this.AddOneOnOtherThread = new AsyncCommand(() => Task.Run(() => this.AddOne()));
            this.disposable = new System.Reactive.Disposables.CompositeDisposable
                              {
                                  this.ObservePropertyChanged(x => x.NumberOfItems)
                                      .Subscribe(_ => this.UpdateRawCollection()),
                                  this.ObservePropertyChanged(x => x.SearchText)
                                      .Subscribe(_ => this.HasSearchText = !string.IsNullOrEmpty(this.SearchText)),
                              };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public AsyncCommand AddOneOnOtherThread { get; }

        public IReadOnlyObservableCollection<Person> PeopleRaw { get; }

        public IEnumerable<int> Tags { get; }

        public FilteredView<Person> Filtered { get; }

        public IReadOnlyObservableCollection<Person> ReadOnlyFiltered { get; }

        public bool HasSearchText
        {
            get => this.hasSearchText;

            private set
            {
                if (value == this.hasSearchText)
                {
                    return;
                }

                this.hasSearchText = value;
                this.OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => this.searchText;

            set
            {
                if (value == this.searchText)
                {
                    return;
                }

                this.searchText = value;
                this.OnPropertyChanged();
            }
        }

        public IEnumerable<int> SelectedTags
        {
            get => this.selectedTags;

            set
            {
                if (Equals(value, this.selectedTags))
                {
                    return;
                }

                this.selectedTags = value ?? Enumerable.Empty<int>();
                this.OnPropertyChanged();
            }
        }

        public int NumberOfItems
        {
            get => this.numberOfItems;

            set
            {
                if (value == this.numberOfItems)
                {
                    return;
                }

                this.numberOfItems = value;
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
            this.AddOneOnOtherThread.Dispose();
            (this.PeopleRaw as IDisposable)?.Dispose();
            this.Filtered.Dispose();
            (this.ReadOnlyFiltered as IDisposable)?.Dispose();
            this.disposable.Dispose();
        }

        private static bool IsMatch(string value, string pattern)
        {
            if (value is null)
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

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool Filter(Person person)
        {
            return this.IsTextMatch(person) && this.IsTagMatch(person);
        }

        private IReadOnlyList<int> CreateTags()
        {
            var tags = Enumerable.Repeat(0, this.random.Next(0, 3))
                                 .Select(_ => this.random.Next(0, 10))
                                 .Distinct()
                                 .ToArray();
            return tags;
        }

        private bool IsTextMatch(Person person)
        {
            if (string.IsNullOrEmpty(this.SearchText))
            {
                return true;
            }

            if (IsMatch(person.FirstName, this.SearchText))
            {
                return true;
            }

            if (IsMatch(person.LastName, this.SearchText))
            {
                return true;
            }

            return false;
        }

        private bool IsTagMatch(Person person)
        {
            if (!this.selectedTags.Any())
            {
                return true;
            }

            if (person.TagsValues is null || !person.TagsValues.Any())
            {
                return true;
            }

            return this.selectedTags.Intersect(person.TagsValues).Any();
        }

        private void UpdateRawCollection()
        {
            this.peopleRaw.Clear();
            for (var i = 0; i < this.NumberOfItems; i++)
            {
                this.AddOne();
            }
        }

        private void AddOne()
        {
            this.peopleRaw.Add(
                new Person
                {
                    FirstName = FirstNames[this.peopleRaw.Count % FirstNames.Count],
                    LastName = LastNames[this.peopleRaw.Count % LastNames.Count],
                    TagsValues = this.CreateTags(),
                });
        }
    }
}
