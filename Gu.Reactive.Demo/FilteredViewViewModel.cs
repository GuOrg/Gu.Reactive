namespace Gu.Reactive.Demo
{
    using System.Collections.ObjectModel;

    using Gu.Wpf.Reactive;

    public class FilteredViewViewModel
    {
        private readonly ObservableCollection<Person> _peopleRaw;

        public FilteredViewViewModel()
        {
            _peopleRaw = new ObservableCollection<Person>
                             {
                                 new Person { FirstName = "Johan", LastName = "Larsson" },
                                 new Person { FirstName = "Max", LastName = "Andersson" },
                                 new Person { FirstName = "Erik", LastName = "Svensson" },
                             };
            Filtered = new FilteredView<Person>(_peopleRaw);
            PeopleReadonly = new ReadOnlyObservableCollection<Person>(_peopleRaw);
            DummyReadonlyCollection = new DummyReadonlyCollection<Person>(_peopleRaw);
        }

        public ObservableCollection<Person> PeopleRaw
        {
            get { return _peopleRaw; }
        }

        public ReadOnlyObservableCollection<Person> PeopleReadonly { get; private set; }

        public FilteredView<Person> Filtered { get; private set; }

        public DummyReadonlyCollection<Person> DummyReadonlyCollection { get; private set; }
    }
}
