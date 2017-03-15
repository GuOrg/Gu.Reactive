namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections.ObjectModel;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public partial class MappingViewTests
    {
        public class Nested
        {
            [Test]
            public void Add()
            {
                var source = new ObservableCollection<int>();
                using (var modelView = source.AsMappingView(x => new Model(x)))
                {
                    using (var modelViewChanges = modelView.SubscribeAll())
                    {
                        using (var indexedView = modelView.AsMappingView((x, i) => new Vm { Model = x, Index = i }, (x, i) => x))
                        {
                            using (var indexedChanges = indexedView.SubscribeAll())
                            {
                                using (var vmView = modelView.AsMappingView(x => new Vm { Model = x }))
                                {
                                    using (var vmViewChanges = vmView.SubscribeAll())
                                    {
                                        source.Add(1);
                                        var expected = new EventArgs[]
                                        {
                                            CachedEventArgs.CountPropertyChanged,
                                            CachedEventArgs.IndexerPropertyChanged,
                                            null
                                        };

                                        expected[2] = Diff.CreateAddEventArgs(modelView[0], 0);
                                        CollectionAssert.AreEqual(expected, modelViewChanges, EventArgsComparer.Default);

                                        expected[2] = Diff.CreateAddEventArgs(vmView[0], 0);
                                        CollectionAssert.AreEqual(expected, vmViewChanges, EventArgsComparer.Default);

                                        expected[2] = Diff.CreateAddEventArgs(indexedView[0], 0);
                                        CollectionAssert.AreEqual(expected, indexedChanges, EventArgsComparer.Default);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            [Test]
            public void Remove()
            {
                var source = new ObservableCollection<int>(new[] { 1 });
                using (var modelView = source.AsMappingView(x => new Model(x)))
                {
                    using (var modelViewChanges = modelView.SubscribeAll())
                    {
                        using (var vmView = modelView.AsMappingView(x => new Vm { Model = x }))
                        {
                            using (var vmViewChanges = vmView.SubscribeAll())
                            {
                                var oldModel = modelView[0];
                                var oldView = vmView[0];
                                source.Remove(1);

                                var expected = new EventArgs[]
                                                   {
                                                   CachedEventArgs.CountPropertyChanged,
                                                   CachedEventArgs.IndexerPropertyChanged,
                                                   null
                                                   };
                                expected[2] = Diff.CreateRemoveEventArgs(oldModel, 0);
                                CollectionAssert.AreEqual(expected, modelViewChanges, EventArgsComparer.Default);
                                expected[2] = Diff.CreateRemoveEventArgs(oldView, 0);
                                CollectionAssert.AreEqual(expected, vmViewChanges, EventArgsComparer.Default);
                            }
                        }
                    }
                }
            }
        }
    }
}