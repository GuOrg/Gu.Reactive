namespace Gu.Wpf.Reactive.Tests.Collections.Views
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using Gu.Reactive;
    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// This class is just to make sure all overloads compile.
    /// </summary>
    public class ExtensionsTests
    {
        [SetUp]
        public void SetUp()
        {
            App.Start();
        }

        [Test]
        public void AsFilteredView()
        {
            var source = new ObservableCollection<int>();
            using (source.AsFilteredView(x => true))
            {
            }

            using (source.AsThrottledView(TimeSpan.Zero)
                         .AsFilteredView(x => true))
            {
            }
        }

        [Test]
        public void AsReadOnlyFilteredView()
        {
            var source = new ObservableCollection<int>();
            using (source.AsReadOnlyFilteredView(x => true))
            {
            }

            using (source.AsMappingView(x => x.ToString(CultureInfo.InvariantCulture))
                         .AsReadOnlyFilteredView(x => true))
            {
            }

            using (source.AsThrottledView(TimeSpan.Zero)
                         .AsReadOnlyFilteredView(x => true))
            {
            }

            var readonlyInts = new ReadOnlyObservableCollection<int>(source);
            using (readonlyInts.AsReadOnlyFilteredView(x => true))
            {
            }

            using (readonlyInts.AsMappingView(x => x.ToString(CultureInfo.InvariantCulture))
                               .AsReadOnlyFilteredView(x => true))
            {
            }

            using (readonlyInts.AsReadOnlyThrottledView(TimeSpan.Zero)
                               .AsReadOnlyFilteredView(x => true))
            {
            }

            using (Enumerable.Range(0, 2)
                             .AsReadOnlyFilteredView(x => true, Mock.Of<IObservable<object>>()))
            {
            }
        }

        [Test]
        public void AsThrottledView()
        {
            var source = new ObservableCollection<int>();
            using (source.AsThrottledView(TimeSpan.Zero))
            {
            }

            using (source.AsFilteredView(x => true)
                       .AsThrottledView(TimeSpan.Zero))
            {
            }
        }

        [Test]
        public void AsReadOnlyThrottledView()
        {
            var source = new ObservableCollection<int>();
            using (source.AsReadOnlyThrottledView(TimeSpan.Zero))
            {
            }

            using (source.AsFilteredView(x => true)
                         .AsReadOnlyThrottledView(TimeSpan.Zero))
            {
            }

            using (source.AsMappingView(x => x.ToString(CultureInfo.InvariantCulture))
                         .AsReadOnlyThrottledView(TimeSpan.Zero))
            {
            }

            var readonlyInts = new ReadOnlyObservableCollection<int>(source);
            using (readonlyInts.AsReadOnlyThrottledView(TimeSpan.Zero))
            {
            }

            using (readonlyInts.AsReadOnlyFilteredView(x => true)
                               .AsReadOnlyThrottledView(TimeSpan.Zero))
            {
            }

            using (readonlyInts.AsMappingView(x => x.ToString(CultureInfo.InvariantCulture))
                               .AsReadOnlyThrottledView(TimeSpan.Zero))
            {
            }
        }

        [Test]
        public void AsMappingView()
        {
            // just checking that the overloads are right.
            // Will be compiler error if we dumb things.
            var source = new ObservableCollection<int>();
            using (source.AsMappingView(x => x.ToString(CultureInfo.InvariantCulture)))
            {
            }

            using (source.AsFilteredView(x => true)
                         .AsMappingView(x => x.ToString(CultureInfo.InvariantCulture)))
            {
            }

            using (source.AsThrottledView(TimeSpan.Zero)
                         .AsMappingView(x => x.ToString(CultureInfo.InvariantCulture)))
            {
            }

            var readonlyInts = new ReadOnlyObservableCollection<int>(source);
            using (readonlyInts.AsMappingView(x => x.ToString(CultureInfo.InvariantCulture)))
            {
            }

            using (readonlyInts.AsReadOnlyFilteredView(x => true)
                               .AsMappingView(x => x.ToString(CultureInfo.InvariantCulture)))
            {
            }

            using (readonlyInts.AsReadOnlyThrottledView(TimeSpan.Zero)
                               .AsMappingView(x => x.ToString(CultureInfo.InvariantCulture)))
            {
            }
        }
    }
}
