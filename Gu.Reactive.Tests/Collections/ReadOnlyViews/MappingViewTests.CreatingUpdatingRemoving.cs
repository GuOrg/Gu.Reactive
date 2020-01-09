namespace Gu.Reactive.Tests.Collections.ReadOnlyViews
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Gu.Reactive.Tests.Helpers;

    using Moq;

    using NUnit.Framework;

    public static partial class MappingViewTests
    {
        public static class CreatingUpdatingRemoving
        {
            public interface IDisposableVm : IDisposable
            {
                int Value { get; }

                string Name { get; }

                int Index { get; set; }
            }

            [Test]
            public static void Initializes()
            {
                var source = new ObservableCollection<int>
                {
                    1,
                    1,
                    1,
                    2,
                    2,
                    2,
                    3,
                    3,
                    3,
                };
                using var view = source.AsMappingView(CreateStrictMock, WithIndex, x => x.Object.Dispose());
                Assert.AreNotSame(view[0], view[1]);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Value));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Object.Index));

                foreach (var mock in view)
                {
                    mock.Setup(x => x.Dispose());
                }
            }

            [Test]
            public static void Updates()
            {
                var source = new ObservableCollection<int>();
                using var view = source.AsMappingView(CreateStrictMock, WithIndex, x => x.Object.Dispose());
                source.Add(1);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Value));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Object.Index));

                var mock = view[0];
                mock.Verify(x => x.Dispose(), Times.Never);
                mock.Setup(x => x.Dispose());
                source.Clear();
                CollectionAssert.IsEmpty(view);
                mock.Verify(x => x.Dispose(), Times.Once);

                source.Add(1);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Value));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Object.Index));

                foreach (var toDispose in view)
                {
                    toDispose.Setup(x => x.Dispose());
                }
            }

            [Test]
            public static void Refresh()
            {
                var source = new ObservableBatchCollection<int>
                {
                    1,
                    1,
                    1,
                    2,
                    2,
                    2,
                    3,
                    3,
                    3,
                };
                using var view = source.AsMappingView(CreateStrictMock, WithIndex, x => x.Object.Dispose());
                using var expected = source.SubscribeAll();
                using var actual = view.SubscribeAll();
                CollectionAssert.IsEmpty(actual);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Value));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Object.Index));

                var mocks = view.ToArray();
                foreach (var mock in mocks)
                {
                    mock.Setup(x => x.Dispose());
                }

                source.AddRange(new[] { 1, 2, 2 });
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Value));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Object.Index));
                CollectionAssert.AreEqual(expected, actual);
                foreach (var mock in mocks)
                {
                    mock.Verify(x => x.Dispose(), Times.Once);
                }

                mocks = view.ToArray();
                foreach (var mock in mocks)
                {
                    mock.Setup(x => x.Dispose());
                }

                source.Clear();
                CollectionAssert.IsEmpty(view);
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                foreach (var mock in mocks)
                {
                    mock.Verify(x => x.Dispose(), Times.Once);
                }
            }

            [Test]
            public static void DoesNotCache()
            {
                var source = new ObservableCollection<int>();
                using var view = source.AsMappingView(CreateStrictMock, WithIndex, x => x.Object.Dispose());
                source.Add(1);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Value));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Object.Index));

                source.Add(1);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Value));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Object.Index));
                Assert.AreNotSame(view[0], view[1]);

                var mocks = view.ToArray();
                var vm = view[0];
                foreach (var mock in mocks)
                {
                    mock.Setup(x => x.Dispose());
                }

                source.Clear();
                CollectionAssert.IsEmpty(view);
                foreach (var mock in mocks)
                {
                    mock.Verify(x => x.Dispose(), Times.Once);
                }

                source.Add(2);
                Assert.AreNotSame(vm, view[0]);
                Assert.AreEqual(2, view[0].Object.Value);

                view[0].Setup(x => x.Dispose());
            }

            [Test]
            public static void Add()
            {
                var source = new ObservableCollection<int>();
                using var view = source.AsMappingView(CreateStrictMock, WithIndex, x => x.Object.Dispose());
                using var actual = view.SubscribeAll();
                source.Add(1);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Value));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Object.Index));
                var expected = new List<EventArgs>
                {
                    CachedEventArgs.CountPropertyChanged,
                    CachedEventArgs.IndexerPropertyChanged,
                    Diff.CreateAddEventArgs(view[0], 0),
                };
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                source.Add(1);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Value));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Object.Index));
                expected.AddRange(new EventArgs[]
                {
                    CachedEventArgs.CountPropertyChanged,
                    CachedEventArgs.IndexerPropertyChanged,
                    Diff.CreateAddEventArgs(view[1], 1),
                });
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                source.Add(2);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Value));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Object.Index));
                expected.AddRange(new EventArgs[]
                {
                    CachedEventArgs.CountPropertyChanged,
                    CachedEventArgs.IndexerPropertyChanged,
                    Diff.CreateAddEventArgs(view[2], 2),
                });
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                foreach (var mock in view)
                {
                    mock.Setup(x => x.Dispose());
                }
            }

            [Test]
            public static void Remove()
            {
                var source = new ObservableCollection<int>
                {
                    1,
                    1,
                    1,
                    2,
                    2,
                    2,
                    3,
                    3,
                    3,
                };
                using var view = source.AsMappingView(CreateStrictMock, WithIndex, x => x.Object.Dispose());
                using (var actual = view.SubscribeAll())
                {
                    var mock = view[0];
                    mock.Setup(x => x.Dispose());
                    source.RemoveAt(0);
                    mock.Verify(x => x.Dispose(), Times.Once);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Object.Value));
                    CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Object.Index));
                    var expected = new List<EventArgs>
                    {
                        CachedEventArgs.CountPropertyChanged,
                        CachedEventArgs.IndexerPropertyChanged,
                        Diff.CreateRemoveEventArgs(mock, 0),
                    };
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                    mock = view[0];
                    mock.Setup(x => x.Dispose());
                    source.RemoveAt(0);
                    mock.Verify(x => x.Dispose(), Times.Once);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Object.Value));
                    CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Object.Index));
                    expected.AddRange(new EventArgs[]
                    {
                        CachedEventArgs.CountPropertyChanged,
                        CachedEventArgs.IndexerPropertyChanged,
                        Diff.CreateRemoveEventArgs(mock, 0),
                    });
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                    mock = view[0];
                    mock.Setup(x => x.Dispose());
                    source.RemoveAt(0);
                    mock.Verify(x => x.Dispose(), Times.Once);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Object.Value));
                    CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Object.Index));
                    expected.AddRange(new EventArgs[]
                    {
                        CachedEventArgs.CountPropertyChanged,
                        CachedEventArgs.IndexerPropertyChanged,
                        Diff.CreateRemoveEventArgs(mock, 0),
                    });
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }

                foreach (var mock in view)
                {
                    mock.Setup(x => x.Dispose());
                }
            }

            [Test]
            public static void Replace()
            {
                var source = new ObservableCollection<int>
                {
                    1,
                    1,
                    1,
                    2,
                    2,
                    2,
                    3,
                    3,
                    3,
                };
                using var view = source.AsMappingView(CreateStrictMock, WithIndex, x => x.Object.Dispose());
                using (var actual = view.SubscribeAll())
                {
                    var old = view[0];
                    old.Setup(x => x.Dispose());
                    source[0] = source[5];
                    old.Verify(x => x.Dispose(), Times.Once);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Object.Value));
                    CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Object.Index));
                    var expected = new List<EventArgs>
                    {
                        CachedEventArgs.IndexerPropertyChanged,
                        Diff.CreateReplaceEventArgs(view[0], old, 0),
                    };
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }

                foreach (var mock in view)
                {
                    mock.Setup(x => x.Dispose());
                }
            }

            [Test]
            public static void Move()
            {
                var source = new ObservableCollection<int>
                {
                    1,
                    1,
                    1,
                    2,
                    2,
                    2,
                    3,
                    3,
                    3,
                };
                using var view = source.AsMappingView(CreateStrictMock, WithIndex, x => x.Object.Dispose());
                using var actual = view.SubscribeAll();
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Value));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Object.Index));

                source.Move(0, 4);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Value));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Object.Index));
                var expected = new List<EventArgs>
                {
                    CachedEventArgs.IndexerPropertyChanged,
                    Diff.CreateMoveEventArgs(view[4], 4, 0),
                };
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                foreach (var mock in view)
                {
                    mock.Setup(x => x.Dispose());
                }
            }

            [Test]
            public static void MoveWhenUpdateCreatesNew()
            {
                var source = new ObservableCollection<int>
                {
                    1,
                    1,
                    1,
                    2,
                    2,
                    2,
                    3,
                    3,
                    3,
                };
                using var view = source.AsMappingView(CreateStrictMock, NewWithIndex, x => x.Object.Dispose());
                using var actual = view.SubscribeAll();
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Value));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Object.Index));

                var mocks = view.Skip(1)
                                .Take(3)
                                .ToArray();
                foreach (var mock in mocks)
                {
                    mock.Setup(x => x.Dispose());
                }

                source.Move(1, 3);
                foreach (var mock in mocks)
                {
                    mock.Verify(x => x.Dispose(), Times.Once);
                }

                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Value));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Object.Index));
                var expected = new List<EventArgs>
                {
                    CachedEventArgs.CountPropertyChanged,
                    CachedEventArgs.IndexerPropertyChanged,
                    CachedEventArgs.NotifyCollectionReset,
                };
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                foreach (var mock in view)
                {
                    mock.Setup(x => x.Dispose());
                }
            }

            [Test]
            public static void Clear()
            {
                var source = new ObservableCollection<int>
                {
                    1,
                    1,
                    1,
                    2,
                    2,
                    2,
                    3,
                    3,
                    3,
                };
                using var view = source.AsMappingView(CreateStrictMock, WithIndex, x => x.Object.Dispose());
                using var actual = view.SubscribeAll();
                var mocks = view.ToArray();
                foreach (var mock in mocks)
                {
                    mock.Setup(x => x.Dispose());
                }

                source.Clear();
                CollectionAssert.IsEmpty(view);
                foreach (var mock in mocks)
                {
                    mock.Verify(x => x.Dispose(), Times.Once);
                }

                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Value));
                CollectionAssert.AreEqual(source.Select((_, i) => i), view.Select(x => x.Object.Index));
                var expected = new List<EventArgs>
                {
                    CachedEventArgs.CountPropertyChanged,
                    CachedEventArgs.IndexerPropertyChanged,
                    CachedEventArgs.NotifyCollectionReset,
                };
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
            }

            [Test]
            public static void Dispose()
            {
                var source = new ObservableCollection<int>
                {
                    1,
                    1,
                    1,
                    2,
                    2,
                    2,
                    3,
                    3,
                    3,
                };
                using var view = source.AsMappingView(CreateStrictMock, WithIndex, x => x.Object.Dispose());
                var mocks = view.ToArray();
                foreach (var mock in mocks)
                {
                    mock.Setup(x => x.Dispose());
                }

#pragma warning disable IDISP016 // Don't use disposed instance.
                view.Dispose();
#pragma warning restore IDISP016 // Don't use disposed instance.
                foreach (var mock in mocks)
                {
                    mock.Verify(x => x.Dispose(), Times.Once);
                }

                _ = Assert.Throws<ObjectDisposedException>(() => CollectionAssert.IsEmpty(view));
                _ = Assert.Throws<ObjectDisposedException>(() => Assert.AreEqual(0, view.Count));
            }

            private static Mock<IDisposableVm> CreateStrictMock(int value, int index)
            {
                var mock = new Mock<IDisposableVm>(MockBehavior.Strict);
                mock.SetupGet(x => x.Value).Returns(value);

                mock.Name = "Original";

                mock.SetupGet(x => x.Index).Returns(index);
                return mock;
            }

            private static Mock<IDisposableVm> WithIndex(Mock<IDisposableVm> mock, int index)
            {
                mock.SetupGet(x => x.Index).Returns(index);
                return mock;
            }

            private static Mock<IDisposableVm> NewWithIndex(Mock<IDisposableVm> mock, int index)
            {
                var newMock = new Mock<IDisposableVm>(MockBehavior.Strict);
                newMock.SetupGet(x => x.Value)
                       .Returns(mock.Object.Value);

                newMock.Name = "New index";

                newMock.SetupGet(x => x.Index)
                       .Returns(index);
                return newMock;
            }
        }
    }
}
