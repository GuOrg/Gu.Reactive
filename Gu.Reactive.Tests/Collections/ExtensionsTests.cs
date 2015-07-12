namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Moq;

    using NUnit.Framework;

    /// <summary>
    /// This class is just to make sure all overloads compile
    /// </summary>
    public class ExtensionsTests
    {
        [Test]
        public void AsFilteredView()
        {
            var ints = new ObservableCollection<int>();
            ints.AsFilteredView(x => true);

            ints.AsThrottledView(TimeSpan.Zero)
                .AsFilteredView(x => true);
        }

        [Test]
        public void AsReadOnlyFilteredView()
        {
            var ints = new ObservableCollection<int>();
            ints.AsReadOnlyFilteredView(x => true);

            ints.AsMappingView(x => x.ToString())
                .AsReadOnlyFilteredView(x => true);

            ints.AsThrottledView(TimeSpan.Zero)
                .AsReadOnlyFilteredView(x => true);

            var readonlyInts = new ReadOnlyObservableCollection<int>(ints);
            readonlyInts.AsReadOnlyFilteredView(x => true);

            readonlyInts.AsMappingView(x => x.ToString())
                        .AsReadOnlyFilteredView(x => true);

            readonlyInts.AsReadOnlyThrottledView(TimeSpan.Zero)
                        .AsReadOnlyFilteredView(x => true);

            Enumerable.Range(0, 2)
                      .AsReadOnlyFilteredView(x => true, Mock.Of<IObservable<object>>());
        }

        [Test]
        public void AsThrottledView()
        {
            var ints = new ObservableCollection<int>();
            ints.AsThrottledView(TimeSpan.Zero);

            ints.AsFilteredView(x => true)
                .AsThrottledView(TimeSpan.Zero);
        }

        [Test]
        public void AsReadOnlyThrottledView()
        {
            var ints = new ObservableCollection<int>();
            ints.AsReadOnlyThrottledView(TimeSpan.Zero);

            ints.AsFilteredView(x => true)
                .AsReadOnlyThrottledView(TimeSpan.Zero);

            ints.AsMappingView(x => x.ToString())
                .AsReadOnlyThrottledView(TimeSpan.Zero);

            var readonlyInts = new ReadOnlyObservableCollection<int>(ints);
            readonlyInts.AsReadOnlyThrottledView(TimeSpan.Zero);

            readonlyInts.AsReadOnlyFilteredView(x => true)
                        .AsReadOnlyThrottledView(TimeSpan.Zero);

            readonlyInts.AsMappingView(x => x.ToString())
                        .AsReadOnlyThrottledView(TimeSpan.Zero);
        }

        [Test]
        public void AsMappingView()
        {
            var ints = new ObservableCollection<int>();
            ints.AsMappingView(x => x.ToString());

            ints.AsFilteredView(x => true)
                .AsMappingView(x => x.ToString());

            ints.AsThrottledView(TimeSpan.Zero)
                .AsMappingView(x => x.ToString());

            var readonlyInts = new ReadOnlyObservableCollection<int>(ints);
            readonlyInts.AsMappingView(x => x.ToString());

            readonlyInts.AsReadOnlyFilteredView(x => true)
                        .AsMappingView(x => x.ToString());

            readonlyInts.AsReadOnlyThrottledView(TimeSpan.Zero)
                        .AsMappingView(x => x.ToString());

        }
    }
}
