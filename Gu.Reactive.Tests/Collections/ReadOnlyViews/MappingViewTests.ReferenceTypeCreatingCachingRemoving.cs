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
        public static class ReferenceTypeCreatingCachingRemoving
        {
            public interface IDisposableVm<T> : IDisposable
            {
                Model<T>? Model { get; }
            }

            [Test]
            public static void Initializes()
            {
                var model1 = Model.Create(1);
                var model2 = Model.Create(2);
                var model3 = Model.Create(3);
                var source = new ObservableCollection<Model<int>>(
                    new[]
                        {
                        model1,
                        model1,
                        model1,
                        model2,
                        model2,
                        model2,
                        model3,
                        model3,
                        model3,
                        });
                using var view = source.AsMappingView(CreateStrictMock, x => x.Object.Dispose());
                Assert.AreSame(view[0], view[1]);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));

                foreach (var mock in view)
                {
                    mock.Setup(x => x.Dispose());
                }
            }

            [Test]
            public static void Updates()
            {
                var source = new ObservableCollection<Model<int>>();
                using var view = source.AsMappingView(CreateStrictMock, x => x.Object.Dispose());
                var model = Model.Create(1);
                source.Add(model);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));

                var mock = view[0];
                mock.Verify(x => x.Dispose(), Times.Never);
                mock.Setup(x => x.Dispose());
                source.Clear();
                CollectionAssert.IsEmpty(view);
                mock.Verify(x => x.Dispose(), Times.Once);

                source.Add(model);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));

                foreach (var toDispose in view)
                {
                    toDispose.Setup(x => x.Dispose());
                }
            }

            [Test]
            public static void UpdatesWithNulls()
            {
                var source = new ObservableCollection<Model<int>?>();
                using var view = source.AsMappingView(CreateStrictMock, x => x.Object.Dispose());
                var model = Model.Create(1);
                source.Add(model);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));

                source.Add(null);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));

                source.Add(null);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
                Assert.AreSame(view[1], view[2]);

                var mocks = view.Select(x => x).ToArray();
                foreach (var mock in mocks)
                {
                    mock.Verify(x => x.Dispose(), Times.Never);
                    mock.Setup(x => x.Dispose());
                }

                source.Clear();
                CollectionAssert.IsEmpty(view);
                foreach (var mock in mocks)
                {
                    mock.Verify(x => x.Dispose(), Times.Once);
                }

                source.Add(model);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));

                foreach (var toDispose in view)
                {
                    toDispose.Setup(x => x.Dispose());
                }
            }

            [Test]
            public static void Refresh()
            {
                var model1 = Model.Create(1);
                var model2 = Model.Create(2);
                var model3 = Model.Create(3);
                var source = new ObservableBatchCollection<Model<int>>(
                    new[]
                        {
                        model1,
                        model1,
                        model1,
                        model2,
                        model2,
                        model2,
                        model3,
                        model3,
                        model3,
                        });
                using var view = source.AsMappingView(CreateStrictMock, x => x.Object.Dispose());
                using var expected = source.SubscribeAll();
                using var actual = view.SubscribeAll();
                CollectionAssert.IsEmpty(actual);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));

                source.AddRange(new[] { model1, model2, model2 });
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
                CollectionAssert.AreEqual(expected, actual);

                var mocks = view.ToArray();
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
            public static void Caches()
            {
                var source = new ObservableCollection<Model<int>>();
                using var view = source.AsMappingView(CreateStrictMock, x => x.Object.Dispose());
                var model = Model.Create(1);
                source.Add(model);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));

                source.Add(model);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
                Assert.AreSame(view[0], view[1]);

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

                source.Add(model);
                Assert.AreNotSame(vm, view[0]);
                Assert.AreSame(vm.Object.Model, view[0].Object.Model);
                view[0].Setup(x => x.Dispose());
            }

            [Test]
            public static void CachesWhenNotEmpty()
            {
                var model1 = Model.Create(1);
                var model2 = Model.Create(2);
                var model3 = Model.Create(3);
                var source = new ObservableCollection<Model<int>>(
                    new[]
                        {
                        model1,
                        model1,
                        model1,
                        model2,
                        model2,
                        model2,
                        model3,
                        model3,
                        model3,
                        });

                using var view = source.AsMappingView(CreateStrictMock, x => x.Object.Dispose());
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));

                var model4 = Model.Create(4);
                source.Add(model4);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));

                source.Add(model4);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));

                source.Add(model1);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));

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
            }

            [Test]
            public static void Add()
            {
                var source = new ObservableCollection<Model<int>>();
                using var view = source.AsMappingView(CreateStrictMock, x => x.Object.Dispose());
                using var actual = view.SubscribeAll();
                var model1 = Model.Create(1);
                source.Add(model1);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
                var expected = new List<EventArgs>
                {
                    CachedEventArgs.CountPropertyChanged,
                    CachedEventArgs.IndexerPropertyChanged,
                    Diff.CreateAddEventArgs(view[0], 0),
                };
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                source.Add(model1);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
                expected.AddRange(new EventArgs[]
                {
                    CachedEventArgs.CountPropertyChanged,
                    CachedEventArgs.IndexerPropertyChanged,
                    Diff.CreateAddEventArgs(view[1], 1),
                });
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                source.Add(Model.Create(2));
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
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
                var model1 = Model.Create(1);
                var model2 = Model.Create(2);
                var model3 = Model.Create(3);
                var source = new ObservableCollection<Model<int>>(
                    new[]
                        {
                        model1,
                        model1,
                        model1,
                        model2,
                        model2,
                        model2,
                        model3,
                        model3,
                        model3,
                        });
                using var view = source.AsMappingView(CreateStrictMock, x => x.Object.Dispose());
                using var actual = view.SubscribeAll();
                var mock = view[0];
                source.RemoveAt(0);
                mock.Verify(x => x.Dispose(), Times.Never);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
                var expected = new List<EventArgs>
                {
                    CachedEventArgs.CountPropertyChanged,
                    CachedEventArgs.IndexerPropertyChanged,
                    Diff.CreateRemoveEventArgs(mock, 0),
                };
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                source.RemoveAt(0);
                mock.Verify(x => x.Dispose(), Times.Never);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
                expected.AddRange(new EventArgs[]
                {
                    CachedEventArgs.CountPropertyChanged,
                    CachedEventArgs.IndexerPropertyChanged,
                    Diff.CreateRemoveEventArgs(mock, 0),
                });
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                mock.Setup(x => x.Dispose());
                source.RemoveAt(0);
                mock.Verify(x => x.Dispose(), Times.Once);
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
                expected.AddRange(new EventArgs[]
                {
                    CachedEventArgs.CountPropertyChanged,
                    CachedEventArgs.IndexerPropertyChanged,
                    Diff.CreateRemoveEventArgs(mock, 0),
                });
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                foreach (var toDispose in view)
                {
                    toDispose.Setup(x => x.Dispose());
                }
            }

            [Test]
            public static void Replace()
            {
                var model1 = Model.Create(1);
                var model2 = Model.Create(2);
                var model3 = Model.Create(3);
                var source = new ObservableCollection<Model<int>>(
                    new[]
                        {
                        model1,
                        model1,
                        model1,
                        model2,
                        model2,
                        model2,
                        model3,
                        model3,
                        model3,
                        });
                using var view = source.AsMappingView(CreateStrictMock, x => x.Object.Dispose());
                using var actual = view.SubscribeAll();
                var old = view[0];
                var @new = view[5];
                source[0] = source[5];
                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
                var expected = new List<EventArgs>
                {
                    CachedEventArgs.IndexerPropertyChanged,
                    Diff.CreateReplaceEventArgs(@new, old, 0),
                };
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                foreach (var mock in view)
                {
                    mock.Setup(x => x.Dispose());
                }
            }

            [Test]
            public static void Move()
            {
                var model1 = Model.Create(1);
                var model2 = Model.Create(2);
                var model3 = Model.Create(3);
                var source = new ObservableCollection<Model<int>>(
                    new[]
                        {
                        model1,
                        model1,
                        model1,
                        model2,
                        model2,
                        model2,
                        model3,
                        model3,
                        model3,
                        });
                using var view = source.AsMappingView(CreateStrictMock, x => x.Object.Dispose());
                using (var actual = view.SubscribeAll())
                {
                    source.Move(0, 4);
                    CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
                    var expected = new List<EventArgs>
                    {
                        CachedEventArgs.IndexerPropertyChanged,
                        Diff.CreateMoveEventArgs(view[4], 4, 0),
                    };
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }

                foreach (var mock in view)
                {
                    mock.Setup(x => x.Dispose());
                }
            }

            [Test]
            public static void Clear()
            {
                var model1 = Model.Create(1);
                var model2 = Model.Create(2);
                var model3 = Model.Create(3);
                var source = new ObservableCollection<Model<int>>(
                    new[]
                        {
                        model1,
                        model1,
                        model1,
                        model2,
                        model2,
                        model2,
                        model3,
                        model3,
                        model3,
                        });
                using var view = source.AsMappingView(CreateStrictMock, x => x.Object.Dispose());
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

                CollectionAssert.AreEqual(source, view.Select(x => x.Object.Model));
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
                var model1 = Model.Create(1);
                var model2 = Model.Create(2);
                var model3 = Model.Create(3);
                var source = new ObservableCollection<Model<int>>(
                    new[]
                        {
                        model1,
                        model1,
                        model1,
                        model2,
                        model2,
                        model2,
                        model3,
                        model3,
                        model3,
                        });
                using var view = source.AsMappingView(CreateStrictMock, x => x.Object.Dispose());
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

            private static Mock<IDisposableVm<T>> CreateStrictMock<T>(Model<T>? model)
            {
                var mock = new Mock<IDisposableVm<T>>(MockBehavior.Strict);
                mock.SetupGet(x => x.Model)
                    .Returns(model);
                return mock;
            }
        }
    }
}
