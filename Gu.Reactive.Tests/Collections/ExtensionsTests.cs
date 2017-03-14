#pragma warning disable CS0618 // Type or member is obsolete
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
            using (ints.AsFilteredView(x => true))
            {
            }

            using (ints.AsThrottledView(TimeSpan.Zero)
                       .AsFilteredView(x => true))
            {
            }
        }

        [Test]
        public void AsReadOnlyFilteredView()
        {
            var ints = new ObservableCollection<int>();
            using (ints.AsReadOnlyFilteredView(x => true))
            {
            }

            using (ints.AsMappingView(x => x.ToString())
                       .AsReadOnlyFilteredView(x => true))
            {
            }

            using (ints.AsThrottledView(TimeSpan.Zero)
                       .AsReadOnlyFilteredView(x => true))
            {
            }

            var readonlyInts = new ReadOnlyObservableCollection<int>(ints);
            using (readonlyInts.AsReadOnlyFilteredView(x => true))
            {
            }

            using (readonlyInts.AsMappingView(x => x.ToString())
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
            var ints = new ObservableCollection<int>();
            using (ints.AsThrottledView(TimeSpan.Zero))
            {
            }

            using (ints.AsFilteredView(x => true)
                       .AsThrottledView(TimeSpan.Zero))
            {
            }
        }

        [Test]
        public void AsReadOnlyThrottledView()
        {
            var ints = new ObservableCollection<int>();
            using (ints.AsReadOnlyThrottledView(TimeSpan.Zero))
            {
            }

            using (ints.AsFilteredView(x => true)
                       .AsReadOnlyThrottledView(TimeSpan.Zero))
            {
            }

            using (ints.AsMappingView(x => x.ToString())
                       .AsReadOnlyThrottledView(TimeSpan.Zero))
            {
            }

            var readonlyInts = new ReadOnlyObservableCollection<int>(ints);
            using (readonlyInts.AsReadOnlyThrottledView(TimeSpan.Zero))
            {
            }

            using (readonlyInts.AsReadOnlyFilteredView(x => true)
                               .AsReadOnlyThrottledView(TimeSpan.Zero))
            {
            }

            using (readonlyInts.AsMappingView(x => x.ToString())
                               .AsReadOnlyThrottledView(TimeSpan.Zero))
            {
            }
        }

        [Test]
        public void AsMappingView()
        {
            // just checking that the overloads are right.
            // Will be compiler error if we dumb things.
            var ints = new ObservableCollection<int>();
            using (ints.AsMappingView(x => x.ToString()))
            {
            }

            using (ints.AsFilteredView(x => true)
                       .AsMappingView(x => x.ToString()))
            {
            }

            using (ints.AsThrottledView(TimeSpan.Zero)
                       .AsMappingView(x => x.ToString()))
            {
            }

            var readonlyInts = new ReadOnlyObservableCollection<int>(ints);
            using (readonlyInts.AsMappingView(x => x.ToString()))
            {
            }

            using (readonlyInts.AsReadOnlyFilteredView(x => true)
                               .AsMappingView(x => x.ToString()))
            {
            }

            using (readonlyInts.AsReadOnlyThrottledView(TimeSpan.Zero)
                               .AsMappingView(x => x.ToString()))
            {
            }
        }
    }
}
